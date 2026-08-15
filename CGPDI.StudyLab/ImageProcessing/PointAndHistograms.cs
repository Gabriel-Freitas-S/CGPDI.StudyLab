using System;
using System.Threading.Tasks;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.ImageProcessing
{
    /// <summary>
    /// Operações Pontuais (Point Operations) e Processamento de Histograma em PDI.
    /// 
    /// TEORIA DAS OPERAÇÕES PONTUAIS:
    /// Uma transformação pontual mapeia a intensidade de um único pixel f(x, y) para g(x, y)
    /// independentemente de seus vizinhos: g(x, y) = T[f(x, y)].
    /// Exemplos: Brilho, Contraste, Correção Gamma, Binarização, Posterização e Equalização de Histograma.
    /// </summary>
    public static class PointAndHistograms
    {
        #region Operações Pontuais de Cor e Brilho

        /// <summary>
        /// Ajuste Linear de Brilho: Soma uma constante aditiva beta a cada canal de cor.
        /// g(x, y) = clamp(f(x, y) + beta, 0, 255)
        /// </summary>
        public static DirectBitmap AdjustBrightness(DirectBitmap src, int delta)
        {
            DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
            src.Lock();
            dst.Lock();

            // Tabela Look-Up Table (LUT) para evitar cálculos repetitivos por pixel: O(256) em vez de O(W*H)
            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                lut[i] = (byte)Math.Clamp(i + delta, 0, 255);
            }

            unsafe
            {
                Parallel.For(0, src.Height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * 4;
                        pDst[idx + 0] = lut[pSrc[idx + 0]]; // B
                        pDst[idx + 1] = lut[pSrc[idx + 1]]; // G
                        pDst[idx + 2] = lut[pSrc[idx + 2]]; // R
                        pDst[idx + 3] = pSrc[idx + 3];     // A
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Ajuste de Contraste Linear em torno do pivô de cinza médio (128):
        /// g(x, y) = clamp(alpha * (f(x, y) - 128) + 128, 0, 255)
        /// - alpha > 1: Expande a faixa dinâmica (aumenta contraste).
        /// - alpha < 1: Comprime a faixa dinâmica (diminui contraste).
        /// </summary>
        public static DirectBitmap AdjustContrast(DirectBitmap src, double contrastFactor)
        {
            DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
            src.Lock();
            dst.Lock();

            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                double val = contrastFactor * (i - 128.0) + 128.0;
                lut[i] = (byte)Math.Clamp(val, 0, 255);
            }

            unsafe
            {
                Parallel.For(0, src.Height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * 4;
                        pDst[idx + 0] = lut[pSrc[idx + 0]];
                        pDst[idx + 1] = lut[pSrc[idx + 1]];
                        pDst[idx + 2] = lut[pSrc[idx + 2]];
                        pDst[idx + 3] = pSrc[idx + 3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Correção Gamma (Transformação de Potência / Power-Law):
        /// s = c * r^gamma  (com r normalizado em [0, 1])
        /// - gamma &lt; 1: Clareia tons escuros e médios (expande sombras sem estourar realces).
        /// - gamma &gt; 1: Escurece a imagem e aumenta saturação perceptiva.
        /// Fundamental para compensar a resposta não-linear de tubos CRT e painéis OLED/LCD.
        /// </summary>
        public static DirectBitmap AdjustGamma(DirectBitmap src, double gamma)
        {
            DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
            src.Lock();
            dst.Lock();

            byte[] lut = new byte[256];
            double invGamma = 1.0 / Math.Max(0.01, gamma);

            for (int i = 0; i < 256; i++)
            {
                double normalized = i / 255.0;
                double corrected = Math.Pow(normalized, invGamma);
                lut[i] = (byte)Math.Clamp(corrected * 255.0, 0, 255);
            }

            unsafe
            {
                Parallel.For(0, src.Height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * 4;
                        pDst[idx + 0] = lut[pSrc[idx + 0]];
                        pDst[idx + 1] = lut[pSrc[idx + 1]];
                        pDst[idx + 2] = lut[pSrc[idx + 2]];
                        pDst[idx + 3] = pSrc[idx + 3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Posterização (Quantização de Níveis de Cor):
        /// Reduz a profundidade tonal contínua para N níveis discretos, criando visual artístico/estilizado.
        /// </summary>
        public static DirectBitmap Posterize(DirectBitmap src, int levels = 4)
        {
            levels = Math.Clamp(levels, 2, 64);
            DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
            src.Lock();
            dst.Lock();

            byte[] lut = new byte[256];
            double step = 255.0 / (levels - 1);
            for (int i = 0; i < 256; i++)
            {
                int bucket = (int)Math.Round((i / 255.0) * (levels - 1));
                lut[i] = (byte)Math.Clamp(bucket * step, 0, 255);
            }

            unsafe
            {
                Parallel.For(0, src.Height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * 4;
                        pDst[idx + 0] = lut[pSrc[idx + 0]];
                        pDst[idx + 1] = lut[pSrc[idx + 1]];
                        pDst[idx + 2] = lut[pSrc[idx + 2]];
                        pDst[idx + 3] = pSrc[idx + 3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Solarização (Efeito Sabattier):
        /// Inverte os pixels cuja intensidade ultrapassa um limiar específico, criando visual surrealista.
        /// </summary>
        public static DirectBitmap Solarize(DirectBitmap src, byte threshold = 128)
        {
            DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
            src.Lock();
            dst.Lock();

            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                lut[i] = (byte)(i > threshold ? 255 - i : i);
            }

            unsafe
            {
                Parallel.For(0, src.Height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * 4;
                        pDst[idx + 0] = lut[pSrc[idx + 0]];
                        pDst[idx + 1] = lut[pSrc[idx + 1]];
                        pDst[idx + 2] = lut[pSrc[idx + 2]];
                        pDst[idx + 3] = pSrc[idx + 3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Histograma e Equalização

        /// <summary>
        /// Calcula os histogramas de intensidade para os canais Vermelho, Verde, Azul e Luminância (0 a 255).
        /// </summary>
        public static void CalculateHistograms(DirectBitmap src, out int[] histR, out int[] histG, out int[] histB, out int[] histLum)
        {
            histR = new int[256];
            histG = new int[256];
            histB = new int[256];
            histLum = new int[256];

            int[] localR = histR;
            int[] localG = histG;
            int[] localB = histB;
            int[] localLum = histLum;

            src.Lock();

            unsafe
            {
                byte* buf = src.BackBuffer;
                int stride = src.Stride;
                int width = src.Width;
                int height = src.Height;

                for (int y = 0; y < height; y++)
                {
                    byte* row = buf + (y * stride);
                    for (int x = 0; x < width; x++)
                    {
                        byte b = row[x * 4 + 0];
                        byte g = row[x * 4 + 1];
                        byte r = row[x * 4 + 2];
                        byte lum = (byte)((r * 2126 + g * 7152 + b * 722) / 10000);

                        localB[b]++;
                        localG[g]++;
                        localR[r]++;
                        localLum[lum]++;
                    }
                }
            }

            src.Unlock(false);
        }

        /// <summary>
        /// Equalização Global de Histograma (Histogram Equalization):
        /// Transforma a distribuição de intensidades para que a Função de Distribuição Acumulada (CDF)
        /// torne-se linear, maximizando o contraste global da imagem.
        /// 
        /// FÓRMULA MATEMÁTICA:
        /// CDF(i) = \sum_{j=0}^{i} p(j) = \sum_{j=0}^{i} \frac{n_j}{N}
        /// h_eq(v) = round( \frac{CDF(v) - CDF_min}{N - CDF_min} \times (L - 1) )
        /// </summary>
        public static DirectBitmap EqualizeHistogram(DirectBitmap src, bool equalizeInHsv = true)
        {
            int width = src.Width;
            int height = src.Height;
            int totalPixels = width * height;

            DirectBitmap dst = new DirectBitmap(width, height);
            src.Lock();
            dst.Lock();

            if (equalizeInHsv)
            {
                // Equaliza apenas o canal de Luminância/Valor (V no HSV ou Y no YCbCr) para não distorcer o matiz cromático!
                int[] histV = new int[256];

                unsafe
                {
                    for (int y = 0; y < height; y++)
                    {
                        byte* row = src.BackBuffer + (y * src.Stride);
                        for (int x = 0; x < width; x++)
                        {
                            byte b = row[x * 4 + 0];
                            byte g = row[x * 4 + 1];
                            byte r = row[x * 4 + 2];
                            byte lum = (byte)((r * 299 + g * 587 + b * 114) / 1000);
                            histV[lum]++;
                        }
                    }
                }

                // Cálculo da CDF (Função de Distribuição Acumulada)
                int[] cdf = new int[256];
                cdf[0] = histV[0];
                int cdfMin = cdf[0] > 0 ? cdf[0] : 0;

                for (int i = 1; i < 256; i++)
                {
                    cdf[i] = cdf[i - 1] + histV[i];
                    if (cdfMin == 0 && cdf[i] > 0)
                        cdfMin = cdf[i];
                }

                // Mapeamento equalizado
                byte[] lut = new byte[256];
                for (int i = 0; i < 256; i++)
                {
                    if (totalPixels == cdfMin)
                        lut[i] = (byte)i;
                    else
                        lut[i] = (byte)Math.Clamp(Math.Round(((double)(cdf[i] - cdfMin) / (totalPixels - cdfMin)) * 255.0), 0, 255);
                }

                // Aplica preservando as proporções de cor RGB
                unsafe
                {
                    Parallel.For(0, height, y =>
                    {
                        byte* pSrc = src.BackBuffer + (y * src.Stride);
                        byte* pDst = dst.BackBuffer + (y * dst.Stride);

                        for (int x = 0; x < width; x++)
                        {
                            int idx = x * 4;
                            byte b = pSrc[idx + 0];
                            byte g = pSrc[idx + 1];
                            byte r = pSrc[idx + 2];

                            byte lum = (byte)((r * 299 + g * 587 + b * 114) / 1000);
                            byte newLum = lut[lum];

                            double scale = lum == 0 ? 1.0 : (double)newLum / lum;

                            pDst[idx + 0] = (byte)Math.Clamp(b * scale, 0, 255);
                            pDst[idx + 1] = (byte)Math.Clamp(g * scale, 0, 255);
                            pDst[idx + 2] = (byte)Math.Clamp(r * scale, 0, 255);
                            pDst[idx + 3] = pSrc[idx + 3];
                        }
                    });
                }
            }
            else
            {
                // Equalização de histograma independente canal a canal (R, G, B)
                CalculateHistograms(src, out int[] hR, out int[] hG, out int[] hB, out _);

                byte[] lutR = ComputeCdfLut(hR, totalPixels);
                byte[] lutG = ComputeCdfLut(hG, totalPixels);
                byte[] lutB = ComputeCdfLut(hB, totalPixels);

                unsafe
                {
                    Parallel.For(0, height, y =>
                    {
                        byte* pSrc = src.BackBuffer + (y * src.Stride);
                        byte* pDst = dst.BackBuffer + (y * dst.Stride);

                        for (int x = 0; x < width; x++)
                        {
                            int idx = x * 4;
                            pDst[idx + 0] = lutB[pSrc[idx + 0]];
                            pDst[idx + 1] = lutG[pSrc[idx + 1]];
                            pDst[idx + 2] = lutR[pSrc[idx + 2]];
                            pDst[idx + 3] = pSrc[idx + 3];
                        }
                    });
                }
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        private static byte[] ComputeCdfLut(int[] hist, int totalPixels)
        {
            int[] cdf = new int[256];
            cdf[0] = hist[0];
            int cdfMin = cdf[0] > 0 ? cdf[0] : 0;

            for (int i = 1; i < 256; i++)
            {
                cdf[i] = cdf[i - 1] + hist[i];
                if (cdfMin == 0 && cdf[i] > 0)
                    cdfMin = cdf[i];
            }

            byte[] lut = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                if (totalPixels == cdfMin)
                    lut[i] = (byte)i;
                else
                    lut[i] = (byte)Math.Clamp(Math.Round(((double)(cdf[i] - cdfMin) / (totalPixels - cdfMin)) * 255.0), 0, 255);
            }
            return lut;
        }

        /// <summary>
        /// Expansão de Contraste (Contrast Stretching / Normalização Min-Max):
        /// Mapeia o menor valor presente na imagem (f_min) para 0 e o maior (f_max) para 255.
        /// g(x, y) = ((f(x, y) - f_min) / (f_max - f_min)) * 255
        /// </summary>
        public static DirectBitmap ContrastStretching(DirectBitmap src)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            src.Lock();
            dst.Lock();

            byte minR = 255, maxR = 0;
            byte minG = 255, maxG = 0;
            byte minB = 255, maxB = 0;

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* row = src.BackBuffer + (y * src.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        byte b = row[x * 4 + 0];
                        byte g = row[x * 4 + 1];
                        byte r = row[x * 4 + 2];

                        if (r < minR) minR = r; if (r > maxR) maxR = r;
                        if (g < minG) minG = g; if (g > maxG) maxG = g;
                        if (b < minB) minB = b; if (b > maxB) maxB = b;
                    }
                }

                // Cria LUTs normalizadas
                byte[] lutR = CreateStretchLut(minR, maxR);
                byte[] lutG = CreateStretchLut(minG, maxG);
                byte[] lutB = CreateStretchLut(minB, maxB);

                Parallel.For(0, height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        int idx = x * 4;
                        pDst[idx + 0] = lutB[pSrc[idx + 0]];
                        pDst[idx + 1] = lutG[pSrc[idx + 1]];
                        pDst[idx + 2] = lutR[pSrc[idx + 2]];
                        pDst[idx + 3] = pSrc[idx + 3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        private static byte[] CreateStretchLut(byte min, byte max)
        {
            byte[] lut = new byte[256];
            double range = max - min;
            if (range <= 0) range = 1.0;

            for (int i = 0; i < 256; i++)
            {
                double norm = (i - min) / range;
                lut[i] = (byte)Math.Clamp(norm * 255.0, 0, 255);
            }
            return lut;
        }

        #endregion
    }
}
