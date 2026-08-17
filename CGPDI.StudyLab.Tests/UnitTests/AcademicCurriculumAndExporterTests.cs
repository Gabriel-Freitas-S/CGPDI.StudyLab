using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class AcademicCurriculumAndExporterTests
    {
        [Fact]
        public void NewAppliedTopics_ExistAndAreProperlyFormulated()
        {
            var topics = StudyGuideData.GetTopics();

            string[] expectedIds = {
                "cg2d_templates_and_pipeline",
                "cg2d_activity_articulated_system",
                "cg3d_mesh_cameras_lambert",
                "cg3d_activity_architectural_scene",
                "cg3d_hierarchical_modeling",
                "cg3d_activity_quadruped_caravan"
            };

            foreach (var id in expectedIds)
            {
                var topic = topics.FirstOrDefault(t => t.Id == id);
                topic.Should().NotBeNull($"O tópico com ID '{id}' deve existir.");
                topic!.Title.Should().NotBeNullOrWhiteSpace();
                topic.Summary.Should().NotBeNullOrWhiteSpace();
                topic.MathFormulas.Should().NotBeNullOrWhiteSpace();
                topic.CodeExplanation.Should().NotBeNullOrWhiteSpace();
                topic.CodeSnippet.Should().NotBeNullOrWhiteSpace();
                topic.Quiz.Should().NotBeNull();
                topic.Quiz!.Options.Should().HaveCount(3);
                topic.Quiz.Explanation.Should().NotBeNullOrWhiteSpace();
            }
        }

        [Fact]
        public void NewInteractiveLessons_ExecuteSimulationAndProvideSolutions()
        {
            var lessons = InteractiveLabManager.GetLessons();

            int[] expectedLessonNumbers = { 13, 14, 15 };
            using var bmp = new DirectBitmap(512, 512);

            foreach (int num in expectedLessonNumbers)
            {
                var lesson = lessons.FirstOrDefault(l => l.Number == num);
                lesson.Should().NotBeNull($"A lição {num} deve existir.");
                lesson!.SolutionCode.Should().NotBeNullOrWhiteSpace();
                lesson.StarterTemplate.Should().NotBeNullOrWhiteSpace();
                lesson.BlankTemplate.Should().NotBeNullOrWhiteSpace();
                lesson.QuizOptions.Should().ContainSingle(o => o.IsCorrect);

                var log = new StringBuilder();
                InteractiveLabManager.RenderSimulation(bmp, lesson, 15, 20, 30, 40, 0, log);
                log.Length.Should().BeGreaterThan(0);
            }
        }

        [Fact]
        public void AppliedProjectTemplates_AreConfiguredProperly()
        {
            var templates = ProjectTemplatesManager.GetTemplates();

            string[] expectedTemplateIds = {
                "vehicle-articulated-2d",
                "architectural-scene-3d",
                "hierarchical-quadruped-3d"
            };

            foreach (var id in expectedTemplateIds)
            {
                var tpl = templates.FirstOrDefault(t => t.Id == id);
                tpl.Should().NotBeNull($"O template '{id}' deve existir.");
                tpl!.Title.Should().NotBeNullOrWhiteSpace();
                tpl.Category.Should().Be("Projetos de Computação Gráfica Aplicada");
                tpl.InitialCode.Should().NotBeNullOrWhiteSpace();
                tpl.XamlCode.Should().NotBeNullOrWhiteSpace();
                tpl.StepsGuide.Should().NotBeNullOrWhiteSpace();
                tpl.IsInteractiveActivity.Should().BeTrue();
            }
        }

        [Fact]
        public void ProceduralTextures_GenerateValidDirectBitmaps()
        {
            using var stoneBmp = ImageSampleGenerator.GenerateStoneGraniteTexture(128, 128);
            stoneBmp.Should().NotBeNull();
            stoneBmp.Width.Should().Be(128);
            stoneBmp.Height.Should().Be(128);
            stoneBmp.GetPixel(64, 64).Should().NotBe(0);

            using var sandBmp = ImageSampleGenerator.GenerateDesertSandTexture(128, 128);
            sandBmp.Should().NotBeNull();
            sandBmp.Width.Should().Be(128);
            sandBmp.Height.Should().Be(128);
            sandBmp.GetPixel(64, 64).Should().NotBe(0);
        }

        [Fact]
        public void AcademicProjectExporter_ExportsCompleteSolution()
        {
            var templates = ProjectTemplatesManager.GetTemplates();
            var tpl = templates.First(t => t.Id == "vehicle-articulated-2d");

            string tempDir = Path.Combine(Path.GetTempPath(), "CGPDI_TestExport_" + Guid.NewGuid().ToString("N"));
            string zipPath = Path.Combine(Path.GetTempPath(), "CGPDI_TestExport_" + Guid.NewGuid().ToString("N") + ".zip");

            try
            {
                // Teste de exportação para diretório
                AcademicProjectExporter.ExportProjectToDirectory(tpl, tempDir);
                string projName = AcademicProjectExporter.SanitizeProjectName(tpl.Title);

                File.Exists(Path.Combine(tempDir, $"{projName}.sln")).Should().BeTrue("Arquivo .sln deve ser gerado.");
                File.Exists(Path.Combine(tempDir, projName, $"{projName}.csproj")).Should().BeTrue("Arquivo .csproj deve ser gerado.");
                File.Exists(Path.Combine(tempDir, projName, "App.xaml")).Should().BeTrue("App.xaml deve ser gerado.");
                File.Exists(Path.Combine(tempDir, projName, "MainWindow.xaml")).Should().BeTrue("MainWindow.xaml deve ser gerado.");
                File.Exists(Path.Combine(tempDir, "README.md")).Should().BeTrue("README.md deve ser gerado.");

                // Teste de exportação para ZIP
                AcademicProjectExporter.ExportProjectToZip(tpl, zipPath);
                File.Exists(zipPath).Should().BeTrue("Arquivo ZIP deve ser criado.");

                using var archive = ZipFile.OpenRead(zipPath);
                archive.Entries.Should().Contain(e => e.FullName.EndsWith(".sln"));
                archive.Entries.Should().Contain(e => e.FullName.EndsWith(".csproj"));
                archive.Entries.Should().Contain(e => e.FullName.EndsWith("MainWindow.xaml"));
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch { }
                }
                if (File.Exists(zipPath))
                {
                    try { File.Delete(zipPath); } catch { }
                }
            }
        }
    }
}
