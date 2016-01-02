using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using SynoCamLib;

namespace SynoCam
{
    public partial class DownloadProgress : Form
    {
        private string _filePath;

        public DownloadProgress()
        {
            InitializeComponent();
        }

        public void ShowDialog(SynoCommand synoCommand, CamEvent camEvent)
        {
            _filePath = synoCommand.DownloadEvent(camEvent, FileDownloadCompleted, ProgressChanged);
            ShowDialog();
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            progressBar1.Invoke(new Action(() =>
                {
                    progressBar1.Value = downloadProgressChangedEventArgs.ProgressPercentage;
                }));
        }

        private void FileDownloadCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            Process.Start(_filePath);
            this.Close();
        }
    }
}
