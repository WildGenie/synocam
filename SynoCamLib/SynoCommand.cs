using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SynoCamLib
{
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


        public async Task<string> LoginASync(string username, string password)
        {
            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.API.Auth");
            httpRequest.GetParameters.Add("version", "2");
            httpRequest.GetParameters.Add("method", "Login");
            httpRequest.GetParameters.Add("account", username);
            httpRequest.GetParameters.Add("passwd", password);
            httpRequest.GetParameters.Add("session", "SurveillanceStation");

            string jsonResponse = await httpRequest.GetASync(_url + "auth.cgi");
            var javaScriptSerializer = new JavaScriptSerializer();
            var dict = javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);
            if (dict["success"] == true)
            {
                return dict["data"]["sid"];
            }

            return string.Empty;
        }

        public async void LogoutASync()
        {
            if (_sessionId == null)
                return;

            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("_sid", _sessionId);
            httpRequest.GetParameters.Add("api", "SYNO.API.Auth");
            httpRequest.GetParameters.Add("version", "1");
            httpRequest.GetParameters.Add("method", "logout");
            httpRequest.GetParameters.Add("session", "SurveillanceStation");

            await httpRequest.GetASync(_url + "auth.cgi");

            _sessionId = null;
        }

        public async Task<List<Cam>> GetCamsASync()
        {
            var cams = new List<Cam>();

            string session = _sessionId;

            if (_sessionId == null)
            {
                session = await LoginASync(_username, _password);
                _sessionId = session;
            }

            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("_sid", session);
            httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.GetParameters.Add("version", "3");
            httpRequest.GetParameters.Add("method", "List");
            httpRequest.GetParameters.Add("uri", _url);
            httpRequest.GetParameters.Add("limit", "4");

            string jsonResponse = await httpRequest.GetASync(_url + "entry.cgi");
            var javaScriptSerializer = new JavaScriptSerializer();
            var cameraListDictionary = javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);

            if (cameraListDictionary.ContainsKey("error"))
                throw new ApplicationException("Error received from server: " + cameraListDictionary["error"]["code"]);

            dynamic listOfCams = cameraListDictionary["data"]["cameras"];
            foreach (var cam in listOfCams)
            {
                var status = (CamStatus) cam["status"];
                var realCam = new Cam(cam["name"], status, cam["enabled"] && (status == CamStatus.Normal), GetCamImageUrl(cam["id"].ToString()));
                cams.Add(realCam);
            }

            return cams;
        }

        public string GetCamImageUrl(string camId)
        {
            var httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("_sid", _sessionId);
            httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Camera");
            httpRequest.GetParameters.Add("version", "2");
            httpRequest.GetParameters.Add("cameraId", camId);
            httpRequest.GetParameters.Add("method", "GetSnapshot");
            httpRequest.GetParameters.Add("uri", _url);

            return httpRequest.GetUrl(_url + "entry.cgi");
        }
    }
}