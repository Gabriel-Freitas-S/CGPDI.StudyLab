using System.Windows;
using CGPDI.StudyLab.Views;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UiTests
{
    public class LaboratorioUiTests
    {
        [UIFact]
        public void CodeStudioWindow_InstantiatesAndLoadsLessons()
        {
            var window = new CodeStudioWindow(1);
            window.Should().NotBeNull();

            window.LstStudioLessons.Items.Count.Should().Be(15);
            window.LstStudioLessons.SelectedIndex.Should().Be(0);

            // Verifica elementos de UI essenciais
            window.TxtStudioLessonTitle.Text.Should().NotBeNullOrEmpty();
            window.TxtStudioQuizQuestion.Text.Should().NotBeNullOrEmpty();
            window.BtnStudioQuizOpt0.Visibility.Should().Be(Visibility.Visible);

            // Simula clique em opção de quiz
            window.BtnStudioQuizOpt0.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            window.BrdStudioQuizFeedback.Visibility.Should().Be(Visibility.Visible);

            // Testa botões de alternância de painéis
            window.BtnStudioToggleTrack.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            window.BtnStudioToggleTrack.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

            window.BtnStudioToggleCanvas.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            window.BtnStudioToggleCanvas.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));

            // Testa mudança de lição
            window.LstStudioLessons.SelectedIndex = 1;
            window.TxtStudioLessonTitle.Text.Should().NotBeNullOrEmpty();

            window.Close();
        }
    }
}
