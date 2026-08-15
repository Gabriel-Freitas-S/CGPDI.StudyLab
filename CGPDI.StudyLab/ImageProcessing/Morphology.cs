using System;
using System.Threading.Tasks;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.ImageProcessing
{
    /// <summary>
    /// Tipos de Elementos Estruturantes (Structuring Elements) para Morfologia Matemática.
    /// </summary>
    public enum StructuringElementType
    {
        /// <summary> Cruz 3x3 (Vizinhança-4 de Von Neumann) </summary>
        Cross3x3,
        /// <summary> Quadrado 3x3 (Vizinhança-8 de Moore) </summary>
        Square3x3,
        /// <summary> Disco/Círculo 5x5 Euclidiano </summary>
        Disk5x5
    }

    /// <summary>
    /// Morfologia Matemática e Segmentação por Limiarização Automática em PDI.
    /// 
    /// TEORIA MORFOLÓGICA:
    /// A morfologia matemática é baseada na teoria dos conjuntos e na geometria espacial.
    /// Opera sobre imagens binárias e em escala de cinza através do probing da imagem
    /// com um elemento estruturante pré-definido B.
    /// 
    /// OPERAÇÕES PRIMITIVAS:
    /// 1. Erosão (A \ominus B): O pixel resultante é o valor MÍNIMO sob o elemento estruturante.
    /// 2. Dilatação (A \oplus B): O pixel resultante é o valor MÁXIMO sob o elemento estruturante.
    /// 3. Abertura (A \circ B = (A \ominus B) \oplus B): Suaviza contornos, elimina ilhas de ruído e pontas finas.
    /// 4. Fechamento (A \bullet B = (A \oplus B) \ominus B): Preenche pequenos buracos, fendas e une objetos próximos.
    /// </summary>
    public static class Morphology
    {
        #region Limiarização Automática (Otsu & Adaptativa)

        /// <summary>
        /// Método de Limiarização Automática de Otsu (Nobuyuki Otsu, 1979):
        /// Calcula matematicamente o limiar T* ótimo que maximiza a variância inter-classes (entre fundo e primeiro plano):
        /// \sigma_B^2(t) = \omega_0(t) \cdot \omega_1(t) \cdot [\mu_0(t) - \mu_1(t)]^2
        /// </summary>
        public static DirectBitmap OtsuThreshold(DirectBitmap src, out byte calculatedThreshold)
        {
            int width = src.Width;
            int height = src.Height;
            int total = width * height;

            // 1. Calcula o histograma de luminância
            int[] hist = new int[256];
            src.Lock();

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
                        hist[lum]++;
                    }
                }
            }

            // 2. Encontra o limiar ótimo de Otsu
            double sumTotal = 0;
            for (int t = 0; t < 256; t++)
                sumTotal += t * hist[t];

            double sumBackground = 0;
            int weightBackground = 0;
            double maxVariance = 0;
            byte bestThreshold = 128;

            for (int t = 0; t < 256; t++)
            {
                weightBackground += hist[t];
                if (weightBackground == 0) continue;

                int weightForeground = total - weightBackground;
                if (weightForeground == 0) break;

                sumBackground += t * hist[t];

                double meanBackground = sumBackground / weightBackground;
                double meanForeground = (sumTotal - sumBackground) / weightForeground;

                // Variância inter-classes de Otsu
                double varBetween = (double)weightBackground * weightForeground * Math.Pow(meanBackground - meanForeground, 2);

                if (varBetween > maxVariance)
                {
                    maxVariance = varBetween;
                    bestThreshold = (byte)t;
                }
            }

            calculatedThreshold = bestThreshold;

            // 3. Aplica a binarização com o limiar ótimo
            DirectBitmap dst = new DirectBitmap(width, height);
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        int idx = x * 4;
                        byte lum = (byte)((pSrc[idx + 2] * 299 + pSrc[idx + 1] * 587 + pSrc[idx + 0] * 114) / 1000);
                        byte binVal = (byte)(lum >= bestThreshold ? 255 : 0);

                        pDst[idx + 0] = binVal;
                        pDst[idx + 1] = binVal;
                        pDst[idx + 2] = binVal;
                        pDst[idx + 3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Limiarização Adaptativa Local (Local Adaptive Thresholding):
        /// Calcula o limiar de cada pixel como a média de sua vizinhança local (janela WxW) menos uma constante C.
        /// Ideal para documentos ou imagens com iluminação não-uniforme (sombras graduais).
        /// </summary>
        public static DirectBitmap AdaptiveThreshold(DirectBitmap src, int windowSize = 15, int c = 5)
        {
            if (windowSize % 2 == 0) windowSize++;
            int radius = windowSize / 2;
            int width = src.Width;
            int height = src.Height;

            DirectBitmap dst = new DirectBitmap(width, height);
            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        int sum = 0;
                        int count = 0;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            int py = Math.Clamp(y + ky, 0, height - 1);
                            byte* sRow = src.BackBuffer + (py * src.Stride);

                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int px = Math.Clamp(x + kx, 0, width - 1);
                                byte* p = sRow + (px * 4);
                                sum += (p[2] * 299 + p[1] * 587 + p[0] * 114) / 1000;
                                count++;
                            }
                        }

                        double localMean = (double)sum / count;
                        byte* curr = src.BackBuffer + (y * src.Stride) + (x * 4);
                        byte currLum = (byte)((curr[2] * 299 + curr[1] * 587 + curr[0] * 114) / 1000);

                        byte bin = (byte)(currLum > (localMean - c) ? 255 : 0);

                        pDst[x * 4 + 0] = bin;
                        pDst[x * 4 + 1] = bin;
                        pDst[x * 4 + 2] = bin;
                        pDst[x * 4 + 3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Operadores Morfológicos (Erosão, Dilatação, Abertura, Fechamento)

        private static bool[,] GetStructuringElement(StructuringElementType type, out int radius)
        {
            switch (type)
            {
                case StructuringElementType.Cross3x3:
                    radius = 1;
                    return new bool[3, 3]
                    {
                        { false, true,  false },
                        { true,  true,  true  },
                        { false, true,  false }
                    };

                case StructuringElementType.Square3x3:
                    radius = 1;
                    return new bool[3, 3]
                    {
                        { true, true, true },
                        { true, true, true },
                        { true, true, true }
                    };

                case StructuringElementType.Disk5x5:
                    radius = 2;
                    return new bool[5, 5]
                    {
                        { false, true, true, true, false },
                        { true,  true, true, true, true  },
                        { true,  true, true, true, true  },
                        { true,  true, true, true, true  },
                        { false, true, true, true, false }
                    };

                default:
                    radius = 1;
                    return new bool[3, 3] { { true, true, true }, { true, true, true }, { true, true, true } };
            }
        }

        /// <summary>
        /// Erosão Morfológica (Erosion - A \ominus B):
        /// Substitui cada pixel pelo valor MÍNIMO presente sob o elemento estruturante B.
        /// Efeito: Encolhe áreas claras (objetos) e expande áreas escuras (fundo).
        /// </summary>
        public static DirectBitmap Erosion(DirectBitmap src, StructuringElementType element = StructuringElementType.Square3x3)
        {
            bool[,] se = GetStructuringElement(element, out int radius);
            int width = src.Width;
            int height = src.Height;

            DirectBitmap dst = new DirectBitmap(width, height);
            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        byte minB = 255, minG = 255, minR = 255;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            int py = Math.Clamp(y + ky, 0, height - 1);
                            byte* sRow = src.BackBuffer + (py * src.Stride);

                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                if (!se[ky + radius, kx + radius]) continue;

                                int px = Math.Clamp(x + kx, 0, width - 1);
                                byte* p = sRow + (px * 4);

                                if (p[0] < minB) minB = p[0];
                                if (p[1] < minG) minG = p[1];
                                if (p[2] < minR) minR = p[2];
                            }
                        }

                        pDst[x * 4 + 0] = minB;
                        pDst[x * 4 + 1] = minG;
                        pDst[x * 4 + 2] = minR;
                        pDst[x * 4 + 3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Dilatação Morfológica (Dilation - A \oplus B):
        /// Substitui cada pixel pelo valor MÁXIMO presente sob o elemento estruturante B.
        /// Efeito: Expande áreas claras (objetos) e encolhe áreas escuras (fundo).
        /// </summary>
        public static DirectBitmap Dilation(DirectBitmap src, StructuringElementType element = StructuringElementType.Square3x3)
        {
            bool[,] se = GetStructuringElement(element, out int radius);
            int width = src.Width;
            int height = src.Height;

            DirectBitmap dst = new DirectBitmap(width, height);
            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        byte maxB = 0, maxG = 0, maxR = 0;

                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            int py = Math.Clamp(y + ky, 0, height - 1);
                            byte* sRow = src.BackBuffer + (py * src.Stride);

                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                if (!se[ky + radius, kx + radius]) continue;

                                int px = Math.Clamp(x + kx, 0, width - 1);
                                byte* p = sRow + (px * 4);

                                if (p[0] > maxB) maxB = p[0];
                                if (p[1] > maxG) maxG = p[1];
                                if (p[2] > maxR) maxR = p[2];
                            }
                        }

                        pDst[x * 4 + 0] = maxB;
                        pDst[x * 4 + 1] = maxG;
                        pDst[x * 4 + 2] = maxR;
                        pDst[x * 4 + 3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Abertura Morfológica (Opening - (A \ominus B) \oplus B):
        /// Erosão seguida de Dilatação. Remove pequenas saliências e ilhas de ruído brilhantes menores que o elemento.
        /// </summary>
        public static DirectBitmap Opening(DirectBitmap src, StructuringElementType element = StructuringElementType.Square3x3)
        {
            using (DirectBitmap eroded = Erosion(src, element))
            {
                return Dilation(eroded, element);
            }
        }

        /// <summary>
        /// Fechamento Morfológico (Closing - (A \oplus B) \ominus B):
        /// Dilatação seguida de Erosão. Preenche pequenos buracos escuros, fendas e conecta componentes vizinhos.
        /// </summary>
        public static DirectBitmap Closing(DirectBitmap src, StructuringElementType element = StructuringElementType.Square3x3)
        {
            using (DirectBitmap dilated = Dilation(src, element))
            {
                return Erosion(dilated, element);
            }
        }

        /// <summary>
        /// Gradiente Morfológico (Morphological Gradient - Dilation - Erosion):
        /// Subtrai a erosão da dilatação da imagem. Destaca os contornos e bordas exatas dos objetos.
        /// </summary>
        public static DirectBitmap MorphologicalGradient(DirectBitmap src, StructuringElementType element = StructuringElementType.Square3x3)
        {
            using (DirectBitmap dilated = Dilation(src, element))
            using (DirectBitmap eroded = Erosion(src, element))
            {
                DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
                dilated.Lock();
                eroded.Lock();
                dst.Lock();

                unsafe
                {
                    Parallel.For(0, src.Height, y =>
                    {
                        byte* pDil = dilated.BackBuffer + (y * dilated.Stride);
                        byte* pEro = eroded.BackBuffer + (y * eroded.Stride);
                        byte* pDst = dst.BackBuffer + (y * dst.Stride);

                        for (int x = 0; x < src.Width; x++)
                        {
                            int idx = x * 4;
                            pDst[idx + 0] = (byte)Math.Clamp(pDil[idx + 0] - pEro[idx + 0], 0, 255);
                            pDst[idx + 1] = (byte)Math.Clamp(pDil[idx + 1] - pEro[idx + 1], 0, 255);
                            pDst[idx + 2] = (byte)Math.Clamp(pDil[idx + 2] - pEro[idx + 2], 0, 255);
                            pDst[idx + 3] = 255;
                        }
                    });
                }

                dilated.Unlock(false);
                eroded.Unlock(false);
                dst.Unlock(true);
                return dst;
            }
        }

        /// <summary>
        /// Top-Hat (Cartola Branca - Original - Opening):
        /// Isola elementos brilhantes e picos de intensidade que sejam menores que o elemento estruturante.
        /// </summary>
        public static DirectBitmap TopHat(DirectBitmap src, StructuringElementType element = StructuringElementType.Square3x3)
        {
            using (DirectBitmap opened = Opening(src, element))
            {
                DirectBitmap dst = new DirectBitmap(src.Width, src.Height);
                src.Lock();
                opened.Lock();
                dst.Lock();

                unsafe
                {
                    Parallel.For(0, src.Height, y =>
                    {
                        byte* pSrc = src.BackBuffer + (y * src.Stride);
                        byte* pOpn = opened.BackBuffer + (y * opened.Stride);
                        byte* pDst = dst.BackBuffer + (y * dst.Stride);

                        for (int x = 0; x < src.Width; x++)
                        {
                            int idx = x * 4;
                            pDst[idx + 0] = (byte)Math.Clamp(pSrc[idx + 0] - pOpn[idx + 0], 0, 255);
                            pDst[idx + 1] = (byte)Math.Clamp(pSrc[idx + 1] - pOpn[idx + 1], 0, 255);
                            pDst[idx + 2] = (byte)Math.Clamp(pSrc[idx + 2] - pOpn[idx + 2], 0, 255);
                            pDst[idx + 3] = 255;
                        }
                    });
                }

                src.Unlock(false);
                opened.Unlock(false);
                dst.Unlock(true);
                return dst;
            }
        }

        #endregion
    }
}
