using System;
using System.Drawing;
using System.IO;

namespace SynoCamLib
{
    public interface ICam
    {
        string CamName { get; }
        CamStatus Status { get; }
        bool Enabled { get; }
        //string Url { get; }

        MemoryStream CamImageData { get; }
        void TryRefreshCamImage();

        EventHandler ImageChangedEvent { get; set; }
    }
}
