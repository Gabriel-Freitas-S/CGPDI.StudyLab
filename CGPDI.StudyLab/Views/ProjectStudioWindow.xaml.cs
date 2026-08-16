using System.Windows;
using System.Windows.Input;

namespace CGPDI.StudyLab.Views
{
    public partial class ProjectStudioWindow : Window
    {
        public ProjectStudioWindow()
        {
            InitializeComponent();
            StudioControl.BtnPopoutStudio.Visibility = Visibility.Collapsed;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    ToggleMaximize();
                }
                else
                {
                    DragMove();
                }
            }
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                BtnMaximize.Content = "🗖";
            }
            else
            {
                WindowState = WindowState.Maximized;
                BtnMaximize.Content = "🗗";
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                StudioControl.ExecuteFreeScript();
                e.Handled = true;
            }
            else if (e.Key == Key.F5)
            {
                StudioControl.ExecuteFreeScript();
                e.Handled = true;
            }
        }
    }
}
