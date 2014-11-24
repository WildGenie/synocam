using System;
using System.Windows.Forms;

namespace cam
{
    public partial class CamBox : UserControl
    {
        private string url;
        private int imageNumber;
        private int timerTick;

        public CamBox()
        {
            InitializeComponent();
        }

        public void SetValues(dynamic cam, SynoCommand synoCommand)
        {
            imageNumber = 0;
            var camName = cam["name"];
            var camStatus = (CamStatus)cam["status"];

            toolTip1.SetToolTip(pictureBox1, camName);
            bool enabled = (cam["enabled"] && (camStatus == CamStatus.Normal));
            
            label1.Visible = !enabled;
            pictureBox1.Visible = enabled;

            label1.Text = string.Format(@"{0}: {1}", camName, camStatus);
            if (enabled)
            {
                url = synoCommand.GetCamImageUrl(cam["id"].ToString());
                timer_refresh_Tick(this, null);
            }

            timer_refresh.Enabled = enabled;
        }

        private bool IsTimeOfDayBetween(DateTime time, TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime == startTime)
            {
                return true;
            }
            if (endTime < startTime)
            {
                return time.TimeOfDay <= endTime ||
                       time.TimeOfDay >= startTime;
            }
            return time.TimeOfDay >= startTime &&
                   time.TimeOfDay <= endTime;
        }

        private void timer_refresh_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.OnlyFunctionOnWorkingHours && 
                !IsTimeOfDayBetween(DateTime.Now, new TimeSpan(8, 00, 0), new TimeSpan(17, 00, 0)))
            {
                label1.Visible = true;
                label1.Text = @"Non working hours..";
                return;
            }

            reloadImage();
        }

        private void pictureBox1_LoadProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
        {
            //label1.Text = string.Format("{0}/{1}: loading {2}%", imageNumber + 1, timerTick, e.ProgressPercentage);
        }

        private void pictureBox1_LoadCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            imageNumber++;

            if (e.Error != null)
                label1.Text = e.Error.ToString();

//            if(e.Cancelled)
//                label1.Text = string.Format("{0}/{1}: loading cancelled", imageNumber, timerTick);
//            else
//                label1.Text = string.Format("{0}/{1}", imageNumber, timerTick);

            panel1.Visible = !panel1.Visible;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            timer_refresh.Interval = (int)TimeSpan.FromSeconds(4).TotalMilliseconds;
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            timer_refresh.Interval = (int)TimeSpan.FromSeconds(60).TotalMilliseconds;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            reloadImage();
        }

        private void reloadImage()
        {
            label1.Visible = false;
            timerTick ++;
            pictureBox1.CancelAsync();
            pictureBox1.WaitOnLoad = false;
            pictureBox1.LoadAsync(url);
        }
    }
}
