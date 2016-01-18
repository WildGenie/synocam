namespace SynoCamLib
{
    public interface ICam
    {
        string CamName { get; }
        CamStatus Status { get; }
        bool Enabled { get; }
        string Url { get; }
    }
}
