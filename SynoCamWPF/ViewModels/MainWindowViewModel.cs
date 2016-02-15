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
        private int _windowWidth;
        private int _windowHeight;
        private int _windowLeft;
        private int _windowsTop;
        private Visibility _windowControlClose;
        private Visibility _windowControlMinimize;
        private Visibility _windowControlRefresh;

        public ObservableCollection<CamControl> CameraViews
        {
            get{ return _cameraViews; }
            set{ _cameraViews = value; OnPropertyChanged(); }
        }

        public int NumberOfCameras => CameraViews.Count;

        public int WindowWidth
        {
            get { return _windowWidth; }
            set { _windowWidth = value; OnPropertyChanged(); }
        }

        public int WindowHeight
        {
            get { return _windowHeight; }
            set { _windowHeight = value; OnPropertyChanged(); }
        }

        public int WindowLeft
        {
            get { return _windowLeft; }
            set { _windowLeft = value; OnPropertyChanged(); }
        }

        public int WindowsTop
        {
            get { return _windowsTop; }
            set { _windowsTop = value; OnPropertyChanged(); }
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

        public ICommand WindowClosePressed { get; set; }
        public ICommand WindowRefreshPressed { get; set; }

        public ICommand OpenConfigDialog { get; set; }

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

            OpenConfigDialog = new CustomCommand(OpenConfigWindow, o => true);

            WindowClosePressed = new CustomCommand(CloseWindow, o => true);
            WindowRefreshPressed = new CustomCommand(RefreshCamImagesNow, o => true);

            LoadData();
        }

        private void OpenConfigWindow(object o)
        {

        }

        private void RefreshCamImagesNow(object o)
        {
            foreach (var camView in CameraViews)
            {
                camView.TryRefreshCamImage();
            }
        }

        private void CloseWindow(object o)
        {
            SaveSettings();
            Application.Current.Shutdown(0);
        }

        private void SaveSettings()
        {
            Settings.Default.WindowTop = WindowsTop;
            Settings.Default.WindowLeft = WindowLeft;
            Settings.Default.WindowHeight = WindowHeight;
            Settings.Default.WindowWidth = WindowWidth;
            Settings.Default.Save();
        }

        private void LoadData()
        {
            if (Settings.Default.WindowTop > 0 ||
                Settings.Default.WindowLeft > 0 ||
                Settings.Default.WindowHeight > 0 ||
                Settings.Default.WindowWidth > 0)
            {
                WindowsTop = Settings.Default.WindowTop;
                WindowLeft = Settings.Default.WindowLeft;
                WindowHeight = Settings.Default.WindowHeight;
                WindowWidth = Settings.Default.WindowWidth;
            }
            else
            {
                // Default settings
                WindowsTop = 200;
                WindowLeft = 200;
                WindowHeight = 150;
                WindowWidth = 300;
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

            // Decide if we want the demo mode or live
            SynoCommand = Address == "server.domain.com" ? new SynoCommand() : new SynoCommand(Url, Username, Password);
            
            LoadCameras();
        }

        private async void LoadCameras()
        {
            try
            {
                var result = await SynoCommand.GetCamsASync();
                CameraViews = new ObservableCollection<CamControl>(result.Select(c => new CamControl(c, RefreshRate.Ms30Seconds)).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Unable to load cameras, error: " + Environment.NewLine + "{0}", ex.Message));
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
