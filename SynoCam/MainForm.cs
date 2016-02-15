using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        List<CamUi> _cams = new List<CamUi>();

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
            _synoCommand = Address == "server.domain.com" ? new SynoCommand() : new SynoCommand(Url, Username, Password);

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
                AdjustLoadingLabel();
            }

            AdjustCams();
        }

        private void AdjustCams()
        {
            int formWidth = Width - 4 - panelControl.Width;
            int width = 2;
            foreach (var cam in _cams)
            {
                cam.Left = width;
                cam.Top = 2;
                cam.Height = Height - 4;
                cam.Width = formWidth/_cams.Count;
                width += cam.Width;
            }
        }

        private void AdjustLoadingLabel()
        {
            labelLoading.Top = 2;
            labelLoading.Left = 2;
            labelLoading.Width = Width - panelControl.Width - 4;
            labelLoading.Height = Height - 4;
        }

        protected override void WndProc(ref Message m)
        {
            const int wmNchittest = 0x0084;
            const int wmMousemove = 0x0200;
            const int htleft = 10;
            const int htright = 11;
            const int htbottomright = 17;
            const int htbottom = 15;
            const int htbottomleft = 16;
            const int httop = 12;
            const int httopleft = 13;
            const int httopright = 14;
            const int resizeHandleSize = 10;

            bool handled = false;
            if (m.Msg == wmNchittest || m.Msg == wmMousemove)
            {
                Size formSize = Size;
                var screenPoint = new Point(m.LParam.ToInt32());
                var clientPoint = PointToClient(screenPoint);

                var boxes = new Dictionary<int, Rectangle>
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

                foreach (KeyValuePair<int, Rectangle> hitBox in boxes)
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

            CheckRefreshRateToolStripMenuItems(Ms4Minutes);
        }

        private async Task<bool> UpdatePictureBoxes()
        {
            bool result = false;

            List<ICam> results = await _synoCommand.GetCamsASync();
            _cams = results.Select(c => new CamUi(c, Ms4Minutes)).ToList();

            foreach (var cam in _cams)
            {
                CamUi newCam = cam;
                newCam.MouseDown += MainFormMouseDown;
                newCam.DoubleClick += MainFormDoubleClick;
                newCam.ImageLoaded += cam_LoadCompleted;
                newCam.Visible = false;
                
                Invoke(new Action(() => Controls.Add(newCam)));
                result = true;
            }

            return result;
        }

        void cam_LoadCompleted(object sender, EventArgs e)
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
           var disconnectWait = new Disconnect(_synoCommand);
            disconnectWait.ShowDialog(this);
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

        private void CheckRefreshRateToolStripMenuItems(int seconds)
        {
            twoSecondsRefreshToolStripMenuItem.Checked = seconds == Ms2Seconds;
            thirtySecondsRefreshToolStripMenuItem.Checked = seconds == Ms30Seconds;
            oneMinuteRefreshToolStripMenuItem.Checked = seconds == Ms1Minute;
            twoMinutesRefreshToolStripMenuItem.Checked = seconds == Ms2Minutes;
            fourMinutesRefreshToolStripMenuItem.Checked = seconds == Ms4Minutes;
        }

        private void ChangeRefreshRateForAllCameras(int seconds)
        {
            foreach (var cam in _cams)
            {
                cam.RefreshInMiliseconds = seconds;
            }
        }

        private const int Ms4Minutes = 240000;
        private const int Ms2Minutes = 120000;
        private const int Ms1Minute = 60000;
        private const int Ms30Seconds = 30000;
        private const int Ms2Seconds = 2000;

        private void FourMinutesToolStripMenuItemClick(object sender, EventArgs e)
        {
            CheckRefreshRateToolStripMenuItems(Ms4Minutes);
            ChangeRefreshRateForAllCameras(Ms4Minutes);
        }

        private void TwoMinutesToolStripMenuItemClick(object sender, EventArgs e)
        {
            CheckRefreshRateToolStripMenuItems(Ms2Minutes);
            ChangeRefreshRateForAllCameras(Ms2Minutes);
        }

        private void OneMinuteToolStripMenuItemClick(object sender, EventArgs e)
        {
            CheckRefreshRateToolStripMenuItems(Ms1Minute);
            ChangeRefreshRateForAllCameras(Ms1Minute);
        }

        private void ThirtySecondsToolStripMenuItemClick(object sender, EventArgs e)
        {
            CheckRefreshRateToolStripMenuItems(Ms30Seconds);
            ChangeRefreshRateForAllCameras(Ms30Seconds);
        }

        private void TwoSecondsToolStripMenuItemClick(object sender, EventArgs e)
        {
            CheckRefreshRateToolStripMenuItems(Ms2Seconds);
            ChangeRefreshRateForAllCameras(Ms2Seconds);
        }

        private void viewEventsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var eventviewer = new EventViewer();
            eventviewer.ShowDialog(_synoCommand);
        }
    }
}
