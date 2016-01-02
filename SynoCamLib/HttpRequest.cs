using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SynoCamLib
{
    internal class HttpRequest
    {
        internal HttpParameters Parameters;
        internal Dictionary<string, string> Headers;

        internal HttpRequest()
        {
            Parameters = new HttpParameters();
            Headers = new Dictionary<string, string>();
        }

        internal void AddHeaders(HttpWebRequest requestToAddUpon)
        {
            foreach (var headerPair in Headers)
            {
                if (!WebHeaderCollection.IsRestricted(headerPair.Key)) // Test if the header is restricted, if not we can just add it
                {
                    requestToAddUpon.Headers.Add(headerPair.Key, headerPair.Value);
                }
                else
                {
                    switch (headerPair.Key)
                    {
                        case "Range":
                            if (headerPair.Value.Contains("-")) // This part is a bit of a gamble..
                            {
                                AddRange(requestToAddUpon, headerPair);
                            }
                            else
                            {
                                int range = Convert.ToInt32(headerPair.Value);
                                requestToAddUpon.AddRange(range);
                            }
                            break;
                        case "User-Agent":
                            requestToAddUpon.UserAgent = headerPair.Value;
                            break;
                        default:
                            throw new ApplicationException("Restricted header was found, but no implementation was made in lib to handle it.");
                    }
                }
            }
        }

        private static void AddRange(HttpWebRequest requestToAddUpon, KeyValuePair<string, string> headerPair)
        {
            var splitted = headerPair.Value.Split('-'); // Split from and to
            string part1 = splitted[0];

            string specifier = string.Empty;
            if (part1.Contains("=")) // range does sometimes have a specification part
            {
                var splitted2 = splitted[0].Split('=');
                specifier = splitted2[0];
                part1 = splitted2[1];
            }

            int fromPoint = Convert.ToInt32(part1);
            int toPoint = Convert.ToInt32(splitted[1]);

            if (string.IsNullOrEmpty(specifier))
            {
                requestToAddUpon.AddRange(fromPoint, toPoint);
            }
            else
            {
                requestToAddUpon.AddRange(specifier, fromPoint, toPoint);
            }
        }

        internal async Task<string> GetASync(string url, int timeoutMs = 8000)
        {
            string getUrl = url + "?" + Parameters.UrlEncode();
            var request = (HttpWebRequest) WebRequest.Create(getUrl);

            request.Method = "GET";

            AddHeaders(request);

            HttpWebResponse response;

            try
            {
                response = await request.GetResponseAsync(timeoutMs) as HttpWebResponse;
            }
            catch (TimeoutException)
            {
                return string.Empty;
            }
            
            if (response == null)
                throw new ApplicationException("No data received");

            Stream dataStream = response.GetResponseStream();

            if (dataStream == null)
                throw new ApplicationException("Data received was invalid");

            var streamreader = new StreamReader(dataStream, Encoding.UTF8);
            string html = streamreader.ReadToEnd();
            streamreader.Close();

            Parameters.Clear();
            Headers.Clear();

            return html;
        }

        internal string GetUrl(string url)
        {
            string getUrl = url + "?" + Parameters.UrlEncode();
            var request = (HttpWebRequest) WebRequest.Create(getUrl);
            request.Method = "GET";
            return request.RequestUri.ToString();
        }

        internal string Post(string url)
        {
            string getUrl = url + "?" + Parameters.UrlEncode();
            var request = (HttpWebRequest) WebRequest.Create(getUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            AddHeaders(request);

            byte[] postData = Parameters.ByteEncode();
            request.ContentLength = postData.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(postData, 0, postData.Length);
            dataStream.Close();
            
            var response = (HttpWebResponse) request.GetResponse();
            dataStream = response.GetResponseStream();

            if (dataStream == null)
                throw new ApplicationException("Data received was invalid");

            var streamreader = new StreamReader(dataStream, Encoding.UTF8);
            string html = streamreader.ReadToEnd();

            streamreader.Close();
            Parameters.Clear();
            Headers.Clear();

            return html;
        }

        internal class HttpParameters : NameValueCollection
        {
            public string UrlEncode()
            {
                string result = "";
                foreach (string key in Keys)
                {
                    result += key + "=" + WebUtility.UrlEncode(this[key]) + "&";
                }

                return result.TrimEnd(new[] {'&'});
            }

            public byte[] ByteEncode()
            {
                return Encoding.ASCII.GetBytes(UrlEncode());
            }
        }
    }

    internal static class WebRequestExtensions
    {
        internal static Task<WebResponse> GetResponseAsync(this WebRequest request, int timeout)
        {
            return Task.Factory.StartNew(() =>
            {
                var t = Task.Factory.FromAsync<WebResponse>(
                    request.BeginGetResponse,
                    request.EndGetResponse,
                    null);

                if (!t.Wait(timeout)) throw new TimeoutException();

                return t.Result;
            });
        }
    }
}