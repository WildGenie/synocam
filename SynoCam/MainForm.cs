using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using SynoCam.Properties;
using SynoCamLib;

namespace SynoCam
{
    public partial class MainForm : Form
    {
        protected class NativeCalls
        {
            public const int WmNclbuttondown = 0xA1;
            public const int HtCaption = 0x2;

            [DllImportAttribute("user32.dll")]
            public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

            [DllImport("user32.dll")]
            public static extern bool ReleaseCapture();
        }

        private string Address { get; set; }
        private bool UseHttps { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string Port { get; set; }
        
        private readonly SynoCommand _synoCommand;
        List<Cam> _cams = new List<Cam>();

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

        public MainForm()
        {
            InitializeComponent();
                
            Address = Settings.Default.ServerIpOrDns;
            UseHttps = Settings.Default.UseHttps;
            Username = Settings.Default.Username;
            Password = Settings.Default.Password;
            Port = "";

            Text = Address;

            if (!String.IsNullOrEmpty(Settings.Default.Port))
            {
                Port = ":" + Settings.Default.Port;
            }

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            _synoCommand = new SynoCommand(Url, Username, Password);

            labelLoading.MouseDown += MainFormMouseDown;
            labelLoading.DoubleClick += MainFormDoubleClick;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ResizeCameras();
        }

        private void ResizeCameras()
        {
            if (labelLoading.Visible)
            {
                labelLoading.Top = 2;
                labelLoading.Left = 2;
                labelLoading.Width = Width - panelControl.Width - 4;
                labelLoading.Height = Height - 4;
            }
                

            int formWidth = Width - 4 - panelControl.Width;
            int width = 2;
            foreach (var cam in _cams)
            {
                cam.Left = width;
                cam.Top = 2;
                cam.Height = Height - 4;
                cam.Width = formWidth / _cams.Count;
                width += cam.Width;
            }
        }

        protected override void WndProc(ref Message m)
        {
            const UInt32 wmNchittest = 0x0084;
            const UInt32 wmMousemove = 0x0200;

            const UInt32 htleft = 10;
            const UInt32 htright = 11;
            const UInt32 htbottomright = 17;
            const UInt32 htbottom = 15;
            const UInt32 htbottomleft = 16;
            const UInt32 httop = 12;
            const UInt32 httopleft = 13;
            const UInt32 httopright = 14;

            const int resizeHandleSize = 10;
            bool handled = false;
            if (m.Msg == wmNchittest || m.Msg == wmMousemove)
            {
                Size formSize = Size;
                var screenPoint = new Point(m.LParam.ToInt32());
                var clientPoint = PointToClient(screenPoint);

                var boxes = new Dictionary<UInt32, Rectangle>
                {
                    {htbottomleft, new Rectangle(0, formSize.Height - resizeHandleSize, resizeHandleSize, resizeHandleSize)},
                    {htbottom, new Rectangle(resizeHandleSize, formSize.Height - resizeHandleSize, formSize.Width - 2*resizeHandleSize, resizeHandleSize)},
                    {htbottomright, new Rectangle(formSize.Width - resizeHandleSize, formSize.Height - resizeHandleSize, resizeHandleSize, resizeHandleSize)},
                    {htright, new Rectangle(formSize.Width - resizeHandleSize, resizeHandleSize, resizeHandleSize, formSize.Height - 2*resizeHandleSize)},
                    {httopright, new Rectangle(formSize.Width - resizeHandleSize, 0, resizeHandleSize, resizeHandleSize) },
                    {httop, new Rectangle(resizeHandleSize, 0, formSize.Width - 2*resizeHandleSize, resizeHandleSize) },
                    {httopleft, new Rectangle(0, 0, resizeHandleSize, resizeHandleSize) },
                    {htleft, new Rectangle(0, resizeHandleSize, resizeHandleSize, formSize.Height - 2*resizeHandleSize) }
                };

                foreach (KeyValuePair<UInt32, Rectangle> hitBox in boxes)
                {
                    if (hitBox.Value.Contains(clientPoint))
                    {
                        m.Result = (IntPtr)hitBox.Key;
                        handled = true;
                        break;
                    }
                }
            }

            if (!handled)
                base.WndProc(ref m);
        }

        public override sealed string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        private void Form1_Load(object sender, EventArgs e)
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

            Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdatePictureBoxes().Wait();
                }
                catch (Exception ex)
                {
                    labelLoading.Invoke(new Action(() =>
                    {
                        string text = string.Format(@"Unable to load cameras, error: " + Environment.NewLine + "{0}", ex.InnerException.Message);
                        labelLoading.Text = text;
                        labelLoading.ForeColor = Color.IndianRed;
                    }));
                }
            });
        }

        private async Task<bool> UpdatePictureBoxes()
        {
            bool result = false;

            _cams = await _synoCommand.GetCamsASync();
            foreach (var cam in _cams)
            {
                Cam cam1 = cam;
                cam1.MouseDown += MainFormMouseDown;
                cam1.DoubleClick += MainFormDoubleClick;
                cam1.LoadCompleted += cam_LoadCompleted;
                cam1.Visible = false;
                
                Invoke(new Action(() => Controls.Add(cam1)));
                result = true;
            }

            return result;
        }

        void cam_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            Invoke((Action)delegate 
            { 
                Refresh();
                labelLoading.Visible = false;
                foreach (var cam in _cams)
                {
                    cam.Visible = true;
                }
            });
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ResizeCameras();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            _synoCommand.LogoutASync();
        }

        private void CloseButtonClick(object sender, EventArgs e)
        {
            Settings.Default.WindowTop = Top;
            Settings.Default.WindowLeft = Left;
            Settings.Default.WindowHeight = Height;
            Settings.Default.WindowWidth = Width;
            Settings.Default.Save();
            Close();
        }

        private void MinimizeButtonClick(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void MainFormMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && e.Clicks == 1)
            {
                NativeCalls.ReleaseCapture();
                NativeCalls.SendMessage(Handle, NativeCalls.WmNclbuttondown, NativeCalls.HtCaption, 0);
            }
        }

        private void MainFormDoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Normal)
            {
                WindowState = FormWindowState.Maximized;
                panelControl.BackColor = Color.Black;
            }
            else
            {
                WindowState = FormWindowState.Normal;
                panelControl.BackColor = Color.FromArgb(255, 128, 255);
            }
        }

        private void RefreshButtonClick(object sender, EventArgs e)
        {
            foreach (var cam in _cams)
            {
                cam.GetPictureNow();
            }
        }

        private void ChangeRefreshRateForAllCameras(int seconds)
        {
            foreach (var cam in _cams)
            {
                cam.RefreshInMiliseconds = seconds;
            }
        }

        private void FourMinutesToolStripMenuItemClick(object sender, EventArgs e)
        {
            ChangeRefreshRateForAllCameras(240000);
        }

        private void TwoMinutesToolStripMenuItemClick(object sender, EventArgs e)
        {
            ChangeRefreshRateForAllCameras(120000);
        }

        private void OneMinuteToolStripMenuItemClick(object sender, EventArgs e)
        {
            ChangeRefreshRateForAllCameras(60000);
        }

        private void ThirtySecondsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ChangeRefreshRateForAllCameras(30000);
        }

        private void TwoSecondsToolStripMenuItemClick(object sender, EventArgs e)
        {
            ChangeRefreshRateForAllCameras(2000);
        }
    }
}
