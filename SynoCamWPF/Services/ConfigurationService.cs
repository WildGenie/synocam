using System.Configuration;
using SynoCamWPF.Properties;
using System;

namespace SynoCamWPF.Services
{
    internal class ConfigurationService
    {
        private static ConfigurationService _instance;

        public int WindowsTop { get; set; }
        public int WindowLeft { get; set; }
        public int WindowHeight { get; set; }
        public int WindowWidth { get; set; }
        public RefreshRate RefreshRate { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public Uri Address { get; set; }

        private ConfigurationService()
        {
            LoadData();
        }
        internal static ConfigurationService Instance => _instance ?? (_instance = new ConfigurationService());

        internal void SaveSettings()
        {
            Settings.Default.WindowTop = WindowsTop;
            Settings.Default.WindowLeft = WindowLeft;
            Settings.Default.WindowHeight = WindowHeight;
            Settings.Default.WindowWidth = WindowWidth;
            Settings.Default.RefreshRate = (int)RefreshRate;
            Settings.Default.Address = Address;
            Settings.Default.Username = Username;
            Settings.Default.Password = Password;
            Settings.Default.Save();
        }

        internal string GetConfigurationFilePath()
        {
            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal).FilePath;
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

            Address = Settings.Default.Address;
            Username = Settings.Default.Username;
            Password = Settings.Default.Password;
            RefreshRate = (RefreshRate)Settings.Default.RefreshRate;
        }
    }
}
