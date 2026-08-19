using System.Threading.Tasks;
using System.Windows.Controls;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.Views;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class CompilerAndXamlTests
    {
        [UIFact]
        public void EvaluateXaml_BasicGrid_InstantiatesUIElement()
        {
            string xaml = @"<Grid Width=""200"" Height=""100"">
    <Button Content=""Clique Aqui"" Width=""80"" Height=""30""/>
</Grid>";
            var result = LiveCodeCompiler.EvaluateXaml(xaml);
            result.Success.Should().BeTrue();
            result.Element.Should().NotBeNull();
            result.Element.Should().BeOfType<Grid>();
        }

        [UIFact]
        public void EvaluateXaml_WindowWithXClass_SanitizesAndExtractsContent()
        {
            string windowXaml = @"<Window x:Class=""MeuApp.MainWindow""
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        Title=""Minha Janela"" Height=""300"" Width=""400"">
    <StackPanel>
        <TextBlock Text=""Texto de Teste""/>
    </StackPanel>
</Window>";

            var result = LiveCodeCompiler.EvaluateXaml(windowXaml);
            result.Success.Should().BeTrue();
            result.Element.Should().NotBeNull();
            result.Element.Should().BeOfType<Border>();
            var border = (Border)result.Element!;
            border.Child.Should().BeOfType<StackPanel>();
        }

        [UIFact]
        public void EvaluateXaml_ProjectNamespaceWithoutAssembly_StillResolvesControl()
        {
            string xaml = @"<views:ProjectStudioControl
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:views=""clr-namespace:CGPDI.StudyLab.Views"" />";

            var result = LiveCodeCompiler.EvaluateXaml(xaml);
            result.Success.Should().BeTrue();
            result.Element.Should().BeOfType<ProjectStudioControl>();
        }

        [Fact]
        public async Task ExecuteCustomScriptAsync_ValidScript_ModifiesBitmap()
        {
            using var bmp = new DirectBitmap(32, 32);
            string script = @"
Output.Clear(0xFF0000);
";
            var result = await LiveCodeCompiler.ExecuteCustomScriptAsync(script, bmp, null, 1, 1, 1, 1);
            result.Success.Should().BeTrue();

            bmp.Lock();
            var col = bmp.GetPixel(10, 10);
            bmp.Unlock(false);

            col.R.Should().Be(255);
            col.G.Should().Be(0);
            col.B.Should().Be(0);
        }

        [Fact]
        public async Task RunTestsAndEvaluateAsync_AllLessonsSolutions_ShouldPassPedagogicalTests()
        {
            var lessons = InteractiveLabManager.GetLessons();
            lessons.Should().NotBeEmpty();

            foreach (var lesson in lessons)
            {
                using var bmp = new DirectBitmap(32, 32);
                var report = await LiveCodeCompiler.RunTestsAndEvaluateAsync(lesson, lesson.SolutionCode, bmp, 128, 128, 128);
                report.Success.Should().BeTrue($"Lição {lesson.Number} ({lesson.Title}) deve passar com a solução oficial.");
                report.Tests.Should().NotBeEmpty();
                report.Tests.TrueForAll(t => t.Passed).Should().BeTrue();
                report.FeedbackReport.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public async Task ExecuteCustomScriptAsync_SyntaxError_ReturnsCompilationError()
        {
            using var bmp = new DirectBitmap(16, 16);
            string badScript = "Esse codigo C# possui erro de sintaxe evidente @#$$!";

            var result = await LiveCodeCompiler.ExecuteCustomScriptAsync(badScript, bmp, null, 1, 1, 1, 1);
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().NotBeNullOrEmpty();
            result.ErrorMessage.Should().Contain("Linha");
        }

        [Fact]
        public async Task ExecuteCustomScriptAsync_AllProjectStudioTemplates_ShouldCompileAndExecuteWithoutErrors()
        {
            var templates = ProjectTemplatesManager.GetTemplates();
            templates.Should().NotBeEmpty();

            foreach (var template in templates)
            {
                using var bmp = new DirectBitmap(64, 64);
                LiveCodeCompiler.ClearCustomScriptCache();

                var result = await LiveCodeCompiler.ExecuteCustomScriptAsync(
                    template.InitialCode,
                    bmp,
                    null,
                    template.Param1Default,
                    template.Param2Default,
                    template.Param3Default,
                    template.Param4Default);

                result.Success.Should().BeTrue($"Template '{template.Title}' ({template.Id}) deve compilar e executar sem erros: {result.ErrorMessage}");
                result.ErrorMessage.Should().BeNull();
            }
        }

        [Fact]
        public void CachedMetadataReferences_ShouldContainCoreFrameworkAndApplicationAssemblies()
        {
            var refs = LiveCodeCompiler.CachedMetadataReferences;
            refs.Should().NotBeEmpty();
            refs.Count.Should().BeGreaterThan(5);
        }
    }
}
