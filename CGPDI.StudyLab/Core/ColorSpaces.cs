using System;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Modelos de Espaço de Cores e Transformações Cromáticas em Computação Gráfica.
    /// 
    /// TEORIA FUNDAMENTAL:
    /// 1. RGB (Red, Green, Blue): Modelo aditivo baseado na resposta dos cones tricromáticos humanos (L, M, S).
    ///    Usado em telas digitais, monitores e sensores de câmera.
    /// 2. HSV/HSL (Hue, Saturation, Value/Lightness): Modelo perceptivo cilíndrico, muito mais intuitivo para
    ///    segmentação por cor e interfaces de usuário do que o cubo RGB.
    /// 3. YCbCr: Separa a luminância (Y - brilho perceptivo) das crominâncias (Cb - azul, Cr - vermelho).
    ///    Base da compressão JPEG e transmissão de vídeo (MPEG/H.264), explorando que o olho humano é menos
    ///    sensível a variações de cor do que a variações de brilho (Chroma Subsampling).
    /// 4. CMYK (Cyan, Magenta, Yellow, Key/Black): Modelo subtrativo usado na indústria gráfica de impressão.
    /// </summary>
    public static class ColorSpaces
    {
        #region Escala de Cinza (Grayscale)
        
        public enum GrayscaleMethod
        {
            /// <summary> ITU-R BT.601: Y = 0.299 R + 0.587 G + 0.114 B (Padrão NTSC/PAL de TV analógica) </summary>
            LuminanceBt601,
            /// <summary> ITU-R BT.709: Y = 0.2126 R + 0.7152 G + 0.0722 B (Padrão HDTV e sRGB moderno) </summary>
            LuminanceBt709,
            /// <summary> Média Aritmética simples: (R + G + B) / 3 </summary>
            Average,
            /// <summary> Claridade / Desaturação HSL: (max(R,G,B) + min(R,G,B)) / 2 </summary>
            Lightness,
            /// <summary> Canal Verde puro (o olho humano possui 60% dos cones sensíveis ao espectro verde) </summary>
            GreenChannelOnly
        }

        /// <summary>
        /// Converte uma cor RGB em nível de cinza (0 a 255) baseado no método escolhido.
        /// </summary>
        public static byte RgbToGrayscale(byte r, byte g, byte b, GrayscaleMethod method = GrayscaleMethod.LuminanceBt709)
        {
            switch (method)
            {
                case GrayscaleMethod.LuminanceBt601:
                    // Pesos perceptivos históricos (sensibilidade humana: Verde > Vermelho > Azul)
                    return (byte)((r * 299 + g * 587 + b * 114) / 1000);

                case GrayscaleMethod.LuminanceBt709:
                    // Pesos modernos de luminância para telas sRGB (ITU-R BT.709)
                    return (byte)((r * 2126 + g * 7152 + b * 722) / 10000);

                case GrayscaleMethod.Average:
                    return (byte)((r + g + b) / 3);

                case GrayscaleMethod.Lightness:
                    byte max = Math.Max(r, Math.Max(g, b));
                    byte min = Math.Min(r, Math.Min(g, b));
                    return (byte)((max + min) / 2);

                case GrayscaleMethod.GreenChannelOnly:
                    return g;

                default:
                    return (byte)((r * 2126 + g * 7152 + b * 722) / 10000);
            }
        }

        #endregion

        #region HSV (Hue, Saturation, Value)
        
        /// <summary>
        /// Converte RGB (0-255) para HSV:
        /// - Hue (Matiz): 0 a 360 graus no círculo cromático (0=Vermelho, 120=Verde, 240=Azul).
        /// - Saturation (Saturação): 0.0 a 1.0 (0=Cinza, 1=Cor pura).
        /// - Value (Brilho/Valor): 0.0 a 1.0 (0=Preto, 1=Brilho máximo).
        /// </summary>
        public static void RgbToHsv(byte r, byte g, byte b, out double h, out double s, out double v)
        {
            double rf = r / 255.0;
            double gf = g / 255.0;
            double bf = b / 255.0;

            double max = Math.Max(rf, Math.Max(gf, bf));
            double min = Math.Min(rf, Math.Min(gf, bf));
            double delta = max - min;

            v = max; // Value é o valor máximo entre os 3 canais

            s = max == 0 ? 0 : delta / max; // Saturação

            if (delta == 0)
            {
                h = 0; // Matiz indefinida para escala de cinza
            }
            else
            {
                if (max == rf)
                {
                    h = 60.0 * (((gf - bf) / delta) % 6);
                }
                else if (max == gf)
                {
                    h = 60.0 * (((bf - rf) / delta) + 2);
                }
                else
                {
                    h = 60.0 * (((rf - gf) / delta) + 4);
                }

                if (h < 0)
                    h += 360.0;
            }
        }

        /// <summary>
        /// Converte HSV de volta para RGB (0-255).
        /// </summary>
        public static Color HsvToRgb(double h, double s, double v, byte alpha = 255)
        {
            double c = v * s; // Croma
            double x = c * (1 - Math.Abs((h / 60.0) % 2 - 1));
            double m = v - c;

            double rf = 0, gf = 0, bf = 0;

            if (h >= 0 && h < 60) { rf = c; gf = x; bf = 0; }
            else if (h >= 60 && h < 120) { rf = x; gf = c; bf = 0; }
            else if (h >= 120 && h < 180) { rf = 0; gf = c; bf = x; }
            else if (h >= 180 && h < 240) { rf = 0; gf = x; bf = c; }
            else if (h >= 240 && h < 300) { rf = x; gf = 0; bf = c; }
            else if (h >= 300 && h <= 360) { rf = c; gf = 0; bf = x; }

            byte r = (byte)Math.Clamp((rf + m) * 255.0, 0, 255);
            byte g = (byte)Math.Clamp((gf + m) * 255.0, 0, 255);
            byte b = (byte)Math.Clamp((bf + m) * 255.0, 0, 255);

            return Color.FromArgb(alpha, r, g, b);
        }

        #endregion

        #region YCbCr (Luminance & Chrominance)
        
        /// <summary>
        /// Converte RGB para YCbCr (Padrão ITU-R BT.601 usado em JPEG).
        /// Y: 16 a 235 (ou 0 a 255 escala completa)
        /// Cb (Chroma Blue): -128 a +127 (offset 128)
        /// Cr (Chroma Red): -128 a +127 (offset 128)
        /// </summary>
        public static void RgbToYCbCr(byte r, byte g, byte b, out byte y, out byte cb, out byte cr)
        {
            double yVal = 0.299 * r + 0.587 * g + 0.114 * b;
            double cbVal = 128.0 - 0.168736 * r - 0.331264 * g + 0.5 * b;
            double crVal = 128.0 + 0.5 * r - 0.418688 * g - 0.081312 * b;

            y = (byte)Math.Clamp(yVal, 0, 255);
            cb = (byte)Math.Clamp(cbVal, 0, 255);
            cr = (byte)Math.Clamp(crVal, 0, 255);
        }

        /// <summary>
        /// Converte YCbCr de volta para RGB.
        /// </summary>
        public static Color YCbCrToRgb(byte y, byte cb, byte cr, byte alpha = 255)
        {
            double yVal = y;
            double cbVal = cb - 128.0;
            double crVal = cr - 128.0;

            double r = yVal + 1.402 * crVal;
            double g = yVal - 0.344136 * cbVal - 0.714136 * crVal;
            double b = yVal + 1.772 * cbVal;

            return Color.FromArgb(
                alpha,
                (byte)Math.Clamp(r, 0, 255),
                (byte)Math.Clamp(g, 0, 255),
                (byte)Math.Clamp(b, 0, 255)
            );
        }

        #endregion

        #region CMYK (Cyan, Magenta, Yellow, Key)
        
        /// <summary>
        /// Converte RGB para CMYK (Valores normalizados de 0.0 a 1.0).
        /// </summary>
        public static void RgbToCmyk(byte r, byte g, byte b, out double c, out double m, out double y, out double k)
        {
            double rf = r / 255.0;
            double gf = g / 255.0;
            double bf = b / 255.0;

            k = 1.0 - Math.Max(rf, Math.Max(gf, bf));

            if (Math.Abs(k - 1.0) < 1e-6)
            {
                c = 0;
                m = 0;
                y = 0;
            }
            else
            {
                c = (1.0 - rf - k) / (1.0 - k);
                m = (1.0 - gf - k) / (1.0 - k);
                y = (1.0 - bf - k) / (1.0 - k);
            }
        }

        #endregion

        #region Efeitos e Matrizes de Cor
        
        /// <summary>
        /// Aplica transformação matricial de Sépia fotográfica clássica.
        /// [R']   [0.393  0.769  0.189] [R]
        /// [G'] = [0.349  0.686  0.168] [G]
        /// [B']   [0.272  0.534  0.131] [B]
        /// </summary>
        public static Color ApplySepia(byte r, byte g, byte b, byte alpha = 255)
        {
            int tr = (int)(0.393 * r + 0.769 * g + 0.189 * b);
            int tg = (int)(0.349 * r + 0.686 * g + 0.168 * b);
            int tb = (int)(0.272 * r + 0.534 * g + 0.131 * b);

            return Color.FromArgb(
                alpha,
                (byte)Math.Min(255, tr),
                (byte)Math.Min(255, tg),
                (byte)Math.Min(255, tb)
            );
        }

        /// <summary>
        /// Inversão negativa direta de canais de cor: (255 - C).
        /// </summary>
        public static Color Invert(byte r, byte g, byte b, byte alpha = 255)
        {
            return Color.FromArgb(alpha, (byte)(255 - r), (byte)(255 - g), (byte)(255 - b));
        }

        #endregion
    }
}
