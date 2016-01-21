using System.Collections.Generic;

namespace SynoCamWPF
{
    public class CameraViewsContext : BindableBase
    {
        private List<CamControl> _cameraViews;

        public List<CamControl> CameraViews
        {
            get { return _cameraViews; }
            set
            {
                _cameraViews = value;
                OnPropertyChanged();
            }
        }
    }
}
