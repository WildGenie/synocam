using SynoCamLib;

namespace SynoCamWPF
{
    public class CamControl : ICam
    {
        public string CamName { get; private set; }
        public CamStatus Status { get; private set; }
        public bool Enabled { get; private set;  }
        public string Url { get; private set; }
        public RefreshRate RefreshRate { get; private set; }

        public CamControl(ICam cam, RefreshRate refreshRate)
        {
            CamName = cam.CamName;
            Status = cam.Status;
            Enabled = cam.Enabled;
            Url = cam.Url;
            RefreshRate = refreshRate;
        }
    }
}
