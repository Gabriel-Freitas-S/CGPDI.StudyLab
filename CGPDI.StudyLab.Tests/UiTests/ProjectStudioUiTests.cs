using System.Windows.Controls;
using CGPDI.StudyLab.Views;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UiTests
{
    public class ProjectStudioUiTests
    {
        [UIFact]
        public void ProjectStudioControl_LoadsTemplatesAndHandlesLiveEvaluation()
        {
            var studio = new ProjectStudioControl();
            studio.Should().NotBeNull();

            // Templates list loaded
            studio.LstProjectTemplates.Items.Count.Should().BeGreaterThan(0);
            studio.LstProjectTemplates.SelectedIndex.Should().Be(0);

            // Execute XAML in Live Container
            studio.TabStudioEditor.SelectedIndex = 1; // Tab XAML
            studio.ExecuteFreeXaml();

            studio.PnlFreeLiveXamlContainer.Child.Should().NotBeNull();
            studio.TabStudioVisualizer.SelectedItem.Should().Be(studio.TabItemFreeLiveXaml);
        }

        [UIFact]
        public void ProjectStudioWindow_InstantiatesFramelessStudio()
        {
            var win = new ProjectStudioWindow();
            win.Should().NotBeNull();
            win.StudioControl.Should().NotBeNull();
            win.Close();
        }
    }
}
