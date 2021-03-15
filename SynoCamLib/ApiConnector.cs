using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SynoCamLib
{
    /// <summary>
    /// Interacts with Synology API, SPEC 2.0 (2015/3/13)
    /// </summary>
    internal class ApiConnector : IApiConnector
    {
        private readonly string _url;
        private readonly string _username;
        private readonly string _password;
        private string _sessionId;

        private readonly List<string> _deleteFilesBeforeExit = new List<string>();

        public ApiConnector(string url, string username, string password, bool forceCorrectCertificate = false)
        {
            // Many home servers do not have a valid certificate, do not check unless requested
            if (!forceCorrectCertificate)
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;

            _url = url;
            _username = username;
            _password = password;
        }

        private async Task<string> LoginASync(string username, string password)
        {
            var httpRequest = new HttpRequest();
            httpRequest.Parameters.Add("api", "SYNO.API.Auth");
            httpRequest.Parameters.Add("version", "6");
            httpRequest.Parameters.Add("method", "Login");
            httpRequest.Parameters.Add("account", username);
            httpRequest.Parameters.Add("passwd", password);
            httpRequest.Parameters.Add("session", "SurveillanceStation");
            httpRequest.Parameters.Add("format", "sid");

            string jsonResponse = await httpRequest.GetASync(_url + "auth.cgi");
            var javaScriptSerializer = new JavaScriptSerializer();
            var dict = javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);
            if (dict["success"] == true)
            {
                return dict["data"]["sid"];
            }

            return string.Empty;
        }

        public async Task<bool> LogoutASync()
        {
            foreach (var file in _deleteFilesBeforeExit)
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception)
                {
                    // We have done our best
                }
            }

            if (_sessionId == null)
                return true;

            var httpRequest = new HttpRequest();
            httpRequest.Parameters.Add("api", "SYNO.API.Auth");
            httpRequest.Parameters.Add("version", "6");
            httpRequest.Parameters.Add("method", "Logout");
            httpRequest.Parameters.Add("session", "SurveillanceStation");
            httpRequest.Parameters.Add("_sid", _sessionId);


            await httpRequest.GetASync(_url + "auth.cgi", 200); // There is no data coming back, just continue after some time

            return true;
        }

        public async Task<List<ICam>> GetCamsASync()
        {
            var cams = new List<ICam>();

            if (_sessionId == null)
            {
                _sessionId = await LoginASync(_username, _password);
            }

            var httpRequest = new HttpRequest();
            httpRequest.Parameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.Parameters.Add("version", "8");
            httpRequest.Parameters.Add("method", "List");
            httpRequest.Parameters.Add("_sid", _sessionId);
            httpRequest.Parameters.Add("uri", _url);
            httpRequest.Parameters.Add("limit", "4");
            httpRequest.Parameters.Add("camStm", "2"); // Mobile stream
            httpRequest.Parameters.Add("blIncludeDeletedCam", "false"); // No deleted cameras
            httpRequest.Parameters.Add("privCamType", "1"); //Live view cameras
            httpRequest.Parameters.Add("basic", "true"); // Include basic cam information

            var cameraListDictionary = await GetDataFromUrl(httpRequest, _url + "entry.cgi");

            dynamic listOfCams = cameraListDictionary["data"]["cameras"];
            foreach (var cam in listOfCams)
            {
                var status = (CamStatus)cam["status"];
                var realCam = new Cam(cam["name"], status, cam["enabled"] && (status == CamStatus.Normal), GetCamImageUrl(cam["id"].ToString()));

                var live = GetLiveViewPath(cam["id"].ToString());

                cams.Add(realCam);
            }

            return cams;
        }

        private async Task<Dictionary<string, dynamic>> GetDataFromUrl(HttpRequest httpRequest, string entrypoint)
        {
            string jsonResponse = await httpRequest.GetASync(entrypoint);
            var javaScriptSerializer = new JavaScriptSerializer();
            var cameraListDictionary = javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);

            if (cameraListDictionary.ContainsKey("error"))
                throw new ApplicationException("Error received from server: " + cameraListDictionary["error"]["code"]);

            return cameraListDictionary;
        }

        private string GetLiveViewPath(string camId)
        {
            var httpRequest = new HttpRequest();
            httpRequest.Parameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.Parameters.Add("version", "9");
            httpRequest.Parameters.Add("_sid", _sessionId);
            httpRequest.Parameters.Add("idList", camId);
            httpRequest.Parameters.Add("method", "GetLiveViewPath");
            httpRequest.Parameters.Add("uri", _url);

            return httpRequest.GetUrl(_url + "entry.cgi");
        }

        private string GetCamImageUrl(string camId)
        {
            var httpRequest = new HttpRequest();
            httpRequest.Parameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.Parameters.Add("version", "9");
            httpRequest.Parameters.Add("_sid", _sessionId);            
            httpRequest.Parameters.Add("id", camId);
            httpRequest.Parameters.Add("method", "GetSnapshot");
            httpRequest.Parameters.Add("uri", _url);

            return httpRequest.GetUrl(_url + "entry.cgi");
        }

        public async Task<IEnumerable<ICamEvent>> QueryCamEvents()
        {
            var events = new List<CamEvent>();

            var httpRequest = new HttpRequest();
            httpRequest.Parameters.Add("api", "SYNO.SurveillanceStation.Event");
            httpRequest.Parameters.Add("version", "4");
            httpRequest.Parameters.Add("_sid", _sessionId);
            httpRequest.Parameters.Add("limit", "50");
            httpRequest.Parameters.Add("method", "List");
            httpRequest.Parameters.Add("blIncludeSnapshot", "true");

            var eventData = await GetDataFromUrl(httpRequest, _url + "entry.cgi");

            foreach (var eventEntry in eventData["data"]["events"])
            {
                var id = eventEntry["eventId"];
                var camName = eventEntry["camera_name"];
                var name = eventEntry["name"];
                var snapshot = eventEntry["snapshot_medium"];
                var reason = eventEntry["reason"];
                var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(eventEntry["startTime"]);
                var stopTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(eventEntry["stopTime"]);

                events.Add(new CamEvent(id, name, camName, startTime, stopTime, (EventReason)reason, snapshot));
            }

            return events;
        }

        // Does not work yet.. does not return at the moment, not sure why, specs seem to be correctly implemented
        //public async Task<string> GetEventStream(CamEvent camEvent)
        //{
        //    var httpRequest = new HttpRequest();
        //    httpRequest.Parameters.Add("api", "SYNO.SurveillanceStation.Streaming");
        //    httpRequest.Parameters.Add("version", "1");
        //    httpRequest.Parameters.Add("_sid", _sessionId);
        //    httpRequest.Parameters.Add("eventId", camEvent.Id.ToString());
        //    httpRequest.Parameters.Add("method", "EventStream");

        //    httpRequest.Headers.Add("User-Agent", "SynoCam");
        //    httpRequest.Headers.Add("Range", "bytes=0-9999999");
        //    httpRequest.Headers.Add("Icy-MetaData", "1");

        //    var eventData = await GetDataFromUrl(httpRequest, _url + "streaming.cgi");

        //    return "";
        //}

        public string DownloadEvent(ICamEvent camEvent, AsyncCompletedEventHandler fileDownloadCompleted, DownloadProgressChangedEventHandler progressChanged)
        {
            string tempPath = Path.GetTempPath();

            string nameWithoutPrefix = camEvent.Name.Substring(camEvent.Name.LastIndexOf("/", StringComparison.Ordinal) + 1);
            string url = string.Format("{0}entry.cgi/{1}?api=SYNO.SurveillanceStation.Event&method=Download&version=4&eventId={2}&_sid={3}", _url, nameWithoutPrefix, camEvent.Id, _sessionId);

            string tempFile = Path.Combine(tempPath, nameWithoutPrefix);

            using (var downloadClient = new WebClient())
            {
                downloadClient.DownloadFileCompleted += fileDownloadCompleted;
                downloadClient.DownloadFileCompleted += (sender, args) => _deleteFilesBeforeExit.Add(tempFile);
                downloadClient.DownloadProgressChanged += progressChanged;
                downloadClient.DownloadFileAsync(new Uri(url), tempFile);
            }

            return tempFile;
        }
    }
}
