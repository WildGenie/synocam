using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net;
using SynoCam.Properties;

//http://ukdl.synology.com/download/Document/DeveloperGuide/Surveillance_Station_Web_API.pdf


namespace SynoCam
{
    public partial class Form1 : Form
    {
        private string Address { get; set; }
        private bool UseHttps { get; set; }
        private string Username { get; set; }
        private string Password { get; set; }
        private string Port { get; set; }
        
        private readonly SynoCommand synoCommand;
        readonly List<CamBox> camBoxs = new List<CamBox>();
        
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

        public Form1()
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

            synoCommand = new SynoCommand(Url);

            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer_login_Tick(sender, e);
        }

        private void UpdatePictureBoxes()
        {
            try
            {
                if (synoCommand.LoginRequired())
                {
                    synoCommand.Login(Username, Password);
                }

                ArrayList cams = synoCommand.GetCams();
                //if (camBoxs.Count != cams.Count)
                {
                    foreach (var camBox in camBoxs)
                    {
                        camBox.Dispose();
                        Controls.Remove(camBox);
                    }
                    camBoxs.Clear();
                    CreatePictureBoxes();
                }
                //else
                //{
                //    for (int i = 0; i < cams.Count; i++)
                //    {
                //        dynamic cam = cams[i];
                //        camBoxs[i].SetValues(cam, synoCommand);
                //    }
                //}
            }
            catch (Exception)
            {
                foreach (var camBox in camBoxs)
                {
                    camBox.Dispose();
                    Controls.Remove(camBox);
                }

                camBoxs.Clear();
            }
        }

        private void CreatePictureBoxes()
        {
            ArrayList cams = synoCommand.GetCams();
            foreach (dynamic cam in cams)
            {
                if((CamStatus)cam["status"] == CamStatus.Disabled)
                    continue;

                CamBox camBox = new CamBox();
                camBox.SetValues(cam, synoCommand);
                camBoxs.Add(camBox);
                Controls.Add(camBox);
            }
        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                fileStream.Write(byteArray, 0, byteArray.Length);
                fileStream.Close();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"Exception caught in process: {0}", ex);
            }
            return false;
        }

        private byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if(camBoxs.Count < 1)
                return;

            int maxPictureBoxWith = ((Width -12)/camBoxs.Count) - camBoxs.Count;

            for (int i = 0; i < camBoxs.Count; i++)
            {
                camBoxs[i].Size = new Size(maxPictureBoxWith, Height -38);
                camBoxs[i].Location = new Point(i*(maxPictureBoxWith + 1), 0);
            }
        }

        private void timer_login_Tick(object sender, EventArgs e)
        {
            try
            {
                //synoCommand.Logout();
                if(synoCommand.LoginRequired())
                    synoCommand.Login(Username, Password);
                UpdatePictureBoxes();
                Form1_Resize(sender, e);
            }
            catch (Exception)
            { }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            synoCommand.Logout();
        }


    }
}
