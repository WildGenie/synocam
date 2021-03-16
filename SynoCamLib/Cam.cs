using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace SynoCamLib
{
    internal class Cam : ICam
    {
        public string CamName { get; }
        public CamStatus Status { get; }
        public bool Enabled { get; }
        public MemoryStream CamImageData { get; private set; }
        public void TryRefreshCamImage()
        {
            WebRequest request = WebRequest.Create(_url);

            Task.Factory.StartNew(() =>
            {
                try
                {
                    WebResponse response = request.GetResponse();
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                        {
                            CamImageData = new MemoryStream();

                            var newBitmap = Image.FromStream(responseStream);
                            newBitmap.Save(CamImageData, ImageFormat.Png);
                            CamImageData.Seek(0, SeekOrigin.Begin);
                        }

                        ImageChangedEvent?.Invoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    // Do nothing, we allow this to fail sometimes
                }
            });
        }

        public EventHandler ImageChangedEvent
        {
            get { return _imageChangedEvent; }
            set
            {
                _imageChangedEvent = value;
                if (CamImageData != null)
                    _imageChangedEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        private EventHandler _imageChangedEvent;
        private readonly Uri _url;


        public Cam(string name, CamStatus status, bool enabled, string url)
        {
            CamName = name;
            Status = status;
            Enabled = enabled;
            _url = new Uri(url);

            // Loading..
            CreateLoadingImage();

            // Try to get the real deal
            TryRefreshCamImage();
        }

        private void CreateLoadingImage()
        {
            var newBitmap = new Bitmap(640, 480);

            using (var graph = Graphics.FromImage(newBitmap))
            {
                graph.DrawString("Loading..", new Font(FontFamily.GenericSansSerif, 30), new SolidBrush(Color.White), 10, 10);
                graph.Flush();
            }

            CamImageData = new MemoryStream();
            newBitmap.Save(CamImageData, ImageFormat.Png);
            CamImageData.Seek(0, SeekOrigin.Begin);
        }
    }
}
