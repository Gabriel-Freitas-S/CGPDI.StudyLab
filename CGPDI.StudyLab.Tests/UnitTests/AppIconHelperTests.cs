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
                AppIconHelper.GenerateInstallerVisualAssets(sourceAssets);

                File.Exists(icoPath).Should().BeTrue();
                File.Exists(pngPath).Should().BeTrue();
                File.Exists(Path.Combine(sourceAssets, "installer_splash.png")).Should().BeTrue();
                File.Exists(Path.Combine(sourceAssets, "msi_banner.bmp")).Should().BeTrue();
                File.Exists(Path.Combine(sourceAssets, "msi_dialog_logo.bmp")).Should().BeTrue();
                File.Exists(Path.Combine(sourceAssets, "msi_banner.png")).Should().BeTrue();
                File.Exists(Path.Combine(sourceAssets, "msi_dialog_logo.png")).Should().BeTrue();
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

        [Fact]
        public void RenderInstallerSplash_ReturnsValidDimensions()
        {
            var rtb = AppIconHelper.RenderInstallerSplash(500, 320);
            rtb.Should().NotBeNull();
            rtb.PixelWidth.Should().Be(500);
            rtb.PixelHeight.Should().Be(320);
        }

        [Fact]
        public void RenderMsiBanner_ReturnsExactWixDimensions_493x58()
        {
            var rtb = AppIconHelper.RenderMsiBanner(493, 58);
            rtb.Should().NotBeNull();
            rtb.PixelWidth.Should().Be(493);
            rtb.PixelHeight.Should().Be(58);
        }

        [Fact]
        public void RenderMsiLogo_ReturnsExactWixDimensions_493x312()
        {
            var rtb = AppIconHelper.RenderMsiLogo(493, 312);
            rtb.Should().NotBeNull();
            rtb.PixelWidth.Should().Be(493);
            rtb.PixelHeight.Should().Be(312);
        }

        [Fact]
        public void GenerateInstallerVisualAssets_GeneratesAllRequiredInstallerFiles()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "CGPDI_InstallerTests_" + Guid.NewGuid());
            try
            {
                AppIconHelper.GenerateInstallerVisualAssets(tempDir);

                string splashPath = Path.Combine(tempDir, "installer_splash.png");
                string bannerBmpPath = Path.Combine(tempDir, "msi_banner.bmp");
                string logoBmpPath = Path.Combine(tempDir, "msi_dialog_logo.bmp");
                string bannerPngPath = Path.Combine(tempDir, "msi_banner.png");
                string logoPngPath = Path.Combine(tempDir, "msi_dialog_logo.png");

                File.Exists(splashPath).Should().BeTrue();
                File.Exists(bannerBmpPath).Should().BeTrue("WiX exige msi_banner.bmp com formato Bitmap");
                File.Exists(logoBmpPath).Should().BeTrue("WiX exige msi_dialog_logo.bmp com formato Bitmap");
                File.Exists(bannerPngPath).Should().BeTrue();
                File.Exists(logoPngPath).Should().BeTrue();

                new FileInfo(splashPath).Length.Should().BeGreaterThan(1000);
                new FileInfo(bannerBmpPath).Length.Should().BeGreaterThan(1000);
                new FileInfo(logoBmpPath).Length.Should().BeGreaterThan(1000);
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
        public void CreateDibIconData_For16x16_CreatesValidBitmapInfoHeaderAndAndMask()
        {
            var rtb = AppIconHelper.RenderVectorIcon(16);
            byte[] dibData = AppIconHelper.CreateDibIconData(rtb, 16);

            dibData.Should().NotBeNull();
            dibData.Length.Should().BeGreaterThan(40);

            // BITMAPINFOHEADER: biSize = 40 (bytes 0-3)
            BitConverter.ToUInt32(dibData, 0).Should().Be(40);
            // biWidth = 16 (bytes 4-7)
            BitConverter.ToInt32(dibData, 4).Should().Be(16);
            // biHeight = 32 (16 * 2) (bytes 8-11)
            BitConverter.ToInt32(dibData, 8).Should().Be(32);
            // biPlanes = 1 (bytes 12-13)
            BitConverter.ToUInt16(dibData, 12).Should().Be(1);
            // biBitCount = 32 (bytes 14-15)
            BitConverter.ToUInt16(dibData, 14).Should().Be(32);
            // biCompression = 0 (BI_RGB) (bytes 16-19)
            BitConverter.ToUInt32(dibData, 16).Should().Be(0);
        }

        [Fact]
        public void GenerateAndSaveIcons_EmbedsDibForSmallSizesAndPngFor256()
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "CGPDI_DibIconTests_" + Guid.NewGuid());
            Directory.CreateDirectory(tempDir);
            string icoPath = Path.Combine(tempDir, "dib_test.ico");
            string pngPath = Path.Combine(tempDir, "dib_test.png");

            try
            {
                AppIconHelper.GenerateAndSaveIcons(icoPath, pngPath);

                byte[] icoBytes = File.ReadAllBytes(icoPath);
                icoBytes.Length.Should().BeGreaterThan(1000);

                // Offset da primeira imagem (16x16) = 6 + (16 * 6) = 102
                int firstImageOffset = BitConverter.ToInt32(icoBytes, 18);
                firstImageOffset.Should().Be(102);

                // A primeira imagem (16x16) deve começar com BITMAPINFOHEADER (biSize = 40)
                uint dibHeaderSize = BitConverter.ToUInt32(icoBytes, firstImageOffset);
                dibHeaderSize.Should().Be(40, "ícones menores ou iguais a 128px devem usar DIB com BITMAPINFOHEADER para compatibilidade e transparência");

                // Offset da última imagem (256x256) - entrada 5
                int lastEntryOffset = 6 + (16 * 5);
                int lastImageOffset = BitConverter.ToInt32(icoBytes, lastEntryOffset + 12);

                // A última imagem (256x256) deve começar com a assinatura PNG (0x89 'P' 'N' 'G')
                icoBytes[lastImageOffset].Should().Be(0x89);
                icoBytes[lastImageOffset + 1].Should().Be(0x50);
                icoBytes[lastImageOffset + 2].Should().Be(0x4E);
                icoBytes[lastImageOffset + 3].Should().Be(0x47);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    Directory.Delete(tempDir, true);
                }
            }
        }
    }
}
