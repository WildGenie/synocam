using System;
using System.Drawing;

namespace SynoCamLib
{
    public interface ICamEvent
    {
        int Id { get; }
        string Name { get; }
        string CamName { get; }
        DateTime StartTime { get; }
        DateTime StopTime { get; }
        EventReason Reason { get; }
        Image SnapShot { get; }
    }
}