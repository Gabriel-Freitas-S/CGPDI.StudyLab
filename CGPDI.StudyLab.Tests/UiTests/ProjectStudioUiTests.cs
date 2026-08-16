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

            // Testa troca de templates
            if (studio.LstProjectTemplates.Items.Count > 1)
            {
                studio.LstProjectTemplates.SelectedIndex = 1;
                studio.TxtStudioCurrentProject.Text.Should().NotBeNullOrEmpty();
            }

            // Testa alteração de sliders
            studio.SliderFree1.Value += 5;
            studio.SliderFree2.Value += 5;
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
