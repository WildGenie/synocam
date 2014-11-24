using System;
using System.Collections.Generic;
using System.Security.Authentication;
using System.Web.Script.Serialization;

namespace cam
{
    public class SynoCommand
    {
        private readonly string url;
        private string sessionId;
        
        public SynoCommand(string url)
        {
            this.url = url;
        }

        public bool LoginRequired()
        {
            return sessionId == null;
        }
        
        public void Login(string username, string password)
        {
            HttpRequest httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("api", "SYNO.API.Auth");
            httpRequest.GetParameters.Add("version", "2");
            httpRequest.GetParameters.Add("method", "Login");
            httpRequest.GetParameters.Add("account", username);
            httpRequest.GetParameters.Add("passwd", password);
            httpRequest.GetParameters.Add("session", "SurveillanceStation");

            try
            {
                string jsonResponse = httpRequest.Get(url + "auth.cgi");
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                var dict = javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);
                if (dict["success"] == true)
                    sessionId = dict["data"]["sid"];
            }
            catch (Exception)
            {                
                sessionId = null;
                throw;
            }
            
        }

        public void Logout()
        {
            if (sessionId == null)
                return;

            HttpRequest httpRequest = new HttpRequest();
            httpRequest.GetParameters.Add("_sid", sessionId);
            httpRequest.GetParameters.Add("api", "SYNO.API.Auth");
            httpRequest.GetParameters.Add("version", "1");
            httpRequest.GetParameters.Add("method", "logout");
            httpRequest.GetParameters.Add("session", "SurveillanceStation");

            httpRequest.Get(url + "auth.cgi");

            sessionId = null;
        }

        public dynamic GetCams()
        {
            if (sessionId == null)
                throw new AuthenticationException("Login first");

            try
            {
                HttpRequest httpRequest = new HttpRequest();
                httpRequest.GetParameters.Add("_sid", sessionId);
                httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Camera");
                httpRequest.GetParameters.Add("version", "3");
                httpRequest.GetParameters.Add("method", "List");
                httpRequest.GetParameters.Add("uri", url);
                httpRequest.GetParameters.Add("limit", "4");

                string jsonResponse = httpRequest.Get(url + "SurveillanceStation/camera.cgi");
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                Dictionary<string, dynamic> cameraListDictionary =
                    javaScriptSerializer.Deserialize<Dictionary<string, dynamic>>(jsonResponse);

                return cameraListDictionary["data"]["cameras"];
            }
            catch (Exception)
            {
                sessionId = null;
                throw;
            }
        }

        public string GetCamImageUrl(string camId)
        {
            if(sessionId == null)
                throw new AuthenticationException("Login first");

            try
            {
                HttpRequest httpRequest = new HttpRequest();
                httpRequest.GetParameters.Add("_sid", sessionId);
                httpRequest.GetParameters.Add("api", "SYNO.SurveillanceStation.Camera");
                httpRequest.GetParameters.Add("version", "2");
                httpRequest.GetParameters.Add("cameraId", camId);
                httpRequest.GetParameters.Add("method", "GetSnapshot");
                httpRequest.GetParameters.Add("uri", url);

                return httpRequest.GetUrl(url + "SurveillanceStation/camera.cgi");
            }
            catch (Exception)
            {
                sessionId = null;
                throw;
            }
        }
    }
}
