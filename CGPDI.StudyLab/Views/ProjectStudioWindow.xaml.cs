using System.Windows;
using System.Windows.Input;

namespace CGPDI.StudyLab.Views
{
    public partial class ProjectStudioWindow : BorderlessWindow
    {
        public ProjectStudioWindow()
        {
            InitializeComponent();
            StudioControl.BtnPopoutStudio.Visibility = Visibility.Collapsed;
            Loaded += ProjectStudioWindow_Loaded;
        }

        private void ProjectStudioWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CenterOnScreen();
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
