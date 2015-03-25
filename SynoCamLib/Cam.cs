using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace SynoCamLib
{
    public class Cam : PictureBox
    {
        public string CamName { get; private set; }
        public CamStatus Status { get; private set; }
        public new bool Enabled { get; private set; }
        public string Url { get; private set; }

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

        public Cam(string camName, CamStatus status, bool enabled, string url)
        {
            Url = url;
            Enabled = enabled;
            Status = status;
            CamName = camName;

            SizeMode = PictureBoxSizeMode.Zoom;
            Click += OnClick;

            BackColor = Color.Black;

            _timer = new System.Timers.Timer(1);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void OnClick(object sender, EventArgs eventArgs)
        {
            GetPictureNow();
        }

        public void GetPictureNow()
        {
            LoadAsync(Url);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if ((int)_timer.Interval == 1)
                RefreshInMiliseconds = 240000; // 4 minutes

            LoadAsync(Url);
        }
    }
}
