using System;
using System.Drawing;
using System.IO;

namespace SynoCamLib
{
    internal class CamEvent : ICamEvent
    {
        private readonly string _snapShot;
        private Image _snapShotImage;

        public CamEvent(int id, string name, string camName, DateTime startTime, DateTime stopTime, EventReason reason, string snapshotData)
        {
            Name = name;
            CamName = camName;
            Reason = reason;
            _snapShot = snapshotData;
            StartTime = startTime;
            StopTime = stopTime;
            Id = id;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string CamName { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime StopTime { get; private set; }
        public EventReason Reason { get; private set; }


        public Image SnapShot
        {
            get { return GetImageFromSnapshotData(_snapShot); }
        }

        public Image GetImageFromSnapshotData(string data)
        {
            if (_snapShotImage != null)
                return _snapShotImage;

            _snapShotImage = Base64ToImage(data); // Cache in case of later retrieval

            return _snapShotImage;
        }

        public Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0,imageBytes.Length);

            ms.Write(imageBytes, 0, imageBytes.Length);

            return Image.FromStream(ms, true);
        }
    }
}
