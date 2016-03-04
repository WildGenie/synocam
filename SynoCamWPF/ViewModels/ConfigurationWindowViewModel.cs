using System.ComponentModel;
using System.Runtime.CompilerServices;
using SynoCamWPF.Annotations;
using SynoCamWPF.Services;

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
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
