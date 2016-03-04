using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using SynoCamLib;
using SynoCamWPF.Services;
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

        public RefreshRate RefreshRate { get; set; }

        public MainWindowViewModel()
        {
            RefreshRate = RefreshRate.Ms30Seconds;
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

        private void LoadData()
        {
            WindowsTop = ConfigurationService.Instance.WindowsTop;
            WindowLeft = ConfigurationService.Instance.WindowLeft;
            WindowHeight = ConfigurationService.Instance.WindowHeight;
            WindowWidth = ConfigurationService.Instance.WindowWidth;
            Address = ConfigurationService.Instance.Address;
            UseHttps = ConfigurationService.Instance.UseHttps;
            Username = ConfigurationService.Instance.Username;
            Password = ConfigurationService.Instance.Password;
            RefreshRate = ConfigurationService.Instance.RefreshRate;
            Port = ConfigurationService.Instance.Port;

            // Decide if we want the demo mode or live
            SynoCommand = Address == "server.domain.com" ? new SynoCommand() : new SynoCommand(Url, Username, Password);

            LoadCameras();
        }

        private void OpenConfigWindow(object o)
        {
            ConfigurationWindow configWindow = new ConfigurationWindow();
            configWindow.ShowDialog();
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
            ConfigurationService.Instance.SaveSettings();
            Application.Current.Shutdown(0);
        }

        private async void LoadCameras()
        {
            try
            {
                var result = await SynoCommand.GetCamsASync();
                CameraViews = new ObservableCollection<CamControl>(result.Select(c => new CamControl(c, RefreshRate)).ToList());
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
