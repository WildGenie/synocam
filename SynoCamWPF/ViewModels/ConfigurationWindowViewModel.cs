using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SynoCamWPF.Annotations;
using SynoCamWPF.Services;
using SynoCamWPF.Utilities;

namespace SynoCamWPF.ViewModels
{
    public class ConfigurationWindowViewModel : INotifyPropertyChanged
    {
        private string _password;
        private string _username;
        private bool _useHttps;
        private string _address;
        private string _port;

        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public bool UseHttps
        {
            get { return _useHttps; }
            set { _useHttps = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get { return _address; }
            set { _address = value; OnPropertyChanged(); }
        }

        public string Port
        {
            get { return _port; }
            set { _port = value; OnPropertyChanged(); }
        }

        public ConfigurationWindowViewModel()
        {
            Address = ConfigurationService.Instance.Address;
            UseHttps = ConfigurationService.Instance.UseHttps;
            Username = ConfigurationService.Instance.Username;
            Password = ConfigurationService.Instance.Password;
            Port = ConfigurationService.Instance.Port;

            OpenConfigurationFile = new CustomCommand(OpenConfigFile, o => true);
        }

        private void OpenConfigFile(object o)
        {
            var directoryInfo = new FileInfo(ConfigurationService.Instance.GetConfigurationFilePath()).Directory;
            if (directoryInfo == null)
                return;

            var directory = directoryInfo.FullName;
            Process.Start(directory);
        }

        public ICommand OpenConfigurationFile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
