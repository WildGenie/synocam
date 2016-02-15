using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Windows.Media.Imaging;
using SynoCamLib;
using SynoCamWPF.Annotations;

namespace SynoCamWPF
{
    public class CamControl : INotifyPropertyChanged
    {
        private readonly ICam _cam;
        private RefreshRate _refreshRate;
        public string CamName => _cam.CamName;
        public CamStatus Status => _cam.Status;
        public bool Enabled => _cam.Enabled;
        public BitmapImage CamImage { get; private set; }


        private Timer RefreshTimer { get; }

        private BitmapImage GetImageFromStream()
        {
            if (_cam.CamImageData == null || _cam.CamImageData.Length == 0)
                return new BitmapImage();
                
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = _cam.CamImageData;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }

        public RefreshRate RefreshRate
        {
            get { return _refreshRate; }
            set
            {
                _refreshRate = value;
                if (RefreshTimer != null)
                {
                    RefreshTimer.Stop();
                    RefreshTimer.Interval = Convert.ToDouble(_refreshRate);
                    RefreshTimer.Start();
                }
            }
        }

        public CamControl(ICam cam, RefreshRate refreshRate)
        {
            _cam = cam;
            _cam.ImageChangedEvent += UnderlyingImageChanged;

            RefreshRate = refreshRate;
            RefreshTimer = new Timer(Convert.ToDouble(refreshRate));
            RefreshTimer.Elapsed += RefreshTimerOnElapsed;
            RefreshTimer.Start();
        }

        private void UnderlyingImageChanged(object sender, EventArgs eventArgs)
        {
            CamImage = GetImageFromStream();
            OnPropertyChanged(nameof(CamImage));
        }

        public void TryRefreshCamImage()
        {
            _cam.TryRefreshCamImage();
        }

        private void RefreshTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            TryRefreshCamImage();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
