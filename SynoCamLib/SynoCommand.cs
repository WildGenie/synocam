using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SynoCamLib
{
    /// <summary>
    /// Interacts with Synology API, SPEC 2.0 (2015/3/13)
    /// </summary>
    public class SynoCommand
    {
        private readonly string _url;
        private readonly string _username;
        private readonly string _password;
        private string _sessionId;

        public SynoCommand(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        private async Task<string> LoginASync(string username, string password)
        {
            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.API.Auth");
            httpRequest.GetParameters.Add("version", "2");
            httpRequest.GetParameters.Add("method", "Login");
            httpRequest.GetParameters.Add("account", username);
            httpRequest.GetParameters.Add("passwd", password);
            httpRequest.GetParameters.Add("session", "SurveillanceStation");
            httpRequest.GetParameters.Add("format", "sid");

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
            if (_sessionId == null)
                return true;

            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.API.Auth");
            httpRequest.GetParameters.Add("version", "2");
            httpRequest.GetParameters.Add("method", "Logout");
            httpRequest.GetParameters.Add("session", "SurveillanceStation");
            httpRequest.GetParameters.Add("_sid", _sessionId);

            var result = await httpRequest.GetASync(_url + "auth.cgi");

            _sessionId = null;

            return result != string.Empty;
        }

        public async Task<List<CamUi>> GetCamsASync(int refreshRate)
        {
            var cams = new List<CamUi>();

            if (_sessionId == null)
            {
                _sessionId = await LoginASync(_username, _password);
            }

            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.GetParameters.Add("version", "8");
            httpRequest.GetParameters.Add("method", "List");
            httpRequest.GetParameters.Add("_sid", _sessionId);
            httpRequest.GetParameters.Add("uri", _url);
            httpRequest.GetParameters.Add("limit", "4");
            httpRequest.GetParameters.Add("camStm", "2"); // Mobile stream
            httpRequest.GetParameters.Add("blIncludeDeletedCam", "false"); // No deleted cameras
            httpRequest.GetParameters.Add("privCamType", "1"); //Live view cameras
            httpRequest.GetParameters.Add("basic", "true"); // Include basic cam information
            
            var cameraListDictionary = await GetDataFromUrl(httpRequest);

            dynamic listOfCams = cameraListDictionary["data"]["cameras"];
            foreach (var cam in listOfCams)
            {
                var status = (CamStatus) cam["status"];
                var realCam = new CamUi(cam["name"], status, cam["enabled"] && (status == CamStatus.Normal), GetCamImageUrl(cam["id"].ToString()), refreshRate);
                cams.Add(realCam);
            }

            return cams;
        }

        private async Task<Dictionary<string, dynamic>> GetDataFromUrl(HttpRequest httpRequest)
        {
            string jsonResponse = await httpRequest.GetASync(_url + "entry.cgi");
            var javaScriptSerializer = new JavaScriptSerializer();
            var cameraListDictionary = javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);

            if (cameraListDictionary.ContainsKey("error"))
                throw new ApplicationException("Error received from server: " + cameraListDictionary["error"]["code"]);

            return cameraListDictionary;
        }

        public string GetCamImageUrl(string camId)
        {
            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.GetParameters.Add("version", "8");
            httpRequest.GetParameters.Add("_sid", _sessionId);
            httpRequest.GetParameters.Add("cameraId", camId);
            httpRequest.GetParameters.Add("method", "GetSnapshot");
            httpRequest.GetParameters.Add("uri", _url);

            return httpRequest.GetUrl(_url + "entry.cgi");
        }

        public async Task<IEnumerable<CamEvent>> QueryCamEvents()
        {
            var events = new List<CamEvent>();

            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Event");
            httpRequest.GetParameters.Add("version", "4");
            httpRequest.GetParameters.Add("_sid", _sessionId);
            httpRequest.GetParameters.Add("limit", "50");
            httpRequest.GetParameters.Add("method", "List");
            httpRequest.GetParameters.Add("blIncludeSnapshot", "true");

            var eventData = await GetDataFromUrl(httpRequest);

            foreach (var eventEntry in eventData["data"]["events"])
            {
                var name = eventEntry["camera_name"];
                var snapshot = eventEntry["snapshot_medium"];
                var reason = eventEntry["reason"];
                var startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(eventEntry["startTime"]);
                var stopTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(eventEntry["stopTime"]);

                events.Add(new CamEvent(name, startTime, stopTime, (EventReason)reason, snapshot));
            }

            return events;
        }
    }
}