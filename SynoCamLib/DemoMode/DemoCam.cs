using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SynoCamLib.DemoMode
{
    internal class DemoCam : ICam
    {
        private readonly int _camNumber;
        private int _imageCount = 1;

        public string CamName => "demoCamera";
        public CamStatus Status => CamStatus.Normal;
        public bool Enabled => true;
        public MemoryStream CamImageData { get; set; }

        public void TryRefreshCamImage()
        {
            CreateFakeImage();

            ImageChangedEvent?.Invoke(this, EventArgs.Empty);
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

        public DemoCam(int camNumber)
        {
            _camNumber = camNumber;
            _random = new Random(2 + _camNumber);
            CamImageData = new MemoryStream();
            CreateFakeImage();
        }

        private void CreateFakeImage()
        {
            var newBitmap = new Bitmap(640, 480);

            using (var graph = Graphics.FromImage(newBitmap))
            using (var brush = new SolidBrush(GetRandomColor()))
            {
                graph.FillRectangle(brush, new Rectangle(0, 0, newBitmap.Width, newBitmap.Height));
                graph.DrawString(_camNumber + " > " + _imageCount, new Font(FontFamily.GenericSansSerif, 40), new SolidBrush(Color.Black), 10,10);
                graph.Flush();
            }

            CamImageData = new MemoryStream();
            newBitmap.Save(CamImageData, ImageFormat.Png);
            CamImageData.Seek(0, SeekOrigin.Begin);

            _imageCount++;
        }

        private readonly Random _random;

        public Color GetRandomColor()
        {
            // to create lighter colours:
            // take a random integer between 0 & 128 (rather than between 0 and 255)
            // and then add 127 to make the colour lighter
            byte[] colorBytes = new byte[3];
            colorBytes[0] = (byte)(_random.Next(128) + 127);
            colorBytes[1] = (byte)(_random.Next(128) + 127);
            colorBytes[2] = (byte)(_random.Next(128) + 127);

            return Color.FromArgb(255, colorBytes[0], colorBytes[1], colorBytes[2]); ;
        }
    }
}
