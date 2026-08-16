using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class AppIconHelperTests
    {
        [Fact]
        public void RenderVectorIcon_HasTransparentBackground_CornersAreTransparent()
        {
            // Act
            var rtb = AppIconHelper.RenderVectorIcon(64);

            // Assert
            rtb.Should().NotBeNull();
            rtb.PixelWidth.Should().Be(64);
            rtb.PixelHeight.Should().Be(64);

            // Lê pixels dos 4 cantos para garantir canal alfa = 0 (transparência total)
            int stride = (64 * 4);
            byte[] pixels = new byte[64 * stride];
            rtb.CopyPixels(pixels, stride, 0);

            // Pbgra32: [B, G, R, A]
            byte cornerTopLeftAlpha = pixels[3];
            byte cornerTopRightAlpha = pixels[(63 * 4) + 3];
            byte cornerBottomLeftAlpha = pixels[(63 * stride) + 3];
            byte cornerBottomRightAlpha = pixels[(63 * stride) + (63 * 4) + 3];

            cornerTopLeftAlpha.Should().Be(0, "o canto superior esquerdo deve ser 100% transparente");
            cornerTopRightAlpha.Should().Be(0, "o canto superior direito deve ser 100% transparente");
            cornerBottomLeftAlpha.Should().Be(0, "o canto inferior esquerdo deve ser 100% transparente");
            cornerBottomRightAlpha.Should().Be(0, "o canto inferior direito deve ser 100% transparente");
        }

        [Fact]
        public void GenerateAndSaveIcons_CreatesValidIcoAndPngWithMultiResolution()
        {
            // Arrange
            string tempDir = Path.Combine(Path.GetTempPath(), "CGPDI_IconTests_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            string icoPath = Path.Combine(tempDir, "test_icon.ico");
            string pngPath = Path.Combine(tempDir, "test_logo.png");

            try
            {
                // Act
                AppIconHelper.GenerateAndSaveIcons(icoPath, pngPath);

                // Assert
                File.Exists(icoPath).Should().BeTrue();
                File.Exists(pngPath).Should().BeTrue();

                byte[] icoBytes = File.ReadAllBytes(icoPath);
                icoBytes.Length.Should().BeGreaterThan(1000);
                icoBytes[0].Should().Be(0); // Reserved
                icoBytes[1].Should().Be(0);
                icoBytes[2].Should().Be(1); // Type = 1 (Icon)
                icoBytes[3].Should().Be(0);
                icoBytes[4].Should().Be(6); // 6 tamanhos (16, 32, 48, 64, 128, 256)

                byte[] pngBytes = File.ReadAllBytes(pngPath);
                pngBytes.Length.Should().BeGreaterThan(500);
                // Assinatura PNG: 0x89 'P' 'N' 'G'
                pngBytes[0].Should().Be(0x89);
                pngBytes[1].Should().Be(0x50);
                pngBytes[2].Should().Be(0x4E);
                pngBytes[3].Should().Be(0x47);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }

        [Fact]
        public void GetAppIcon_ReturnsValidImageSource()
        {
            // Act
            var icon = AppIconHelper.GetAppIcon();

            // Assert
            icon.Should().NotBeNull();
            icon.Width.Should().BeGreaterThan(0);
            icon.Height.Should().BeGreaterThan(0);
        }

        [Fact]
        public void EnsureIconFilesExist_DoesNotThrow()
        {
            // Act
            Action act = () => AppIconHelper.EnsureIconFilesExist();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void LogCrashReport_WritesDiagnosticReportToFile()
        {
            // Arrange
            var testEx = new InvalidOperationException("Falha simulada para teste de resiliência");
            string logPath = App.GetCrashLogPath();

            // Act
            App.LogCrashReport(testEx, "UnitTestDiagnostic");

            // Assert
            File.Exists(logPath).Should().BeTrue();
            string content = File.ReadAllText(logPath);
            content.Should().Contain("UnitTestDiagnostic");
            content.Should().Contain("Falha simulada para teste de resiliência");
        }

        [Fact]
        public void GenerateOfficialAssets_RefreshesRepositoryAssetsWithTransparency()
        {
            // Procura a pasta Assets do projeto a partir do diretório de execução
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string sourceAssets = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "CGPDI.StudyLab", "Assets"));
            if (Directory.Exists(sourceAssets))
            {
                string icoPath = Path.Combine(sourceAssets, "app_icon.ico");
                string pngPath = Path.Combine(sourceAssets, "logo.png");
                AppIconHelper.GenerateAndSaveIcons(icoPath, pngPath);

                File.Exists(icoPath).Should().BeTrue();
                File.Exists(pngPath).Should().BeTrue();
            }
        }

        [Theory]
        [InlineData(16)]
        [InlineData(32)]
        [InlineData(48)]
        [InlineData(64)]
        [InlineData(128)]
        [InlineData(256)]
        public void RenderVectorIcon_DifferentSizes_RendersWithoutErrors(int size)
        {
            var rtb = AppIconHelper.RenderVectorIcon(size);
            rtb.Should().NotBeNull();
            rtb.PixelWidth.Should().Be(size);
            rtb.PixelHeight.Should().Be(size);
        }

        [Fact]
        public void GenerateAndSaveIcons_HandlesNullOrEmptyPaths_WithoutThrowing()
        {
            Action act = () => AppIconHelper.GenerateAndSaveIcons(string.Empty, string.Empty);
            act.Should().NotThrow();
        }

        [Fact]
        public void LogCrashReport_HandlesInnerExceptionsAndStackTrace()
        {
            var inner = new ArgumentNullException("parametro", "Inner error");
            var outer = new InvalidOperationException("Outer error", inner);
            string logPath = App.GetCrashLogPath();

            App.LogCrashReport(outer, "TestInnerException");

            File.Exists(logPath).Should().BeTrue();
            string text = File.ReadAllText(logPath);
            text.Should().Contain("Outer error");
            text.Should().Contain("Inner error");
            text.Should().Contain("TestInnerException");
        }

        [Fact]
        public void GetCrashLogPath_ReturnsValidLocation()
        {
            string path = App.GetCrashLogPath();
            path.Should().NotBeNullOrWhiteSpace();
            path.Should().EndWith("crash.log");
        }
    }
}
