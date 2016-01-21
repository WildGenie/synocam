using System;
using System.Linq;
using System.Net;
using System.Windows;
using SynoCamLib;
using SynoCamWPF.Properties;

namespace SynoCamWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SynoCommand SynoCommand { get; set; }
        private string Password { get; set; }
        private string Username { get; set; }
        private bool UseHttps { get; set; }
        private string Address { get; set; }
        private string Port { get; set; }

        CameraViewsContext context = new CameraViewsContext();

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

        public MainWindow()
        {
            InitializeComponent();

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

            if (!String.IsNullOrEmpty(Settings.Default.Port))
            {
                Port = ":" + Settings.Default.Port;
            }
            
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            SynoCommand = new SynoCommand(Url, Username, Password);
            this.DataContext = context;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCameras();
        }

        private async void LoadCameras()
        {
            try
            {
                var result = await SynoCommand.GetCamsASync();
                context.CameraViews = result.Select(c => new CamControl(c, RefreshRate.Ms4Minutes)).ToList();
                
                // LabelLoading.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
               // LabelLoading.Text = string.Format(@"Unable to load cameras, error: " + Environment.NewLine + "{0}", ex.Message);
                //LabelLoading.Foreground = Brushes.IndianRed;
            }
        }

        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CloseButton.Visibility = Visibility.Visible;
            RefreshButton.Visibility = Visibility.Visible;
            MinimizeButton.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CloseButton.Visibility = Visibility.Hidden;
            RefreshButton.Visibility = Visibility.Hidden;
            MinimizeButton.Visibility = Visibility.Hidden;
        }
    }
}
