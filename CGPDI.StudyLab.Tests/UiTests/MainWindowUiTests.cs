using System.Windows;
using System.Windows.Controls;
using CGPDI.StudyLab.Views;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UiTests
{
    public class MainWindowUiTests
    {
        [UIFact]
        public void MainWindow_InstantiatesAndLoadsAll7Tabs()
        {
            var window = new MainWindow();
            window.Should().NotBeNull();

            // Dispara evento Loaded
            window.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

            var tabControl = window.MainTabControl;
            tabControl.Should().NotBeNull();
            tabControl.Items.Count.Should().Be(7);

            // Tab 0: PDI
            tabControl.SelectedIndex = 0;
            window.ImgDisplay.Should().NotBeNull();
            window.TxtTheoryTitle.Should().NotBeNull();

            // Tab 1: 2D
            tabControl.SelectedIndex = 1;
            window.ImgDisplay2D.Should().NotBeNull();
            window.TxtTheory2DTitle.Should().NotBeNull();

            // Tab 2: 3D
            tabControl.SelectedIndex = 2;
            window.ViewportMain.Should().NotBeNull();

            // Tab 3: Ray Tracing
            tabControl.SelectedIndex = 3;
            window.ImgDisplay3DSoft.Should().NotBeNull();

            // Tab 4: Central de Estudos
            tabControl.SelectedIndex = 4;
            window.LstStudyTopics.Should().NotBeNull();
            window.LstStudyTopics.Items.Count.Should().Be(13);

            // Tab 5: Laboratório
            tabControl.SelectedIndex = 5;
            window.LstInteractiveLessons.Should().NotBeNull();
            window.LstInteractiveLessons.Items.Count.Should().Be(12);

            // Tab 6: Estúdio de Projetos
            tabControl.SelectedIndex = 6;
            window.MainStudioControl.Should().NotBeNull();

            window.Close();
        }

        [UIFact]
        public void MainWindow_ContextualTopBar_UpdatesButtonsPerTab()
        {
            var window = new MainWindow();
            window.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

            // Select PDI Tab
            window.MainTabControl.SelectedIndex = 0;
            window.PnlContextualTopActions.Children.Count.Should().BeGreaterThan(0);

            // Select 3D Tab
            window.MainTabControl.SelectedIndex = 2;
            window.PnlContextualTopActions.Children.Count.Should().BeGreaterThan(0);

            // Select Studio Tab
            window.MainTabControl.SelectedIndex = 6;
            window.PnlContextualTopActions.Children.Count.Should().BeGreaterThan(0);

            window.Close();
        }
    }
}
