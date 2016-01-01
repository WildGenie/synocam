using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SynoCamLib;

namespace SynoCam
{
    public partial class EventViewer : Form
    {
        private List<CamEvent> camEvents; 
        private SynoCommand _synoCommand;

        public EventViewer()
        {
            InitializeComponent();
            this.Shown += OnShown;
        }

        public void ShowDialog(SynoCommand command)
        {
            _synoCommand = command;
            ShowDialog();
        }

        private async void BuildData()
        {
            eventView.View = View.Details;

            var events = await _synoCommand.QueryCamEvents();

            foreach (var camEvent in events)
            {
                eventView.Invoke(new Action(() =>
                {                  
                    eventView.Items.Add(new ListViewItem(new[] {camEvent.StartTime.ToString(), camEvent.CamName, camEvent.Reason.ToString() }));
                }));
            }

            camEvents = events.ToList(); // Save for later use
        }

        private void OnShown(object sender, EventArgs eventArgs)
        {
            BuildData();
        }

        private void EventViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (eventView.SelectedItems.Count == 1)
            {
                snapshotBox.Image = camEvents[eventView.SelectedItems[0].Index].SnapShot;
            }
        }
    }
}
