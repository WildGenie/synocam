using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SynoCamLib;

namespace SynoCam
{
    public partial class Disconnect : Form
    {
        private readonly SynoCommand _synoCommand;

        public Disconnect(SynoCommand synoCommand)
        {
            _synoCommand = synoCommand;
            InitializeComponent();
            Shown += OnShown;
        }

        private void OnShown(object sender, EventArgs eventArgs)
        {
            _synoCommand.LogoutASync().ContinueWith(ExitAfterLogout);
        }

        private void ExitAfterLogout(Task task)
        {
            Environment.Exit(0);
        }
    }
}
