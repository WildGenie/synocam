using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SynoCamLib;
using SynoCamWPF.Properties;
using SynoCamWPF.Utilities;

namespace SynoCamWPF.ViewModels
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private static SynoCommand SynoCommand { get; set; }
        private string Password { get; set; }
        private string Username { get; set; }
        private bool UseHttps { get; set; }
        private string Address { get; set; }
        private string Port { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        
        private ObservableCollection<CamControl> _cameraViews;
        private int _width;
        private int _height;
        private int _left;
        private int _top;
        private Visibility _windowControlClose;
        private Visibility _windowControlMinimize;
        private Visibility _windowControlRefresh;

        public ObservableCollection<CamControl> CameraViews
        {
            get{ return _cameraViews; }
            set{ _cameraViews = value; OnPropertyChanged(); }
        }
        public int Width
        {
            get { return _width; }
            set { _width = value; OnPropertyChanged(); }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; OnPropertyChanged(); }
        }

        public int Left
        {
            get { return _left; }
            set { _left = value; OnPropertyChanged(); }
        }

        public int Top
        {
            get { return _top; }
            set { _top = value; OnPropertyChanged(); }
        }

        public Visibility WindowControlClose
        {
            get { return _windowControlClose; }
            set { _windowControlClose = value; OnPropertyChanged(); }
        }

        public Visibility WindowControlMinimize
        {
            get { return _windowControlMinimize; }
            set { _windowControlMinimize = value; OnPropertyChanged(); }
        }

        public Visibility WindowControlRefresh
        {
            get { return _windowControlRefresh; }
            set { _windowControlRefresh = value; OnPropertyChanged(); }
        }

        public ICommand ShowWindowControls { get; set; }
        public ICommand HideWindowControls { get; set; }

        private string Url
        {
            get
            {
                string url;
                if (UseHttps)
                {
                    url = "https://" + Address + Port;
                }
                else
                {
                    url = "http://" + Address + Port;
                }
                url += "/webapi/";
                return url;
            }
        }

        public MainWindowViewModel()
        {
            WindowControlClose = Visibility.Hidden;
            WindowControlMinimize = Visibility.Hidden;
            WindowControlRefresh = Visibility.Hidden;
            ShowWindowControls = new CustomCommand(o => { WindowControlClose = Visibility.Visible; WindowControlMinimize = Visibility.Visible; WindowControlRefresh = Visibility.Visible; }, o => true);
            HideWindowControls = new CustomCommand(o => { WindowControlClose = Visibility.Hidden; WindowControlMinimize = Visibility.Hidden; WindowControlRefresh = Visibility.Hidden; }, o => true);

            LoadData();
        }

        private void LoadData()
        {
            if (Settings.Default.WindowTop > 0 ||
                Settings.Default.WindowLeft > 0 ||
                Settings.Default.WindowHeight > 0 ||
                Settings.Default.WindowWidth > 0)
            {
                Top = Settings.Default.WindowTop;
                Left = Settings.Default.WindowLeft;
                Height = Settings.Default.WindowHeight;
                Width = Settings.Default.WindowWidth;
            }

            Address = Settings.Default.ServerIpOrDns;
            UseHttps = Settings.Default.UseHttps;
            Username = Settings.Default.Username;
            Password = Settings.Default.Password;
            Port = "";

            if (!string.IsNullOrEmpty(Settings.Default.Port))
            {
                Port = ":" + Settings.Default.Port;
            }

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            SynoCommand = new SynoCommand(Url, Username, Password);

            LoadCameras();
        }

        private async void LoadCameras()
        {
            try
            {
                var result = await SynoCommand.GetCamsASync();
                CameraViews = new ObservableCollection<CamControl>(result.Select(c => new CamControl(c, RefreshRate.Ms4Minutes)).ToList());

                // LabelLoading.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                // LabelLoading.Text = string.Format(@"Unable to load cameras, error: " + Environment.NewLine + "{0}", ex.Message);
                //LabelLoading.Foreground = Brushes.IndianRed;
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
