using System.Windows.Forms;

namespace SynoCam
{
    public class NoFocusCueButton : Button
    {

        public override void NotifyDefault(bool value)
        {
            base.NotifyDefault(false);
        }

        protected override bool ShowFocusCues
        {
            get
            {
                return false;
            }
        }
    }
}
