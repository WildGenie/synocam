using System;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;
using SynoCamLib;

namespace SynoCam
{
    public sealed class CamUi : PictureBox
    {
        private bool _showRedDot;
        private readonly ICam _camera;

        private int _refreshInMiliseconds;
        public int RefreshInMiliseconds
        {
            get { return _refreshInMiliseconds; }
            set
            {
                _refreshInMiliseconds = value;
                _timer.Stop();
                _timer.Interval = _refreshInMiliseconds;
                _timer.Start();
            }
        }

        private readonly System.Timers.Timer _timer;
        public EventHandler ImageLoaded { get; set; }

        public CamUi(ICam cam, int refreshRateInMs)
        {
            _camera = cam;
            _refreshInMiliseconds = refreshRateInMs;

            SizeMode = PictureBoxSizeMode.Zoom;
            Click += OnClick;

            BackColor = Color.Black;

            _timer = new System.Timers.Timer(1);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();

            ImageLoaded += OnLoadCompleted;
            Paint += OnPaint;

            _camera.ImageChangedEvent += ImageChangedEvent;
        }

        private void ImageChangedEvent(object sender, EventArgs eventArgs)
        {
            Image = Image.FromStream(_camera.CamImageData);
            
            if (ImageLoaded != null)
                ImageLoaded.Invoke(this, EventArgs.Empty);
        }

        private void OnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            if (!_showRedDot)
                return;

            var rect = new Rectangle(2, 2, 2, 2);
            var pen = new Pen(Color.Crimson, 4);
            paintEventArgs.Graphics.DrawRectangle(pen, rect);
        }

        private void OnLoadCompleted(object sender, EventArgs e)
        {
            _showRedDot = !_showRedDot;
        }

        private void OnClick(object sender, EventArgs eventArgs)
        {
            GetPictureNow();
        }

        public void GetPictureNow()
        {
            _camera.TryRefreshCamImage();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if ((int)_timer.Interval == 1) // The first refresh of camera images should be fast, after that the normale refresh rate should be used
                RefreshInMiliseconds = _refreshInMiliseconds;

            GetPictureNow();
        }
    }
}
