﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace SynoCamLib
{
    public sealed class CamUi : PictureBox
    {
        private bool _showRedDot;

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

        public CamUi(string camName, CamStatus status, bool enabled, string url, int defaultRefreshRateInMs = 240000)
        {
            Url = url;
            Enabled = enabled;
            Status = status;
            CamName = camName;
            _refreshInMiliseconds = defaultRefreshRateInMs;

            SizeMode = PictureBoxSizeMode.Zoom;
            Click += OnClick;

            BackColor = Color.Black;

            _timer = new System.Timers.Timer(1);
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();

            LoadCompleted += OnLoadCompleted;
            Paint += OnPaint;
        }

        private void OnPaint(object sender, PaintEventArgs paintEventArgs)
        {
            if (!_showRedDot)
                return;

            var rect = new Rectangle(2, 2, 2, 2);
            var pen = new Pen(Color.Crimson, 4);
            paintEventArgs.Graphics.DrawRectangle(pen, rect);
        }

        private void OnLoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            _showRedDot = !_showRedDot;
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
            if ((int)_timer.Interval == 1) // The first refresh of camera images should be fast, after that the normale refresh rate should be used
                RefreshInMiliseconds = _refreshInMiliseconds;

            LoadAsync(Url);
        }
    }
}