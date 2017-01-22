using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using SynoCamWPF.Annotations;
using SynoCamWPF.Services;
using SynoCamWPF.Utilities;
using System.Windows.Controls;

namespace SynoCamWPF.ViewModels
{
    public class ConfigurationWindowViewModel : INotifyPropertyChanged
    {
        private string _username;
        private Uri _address;

        public string Username
        {
            get { return _username; }
            set { _username = value; OnPropertyChanged(); }
        }

        public string Address
        {
            get { return _address.AbsoluteUri; }
            set { _address = new Uri(value, UriKind.Absolute); OnPropertyChanged(); }
        }

        public ConfigurationWindowViewModel()
        {
            _address = ConfigurationService.Instance.Address; OnPropertyChanged("Address");
            Username = ConfigurationService.Instance.Username;

            SaveConfigFile = new CustomCommand((password) => 
            {
                ConfigurationService.Instance.Address = _address;
                ConfigurationService.Instance.Username = _username;
                ConfigurationService.Instance.Password = (password as PasswordBox).Password;
                ConfigurationService.Instance.SaveSettings();
            }, (o) => true);
        }

        //private void OpenConfigFile(object o)
        //{
        //    var directoryInfo = new FileInfo(ConfigurationService.Instance.GetConfigurationFilePath()).Directory;
        //    if (directoryInfo == null)
        //        return;

        //    var directory = directoryInfo.FullName;
        //    Process.Start(directory);
        //}


        public ICommand SaveConfigFile { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
