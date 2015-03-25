using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SynoCamLib
{
    public class HttpRequest
    {
        public HttpParameters GetParameters;
        public HttpParameters PostParameters;

        public HttpRequest()
        {
            GetParameters = new HttpParameters();
            PostParameters = new HttpParameters();
        }

        public async Task<string> GetASync(string url)
        {
            string getUrl = url + "?" + GetParameters.UrlEncode();
            var request = (HttpWebRequest) WebRequest.Create(getUrl);
            request.Method = "GET";

            //Send Web-Request and receive a Web-Response
            var response = (HttpWebResponse) await request.GetResponseAsync();

            //Translate data from the Web-Response to a string
            Stream dataStream = response.GetResponseStream();
            var streamreader = new StreamReader(dataStream, Encoding.UTF8);
            string html = streamreader.ReadToEnd();
            streamreader.Close();

            // clear request parameters
            GetParameters.Clear();
            PostParameters.Clear();

            return html;
        }

        public string GetUrl(string url)
        {
            string getUrl = url + "?" + GetParameters.UrlEncode();
            var request = (HttpWebRequest) WebRequest.Create(getUrl);
            request.Method = "GET";
            return request.RequestUri.ToString();
        }

        public string Post(string url)
        {
            string getUrl = url + "?" + GetParameters.UrlEncode();
            var request = (HttpWebRequest) WebRequest.Create(getUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            //Attach data to the Web-Request
            byte[] postData = PostParameters.ByteEncode();
            request.ContentLength = postData.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postData, 0, postData.Length);
            dataStream.Close();

            //Send Web-Request and receive a Web-Response
            var response = (HttpWebResponse) request.GetResponse();

            //Translate data from the Web-Response to a string
            dataStream = response.GetResponseStream();
            var streamreader = new StreamReader(dataStream, Encoding.UTF8);
            string html = streamreader.ReadToEnd();
            streamreader.Close();

            // clear request parameters
            GetParameters.Clear();
            PostParameters.Clear();

            return html;
        }

        public class HttpParameters : NameValueCollection
        {
            public string UrlEncode()
            {
                string result = "";
                foreach (string key in Keys)
                {
                    result += key + "=" + WebUtility.UrlEncode(this[key]) + "&";
                }

                // remove trailing "&"
                return result.TrimEnd(new[] {'&'});
            }

            public byte[] ByteEncode()
            {
                return Encoding.ASCII.GetBytes(UrlEncode());
            }
        }
    }
}