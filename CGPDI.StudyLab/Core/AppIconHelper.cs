using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Gerador e gerenciador dinâmico de ícones da aplicação em alta definição (ICO / PNG / Vector).
    /// </summary>
    public static class AppIconHelper
    {
        private static ImageSource? _cachedIcon;

        /// <summary>
        /// Obtém ou gera a ImageSource do ícone oficial para uso nas janelas WPF.
        /// </summary>
        public static ImageSource GetAppIcon()
        {
            if (_cachedIcon != null) return _cachedIcon;

            try
            {
                EnsureIconFilesExist();
                string icoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "app_icon.ico");
                if (File.Exists(icoPath))
                {
                    _cachedIcon = BitmapFrame.Create(new Uri(icoPath, UriKind.Absolute));
                    return _cachedIcon;
                }
            }
            catch
            {
                // Fallback para renderização em memória
            }

            _cachedIcon = RenderVectorIcon(64);
            return _cachedIcon;
        }

        /// <summary>
        /// Garante que os arquivos app_icon.ico e logo.png existam na pasta Assets tanto do projeto quanto do output.
        /// </summary>
        public static void EnsureIconFilesExist()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string assetsDir = Path.Combine(baseDir, "Assets");
                if (!Directory.Exists(assetsDir)) Directory.CreateDirectory(assetsDir);

                string icoPath = Path.Combine(assetsDir, "app_icon.ico");
                string pngPath = Path.Combine(assetsDir, "logo.png");

                // Se não existir, gera os arquivos em alta resolução
                if (!File.Exists(icoPath) || !File.Exists(pngPath))
                {
                    GenerateAndSaveIcons(icoPath, pngPath);
                }

                // Também tenta salvar na pasta de código-fonte se acessível
                try
                {
                    string sourceDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "Assets"));
                    if (Directory.Exists(sourceDir))
                    {
                        string srcIco = Path.Combine(sourceDir, "app_icon.ico");
                        string srcPng = Path.Combine(sourceDir, "logo.png");
                        if (!File.Exists(srcIco) || !File.Exists(srcPng))
                        {
                            GenerateAndSaveIcons(srcIco, srcPng);
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignora falhas de escrita na pasta do repositório em tempo de execução
                }
            }
            catch (Exception)
            {
                // Fallback seguro se não houver permissão de escrita no diretório do executável
            }
        }

        public static void GenerateAndSaveIcons(string icoFilePath, string pngFilePath)
        {
            int[] sizes = { 16, 32, 48, 64, 128, 256 };
            var pngBuffers = GeneratePngBuffers(sizes, pngFilePath);

            if (!string.IsNullOrEmpty(icoFilePath))
            {
                SaveIcoFile(icoFilePath, sizes, pngBuffers);
            }
        }

        private static List<byte[]> GeneratePngBuffers(int[] sizes, string pngFilePath)
        {
            var pngBuffers = new List<byte[]>();

            foreach (int size in sizes)
            {
                var rtb = RenderVectorIcon(size);
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using var ms = new MemoryStream();
                encoder.Save(ms);
                byte[] bytes = ms.ToArray();
                pngBuffers.Add(bytes);

                if (size == 256 && !string.IsNullOrEmpty(pngFilePath))
                {
                    SavePngFile(pngFilePath, bytes);
                }
            }

            return pngBuffers;
        }

        private static void SavePngFile(string pngFilePath, byte[] bytes)
        {
            string? dir = Path.GetDirectoryName(pngFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
            File.WriteAllBytes(pngFilePath, bytes);
        }

        private static void SaveIcoFile(string icoFilePath, int[] sizes, List<byte[]> pngBuffers)
        {
            string? dir = Path.GetDirectoryName(icoFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir)) Directory.CreateDirectory(dir);

            using var fs = new FileStream(icoFilePath, FileMode.Create, FileAccess.Write);
            using var bw = new BinaryWriter(fs);

            // ICO Header (6 bytes)
            bw.Write((short)0); // Reserved
            bw.Write((short)1); // Type: 1 = Icon
            bw.Write((short)sizes.Length); // Image Count

            int offset = 6 + (16 * sizes.Length);

            // Directory Entries (16 bytes per image)
            for (int i = 0; i < sizes.Length; i++)
            {
                int size = sizes[i];
                byte[] pngData = pngBuffers[i];

                bw.Write((byte)(size == 256 ? 0 : size)); // Width
                bw.Write((byte)(size == 256 ? 0 : size)); // Height
                bw.Write((byte)0); // Color count
                bw.Write((byte)0); // Reserved
                bw.Write((short)1); // Color planes
                bw.Write((short)32); // Bits per pixel
                bw.Write(pngData.Length); // Size of image data
                bw.Write(offset); // Offset of image data

                offset += pngData.Length;
            }

            // Image Data (PNG streams)
            for (int i = 0; i < sizes.Length; i++)
            {
                bw.Write(pngBuffers[i]);
            }
        }

        /// <summary>
        /// Desenha o logotipo vetorial oficial do CGPDI StudyLab em um RenderTargetBitmap.
        /// </summary>
        public static RenderTargetBitmap RenderVectorIcon(int size)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                double s = size / 64.0;

                // Fundo 100% transparente (Alpha = 0) para compatibilidade limpa com qualquer tema/área de trabalho

                // Raios Luminosos dos Vértices
                var cyanPen = new Pen(new SolidColorBrush(Color.FromArgb(180, 56, 189, 248)), 1.5 * s);
                var magentaPen = new Pen(new SolidColorBrush(Color.FromArgb(180, 236, 72, 153)), 1.5 * s);
                var amberPen = new Pen(new SolidColorBrush(Color.FromArgb(180, 251, 191, 36)), 1.5 * s);

                dc.DrawLine(cyanPen, new Point(32 * s, 10 * s), new Point(32 * s, 4 * s));
                dc.DrawLine(cyanPen, new Point(12 * s, 21 * s), new Point(6 * s, 17 * s));
                dc.DrawLine(amberPen, new Point(52 * s, 21 * s), new Point(58 * s, 17 * s));
                dc.DrawLine(magentaPen, new Point(12 * s, 43 * s), new Point(6 * s, 47 * s));
                dc.DrawLine(amberPen, new Point(52 * s, 43 * s), new Point(58 * s, 47 * s));
                dc.DrawLine(magentaPen, new Point(32 * s, 54 * s), new Point(32 * s, 60 * s));

                // Face Superior (Ciano)
                var topGeo = new PathGeometry(new[] {
                    new PathFigure(new Point(32 * s, 10 * s), new[] {
                        new LineSegment(new Point(52 * s, 21 * s), true),
                        new LineSegment(new Point(32 * s, 32 * s), true),
                        new LineSegment(new Point(12 * s, 21 * s), true)
                    }, true)
                });
                var topBrush = new LinearGradientBrush(Color.FromArgb(240, 56, 189, 248), Color.FromArgb(160, 2, 132, 199), new Point(0, 0), new Point(1, 1));
                dc.DrawGeometry(topBrush, new Pen(new SolidColorBrush(Color.FromRgb(103, 232, 249)), 1.5 * s), topGeo);

                // Face Esquerda (Magenta)
                var leftGeo = new PathGeometry(new[] {
                    new PathFigure(new Point(12 * s, 21 * s), new[] {
                        new LineSegment(new Point(32 * s, 32 * s), true),
                        new LineSegment(new Point(32 * s, 54 * s), true),
                        new LineSegment(new Point(12 * s, 43 * s), true)
                    }, true)
                });
                var leftBrush = new LinearGradientBrush(Color.FromArgb(240, 236, 72, 153), Color.FromArgb(160, 147, 51, 234), new Point(0, 0), new Point(1, 1));
                dc.DrawGeometry(leftBrush, new Pen(new SolidColorBrush(Color.FromRgb(244, 114, 182)), 1.5 * s), leftGeo);

                // Face Direita (Ouro / Âmbar)
                var rightGeo = new PathGeometry(new[] {
                    new PathFigure(new Point(32 * s, 32 * s), new[] {
                        new LineSegment(new Point(52 * s, 21 * s), true),
                        new LineSegment(new Point(52 * s, 43 * s), true),
                        new LineSegment(new Point(32 * s, 54 * s), true)
                    }, true)
                });
                var rightBrush = new LinearGradientBrush(Color.FromArgb(240, 251, 191, 36), Color.FromArgb(160, 234, 88, 12), new Point(0, 0), new Point(1, 1));
                dc.DrawGeometry(rightBrush, new Pen(new SolidColorBrush(Color.FromRgb(252, 211, 77)), 1.5 * s), rightGeo);

                // Detalhes da Matriz de Pixels
                var pTop = new PathGeometry(new[] {
                    new PathFigure(new Point(32 * s, 16 * s), new[] {
                        new LineSegment(new Point(38 * s, 19.5 * s), true),
                        new LineSegment(new Point(32 * s, 23 * s), true),
                        new LineSegment(new Point(26 * s, 19.5 * s), true)
                    }, true)
                });
                dc.DrawGeometry(new SolidColorBrush(Color.FromArgb(240, 224, 242, 254)), null, pTop);

                // Raios e Vértices Luminosos
                dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(255, 255, 255)), new Pen(new SolidColorBrush(Color.FromRgb(96, 165, 250)), 1.5 * s), new Point(32 * s, 32 * s), 3 * s, 3 * s);
                dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(224, 242, 254)), null, new Point(32 * s, 10 * s), 2.5 * s, 2.5 * s);
                dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(254, 243, 199)), null, new Point(52 * s, 21 * s), 2.5 * s, 2.5 * s);
                dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(252, 231, 243)), null, new Point(12 * s, 21 * s), 2.5 * s, 2.5 * s);
                dc.DrawEllipse(new SolidColorBrush(Color.FromRgb(255, 228, 230)), null, new Point(32 * s, 54 * s), 2.5 * s, 2.5 * s);
            }

            var rtb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            rtb.Freeze();
            return rtb;
        }

        public static RenderTargetBitmap RenderInstallerSplash(int width = 500, int height = 320)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Fundo Escuro com Gradiente
                var bgBrush = new LinearGradientBrush(
                    Color.FromRgb(13, 14, 24),
                    Color.FromRgb(5, 5, 10),
                    new Point(0, 0),
                    new Point(1, 1));
                dc.DrawRoundedRectangle(bgBrush, new Pen(new SolidColorBrush(Color.FromRgb(34, 34, 56)), 1.5), new Rect(1, 1, width - 2, height - 2), 12, 12);

                // Halos de Luz Neon
                dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(30, 56, 189, 248)), null, new Point(width / 2.0, 100), 100, 70);
                dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(25, 236, 72, 153)), null, new Point((width / 2.0) - 70, 120), 80, 60);
                dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(25, 251, 191, 36)), null, new Point((width / 2.0) + 70, 120), 80, 60);

                // Cubo 3D Isométrico Central
                var iconBitmap = RenderVectorIcon(110);
                dc.DrawImage(iconBitmap, new Rect((width - 110) / 2.0, 30, 110, 110));

                // Tipografia Oficial
                var titleTypeface = new Typeface(new FontFamily("Segoe UI, Inter, Arial"), FontStyles.Normal, FontWeights.Black, FontStretches.Normal);
                var titleText = new FormattedText(
                    "CGPDI STUDYLAB",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    titleTypeface,
                    22,
                    new SolidColorBrush(Color.FromRgb(56, 189, 248)),
                    96.0);
                dc.DrawText(titleText, new Point((width - titleText.Width) / 2.0, 160));

                var subTypeface = new Typeface(new FontFamily("Segoe UI, Arial"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal);
                var subText = new FormattedText(
                    "Laboratório de Computação Gráfica & PDI (2D / 3D)",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    subTypeface,
                    12,
                    new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                    96.0);
                dc.DrawText(subText, new Point((width - subText.Width) / 2.0, 195));

                var footerTypeface = new Typeface(new FontFamily("Segoe UI, Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var footerText = new FormattedText(
                    "Instalador Velopack Oficial • .NET 10 • Zero-Admin",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    footerTypeface,
                    10.5,
                    new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                    96.0);
                dc.DrawText(footerText, new Point((width - footerText.Width) / 2.0, 235));
            }

            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            rtb.Freeze();
            return rtb;
        }

        public static RenderTargetBitmap RenderMsiBanner(int width = 493, int height = 58)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Fundo Escuro do Banner
                var bgBrush = new LinearGradientBrush(
                    Color.FromRgb(16, 20, 34),
                    Color.FromRgb(10, 13, 24),
                    new Point(0, 0),
                    new Point(1, 0));
                dc.DrawRectangle(bgBrush, null, new Rect(0, 0, width, height));
                dc.DrawLine(new Pen(new SolidColorBrush(Color.FromRgb(37, 47, 70)), 1), new Point(0, height - 1), new Point(width, height - 1));

                // Texto do Banner à esquerda
                var titleTypeface = new Typeface(new FontFamily("Segoe UI, Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                var titleText = new FormattedText(
                    "CGPDI StudyLab",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    titleTypeface,
                    15,
                    new SolidColorBrush(Color.FromRgb(248, 250, 252)),
                    96.0);
                dc.DrawText(titleText, new Point(18, 10));

                var subTypeface = new Typeface(new FontFamily("Segoe UI, Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var subText = new FormattedText(
                    "Instalação Corporativa / Laboratórios Universitários",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    subTypeface,
                    11,
                    new SolidColorBrush(Color.FromRgb(56, 189, 248)),
                    96.0);
                dc.DrawText(subText, new Point(18, 30));

                // Mini Ícone 3D à direita
                var iconBitmap = RenderVectorIcon(42);
                dc.DrawImage(iconBitmap, new Rect(width - 55, 8, 42, 42));
            }

            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            rtb.Freeze();
            return rtb;
        }

        public static RenderTargetBitmap RenderMsiLogo(int width = 493, int height = 312)
        {
            var visual = new DrawingVisual();
            using (var dc = visual.RenderOpen())
            {
                // Fundo Escuro com Gradiente
                var bgBrush = new LinearGradientBrush(
                    Color.FromRgb(13, 14, 24),
                    Color.FromRgb(5, 5, 10),
                    new Point(0, 0),
                    new Point(1, 1));
                dc.DrawRectangle(bgBrush, null, new Rect(0, 0, width, height));

                // Halos de Luz Neon
                dc.DrawEllipse(new SolidColorBrush(Color.FromArgb(30, 56, 189, 248)), null, new Point(width / 2.0, 95), 120, 80);

                // Ícone 3D Central
                var iconBitmap = RenderVectorIcon(110);
                dc.DrawImage(iconBitmap, new Rect((width - 110) / 2.0, 35, 110, 110));

                // Textos
                var titleTypeface = new Typeface(new FontFamily("Segoe UI, Arial"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
                var titleText = new FormattedText(
                    "CGPDI StudyLab",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    titleTypeface,
                    20,
                    new SolidColorBrush(Color.FromRgb(248, 250, 252)),
                    96.0);
                dc.DrawText(titleText, new Point((width - titleText.Width) / 2.0, 165));

                var subTypeface = new Typeface(new FontFamily("Segoe UI, Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
                var subText = new FormattedText(
                    "Plataforma de Computação Gráfica & PDI em .NET 10",
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    subTypeface,
                    12,
                    new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                    96.0);
                dc.DrawText(subText, new Point((width - subText.Width) / 2.0, 198));
            }

            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            rtb.Freeze();
            return rtb;
        }

        public static void GenerateInstallerVisualAssets(string assetsDir)
        {
            if (!Directory.Exists(assetsDir)) Directory.CreateDirectory(assetsDir);

            // 1. Splash Screen do Velopack Setup (.png)
            SaveBitmapAsPng(RenderInstallerSplash(500, 320), Path.Combine(assetsDir, "installer_splash.png"));

            // 2. Banner do Instalador MSI (493x58) - WiX exige extensão e formato .bmp
            var bannerRtb = RenderMsiBanner(493, 58);
            SaveBitmapAsBmp(bannerRtb, Path.Combine(assetsDir, "msi_banner.bmp"));
            SaveBitmapAsPng(bannerRtb, Path.Combine(assetsDir, "msi_banner.png"));

            // 3. Logo Dialog do Instalador MSI (493x312) - WiX exige extensão e formato .bmp
            var logoRtb = RenderMsiLogo(493, 312);
            SaveBitmapAsBmp(logoRtb, Path.Combine(assetsDir, "msi_dialog_logo.bmp"));
            SaveBitmapAsPng(logoRtb, Path.Combine(assetsDir, "msi_dialog_logo.png"));
        }

        private static void SaveBitmapAsPng(RenderTargetBitmap rtb, string filePath)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            File.WriteAllBytes(filePath, ms.ToArray());
        }

        private static void SaveBitmapAsBmp(RenderTargetBitmap rtb, string filePath)
        {
            var encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            File.WriteAllBytes(filePath, ms.ToArray());
        }
    }
}
