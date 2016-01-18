namespace SynoCamLib
{
    internal class Cam : ICam
    {
        public string CamName { get; private set; }
        public CamStatus Status { get; private set; }
        public bool Enabled { get; private set; }
        public string Url { get; private set; }

        public Cam(string name, CamStatus status, bool enabled, string url)
        {
            CamName = name;
            Status = status;
            Enabled = enabled;
            Url = url;
        }
    }
}
