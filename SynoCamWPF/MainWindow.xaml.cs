using System.Windows;
using System.Windows.Input;

namespace SynoCamWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        
        private void Window_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CloseButton.Visibility = Visibility.Visible;
            RefreshButton.Visibility = Visibility.Visible;
            MinimizeButton.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            CloseButton.Visibility = Visibility.Hidden;
            RefreshButton.Visibility = Visibility.Hidden;
            MinimizeButton.Visibility = Visibility.Hidden;
        }

        private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
