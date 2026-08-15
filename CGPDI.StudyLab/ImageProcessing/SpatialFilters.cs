using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.ImageProcessing
{
    /// <summary>
    /// Filtros Espaciais e Convoluções Lineares/Não-Lineares em Processamento Digital de Imagens (PDI).
    /// 
    /// TEORIA DA CONVOLUÇÃO ESPACIAL 2D:
    /// A convolução discreta em um pixel (x, y) com uma máscara/kernel K de tamanho (2k+1)x(2k+1) é dada por:
    /// g(x, y) = \sum_{u=-k}^{k} \sum_{v=-k}^{k} f(x - u, y - v) \cdot K(u, v)
    /// 
    /// Onde:
    /// - f(x, y) é a imagem de entrada.
    /// - K(u, v) é a matriz de pesos (kernel/máscara).
    /// - g(x, y) é a imagem resultante filtrada.
    /// </summary>
    public static class SpatialFilters
    {
        #region Convolução Genérica 2D

        /// <summary>
        /// Aplica uma matriz de convolução 2D genérica de dimensões arbitrárias ímpares (3x3, 5x5, 7x7, etc.).
        /// Possui suporte a divisor de normalização, bias (deslocamento de tom) e tratamento de bordas (Clamp).
        /// </summary>
        public static DirectBitmap Convolve2D(DirectBitmap src, double[,] kernel, double divisor = 1.0, double bias = 0.0)
        {
            int width = src.Width;
            int height = src.Height;
            int kRows = kernel.GetLength(0);
            int kCols = kernel.GetLength(1);
            int kRadiusY = kRows / 2;
            int kRadiusX = kCols / 2;

            if (Math.Abs(divisor) < 1e-7)
                divisor = 1.0;

            DirectBitmap dst = new DirectBitmap(width, height);
            src.Lock();
            dst.Lock();

            unsafe
            {
                byte* srcBuf = src.BackBuffer;
                int srcStride = src.Stride;
                byte* dstBuf = dst.BackBuffer;
                int dstStride = dst.Stride;

                Parallel.For(0, height, y =>
                {
                    byte* dstRow = dstBuf + (y * dstStride);

                    for (int x = 0; x < width; x++)
                    {
                        double sumB = 0, sumG = 0, sumR = 0;

                        for (int ky = -kRadiusY; ky <= kRadiusY; ky++)
                        {
                            // Tratamento de borda: Clamp (espelhamento no limite mais próximo)
                            int py = Math.Clamp(y + ky, 0, height - 1);
                            byte* srcRow = srcBuf + (py * srcStride);

                            for (int kx = -kRadiusX; kx <= kRadiusX; kx++)
                            {
                                int px = Math.Clamp(x + kx, 0, width - 1);
                                byte* p = srcRow + (px * 4);

                                double weight = kernel[ky + kRadiusY, kx + kRadiusX];
                                sumB += p[0] * weight;
                                sumG += p[1] * weight;
                                sumR += p[2] * weight;
                            }
                        }

                        byte b = (byte)Math.Clamp((sumB / divisor) + bias, 0, 255);
                        byte g = (byte)Math.Clamp((sumG / divisor) + bias, 0, 255);
                        byte r = (byte)Math.Clamp((sumR / divisor) + bias, 0, 255);
                        byte a = 255;

                        byte* dstPixel = dstRow + (x * 4);
                        dstPixel[0] = b;
                        dstPixel[1] = g;
                        dstPixel[2] = r;
                        dstPixel[3] = a;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Filtros Passa-Baixa (Suavização / Desfoque)

        /// <summary>
        /// Filtro da Média (Box Blur): Cada pixel torna-se a média aritmética simples de seus vizinhos.
        /// Atenua ruído de alta frequência, mas suaviza bordas nítidas.
        /// </summary>
        public static DirectBitmap BoxBlur(DirectBitmap src, int size = 3)
        {
            if (size % 2 == 0) size++; // Garante tamanho ímpar
            double[,] kernel = new double[size, size];
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    kernel[r, c] = 1.0;

            return Convolve2D(src, kernel, size * size, 0.0);
        }

        /// <summary>
        /// Filtro Gaussiano 2D: Pondera a vizinhança pela distribuição normal bidimensional:
        /// G(x, y) = (1 / 2*pi*sigma^2) * e^(-(x^2 + y^2) / (2*sigma^2))
        /// Preserva melhor os gradientes naturais da imagem em comparação ao Box Blur.
        /// </summary>
        public static DirectBitmap GaussianBlur(DirectBitmap src, double sigma = 1.4, int size = 5)
        {
            if (size % 2 == 0) size++;
            int radius = size / 2;
            double[,] kernel = new double[size, size];
            double sum = 0.0;
            double twoSigmaSq = 2.0 * sigma * sigma;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    double val = Math.Exp(-(x * x + y * y) / twoSigmaSq);
                    kernel[y + radius, x + radius] = val;
                    sum += val;
                }
            }

            return Convolve2D(src, kernel, sum, 0.0);
        }

        #endregion

        #region Filtros Passa-Alta e Realce de Nitidez (Sharpening)

        /// <summary>
        /// Realce de Nitidez (Sharpen): Amplifica as altas frequências (bordas e detalhes finos)
        /// subtraindo da imagem original a sua componente de baixa frequência.
        /// Kernel clássico Laplaciano com ganho central positivo.
        /// </summary>
        public static DirectBitmap Sharpen(DirectBitmap src, double strength = 1.0)
        {
            // Kernel 3x3 de Nitidez Laplaciano:
            // [  0, -1,  0 ]
            // [ -1, 4+k, -1 ]
            // [  0, -1,  0 ]
            double k = Math.Max(0.1, strength);
            double[,] kernel = new double[3, 3]
            {
                {  0, -k,  0 },
                { -k, 4*k + 1.0, -k },
                {  0, -k,  0 }
            };

            return Convolve2D(src, kernel, 1.0, 0.0);
        }

        /// <summary>
        /// Máscara de Desfoque (Unsharp Masking): Técnica profissional da fotografia analógica e digital.
        /// 1. Cria versão desfocada da imagem (Passa-Baixa).
        /// 2. Subtrai a versão desfocada da original para obter apenas a máscara de bordas (High-Pass).
        /// 3. Soma a máscara multiplicada por uma intensidade à imagem original: Result = Src + Amount * (Src - Blur).
        /// </summary>
        public static DirectBitmap UnsharpMask(DirectBitmap src, double sigma = 1.5, double amount = 1.5)
        {
            DirectBitmap blurred = GaussianBlur(src, sigma, 5);
            DirectBitmap dst = new DirectBitmap(src.Width, src.Height);

            src.Lock();
            blurred.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, src.Height, y =>
                {
                    byte* pSrc = src.BackBuffer + (y * src.Stride);
                    byte* pBlur = blurred.BackBuffer + (y * blurred.Stride);
                    byte* pDst = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < src.Width; x++)
                    {
                        int idx = x * 4;
                        for (int c = 0; c < 3; c++) // B, G, R
                        {
                            double diff = pSrc[idx + c] - pBlur[idx + c];
                            double val = pSrc[idx + c] + amount * diff;
                            pDst[idx + c] = (byte)Math.Clamp(val, 0, 255);
                        }
                        pDst[idx + 3] = 255; // Alpha
                    }
                });
            }

            src.Unlock(false);
            blurred.Unlock(false);
            blurred.Dispose();
            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Detectores de Bordas e Gradientes

        /// <summary>
        /// Operador Sobel: Calcula a aproximação discreta do gradiente da imagem em duas direções ortogonais:
        /// Gx (gradiente horizontal) e Gy (gradiente vertical).
        /// Magnitude do Gradiente: G = sqrt(Gx^2 + Gy^2)
        /// Direção do Gradiente: theta = atan2(Gy, Gx)
        /// </summary>
        public static DirectBitmap Sobel(DirectBitmap src, bool magnitude = true, bool horizontalOnly = false, bool verticalOnly = false)
        {
            double[,] kx = new double[3, 3]
            {
                { -1, 0, 1 },
                { -2, 0, 2 },
                { -1, 0, 1 }
            };

            double[,] ky = new double[3, 3]
            {
                { -1, -2, -1 },
                {  0,  0,  0 },
                {  1,  2,  1 }
            };

            if (horizontalOnly) return Convolve2D(src, kx, 1.0, 128.0);
            if (verticalOnly) return Convolve2D(src, ky, 1.0, 128.0);

            // Calcula magnitude completa do gradiente euclidiano
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            src.Lock();
            dst.Lock();

            unsafe
            {
                byte* srcBuf = src.BackBuffer;
                int srcStride = src.Stride;
                byte* dstBuf = dst.BackBuffer;
                int dstStride = dst.Stride;

                Parallel.For(1, height - 1, y =>
                {
                    byte* dstRow = dstBuf + (y * dstStride);

                    for (int x = 1; x < width - 1; x++)
                    {
                        double gx = 0, gy = 0;

                        for (int kyIdx = -1; kyIdx <= 1; kyIdx++)
                        {
                            byte* sRow = srcBuf + ((y + kyIdx) * srcStride);
                            for (int kxIdx = -1; kxIdx <= 1; kxIdx++)
                            {
                                byte* p = sRow + ((x + kxIdx) * 4);
                                // Converte para luminância em tempo de execução
                                double lum = 0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0];
                                gx += lum * kx[kyIdx + 1, kxIdx + 1];
                                gy += lum * ky[kyIdx + 1, kxIdx + 1];
                            }
                        }

                        byte mag = (byte)Math.Clamp(Math.Sqrt(gx * gx + gy * gy), 0, 255);
                        byte* pDst = dstRow + (x * 4);
                        pDst[0] = mag;
                        pDst[1] = mag;
                        pDst[2] = mag;
                        pDst[3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Operador Scharr: Otimizado numericamente para máxima simetria rotacional em gradientes 3x3.
        /// Reduz o erro angular do Sobel clássico de 10 graus para menos de 1 grau.
        /// </summary>
        public static DirectBitmap Scharr(DirectBitmap src)
        {
            double[,] kx = new double[3, 3]
            {
                { -3,  0,  3 },
                { -10, 0, 10 },
                { -3,  0,  3 }
            };

            double[,] ky = new double[3, 3]
            {
                { -3, -10, -3 },
                {  0,   0,  0 },
                {  3,  10,  3 }
            };

            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(1, height - 1, y =>
                {
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);
                    for (int x = 1; x < width - 1; x++)
                    {
                        double gx = 0, gy = 0;
                        for (int kyIdx = -1; kyIdx <= 1; kyIdx++)
                        {
                            byte* sRow = src.BackBuffer + ((y + kyIdx) * src.Stride);
                            for (int kxIdx = -1; kxIdx <= 1; kxIdx++)
                            {
                                byte* p = sRow + ((x + kxIdx) * 4);
                                double lum = 0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0];
                                gx += lum * kx[kyIdx + 1, kxIdx + 1];
                                gy += lum * ky[kyIdx + 1, kxIdx + 1];
                            }
                        }

                        // Normalização (16 é a soma positiva do kernel de Scharr)
                        byte mag = (byte)Math.Clamp(Math.Sqrt(gx * gx + gy * gy) / 4.0, 0, 255);
                        byte* pDst = dstRow + (x * 4);
                        pDst[0] = mag; pDst[1] = mag; pDst[2] = mag; pDst[3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Operador Laplaciano: Segunda derivada espacial (\nabla^2 f = d^2f/dx^2 + d^2f/dy^2).
        /// Detecta bordas através do cruzamento por zero (zero-crossing), sendo isotrópico (invariante a rotação).
        /// </summary>
        public static DirectBitmap Laplacian(DirectBitmap src, bool eightNeighborhood = true)
        {
            double[,] kernel = eightNeighborhood
                ? new double[3, 3] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } }
                : new double[3, 3] { {  0, -1,  0 }, { -1, 4, -1 }, {  0, -1,  0 } };

            return Convolve2D(src, kernel, 1.0, 0.0);
        }

        /// <summary>
        /// Laplaciano do Gaussiano (LoG / Filtro Mexican Hat):
        /// Combina suavização Gaussiana prévia para remover ruídos com o operador Laplaciano de segunda derivada.
        /// LoG(x, y) = -1/(pi*sigma^4) * [1 - (x^2+y^2)/(2*sigma^2)] * e^(-(x^2+y^2)/(2*sigma^2))
        /// </summary>
        public static DirectBitmap LaplacianOfGaussian(DirectBitmap src)
        {
            double[,] log5x5 = new double[5, 5]
            {
                {  0,  0, -1,  0,  0 },
                {  0, -1, -2, -1,  0 },
                { -1, -2, 16, -2, -1 },
                {  0, -1, -2, -1,  0 },
                {  0,  0, -1,  0,  0 }
            };

            return Convolve2D(src, log5x5, 1.0, 0.0);
        }

        /// <summary>
        /// Algoritmo Canny Edge Detector Completo:
        /// Considerado o padrão-ouro de detecção de bordas na Computação Gráfica e Visão Computacional.
        /// 
        /// PIPELINE EM 5 ETAPAS:
        /// 1. Suavização Gaussiana (reduz ruído que causaria falsas bordas)
        /// 2. Cálculo do Gradiente e Ângulo (Sobel Gx, Gy, Mag, Theta)
        /// 3. Supressão de Não-Máximos (NMS) - afina as bordas para largura de 1 pixel
        /// 4. Limiarização Dupla (Double Threshold) com limiar forte e limiar fraco
        /// 5. Rastreamento de Bordas por Histerese (mantém bordas fracas conectadas a fortes).
        /// </summary>
        public static DirectBitmap CannyEdgeDetector(DirectBitmap src, double lowThreshold = 25.0, double highThreshold = 65.0)
        {
            int width = src.Width;
            int height = src.Height;

            // 1. Suavização Gaussiana 5x5
            DirectBitmap smooth = GaussianBlur(src, 1.4, 5);

            // Buffers de gradiente e ângulo
            double[] magnitude = new double[width * height];
            double[] angle = new double[width * height];

            smooth.Lock();

            unsafe
            {
                byte* sBuf = smooth.BackBuffer;
                int sStride = smooth.Stride;

                // 2. Gradientes Sobel
                Parallel.For(1, height - 1, y =>
                {
                    for (int x = 1; x < width - 1; x++)
                    {
                        double gx = 0, gy = 0;

                        // Sobel Gx e Gy
                        for (int ky = -1; ky <= 1; ky++)
                        {
                            byte* row = sBuf + ((y + ky) * sStride);
                            for (int kx = -1; kx <= 1; kx++)
                            {
                                byte* p = row + ((x + kx) * 4);
                                double lum = 0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0];
                                
                                int wx = (kx == 0 ? 0 : (kx < 0 ? -1 : 1)) * (ky == 0 ? 2 : 1);
                                int wy = (ky == 0 ? 0 : (ky < 0 ? -1 : 1)) * (kx == 0 ? 2 : 1);

                                gx += lum * wx;
                                gy += lum * wy;
                            }
                        }

                        int idx = y * width + x;
                        magnitude[idx] = Math.Sqrt(gx * gx + gy * gy);
                        
                        // Ângulo em graus no intervalo [0, 180)
                        double deg = Math.Atan2(gy, gx) * 180.0 / Math.PI;
                        if (deg < 0) deg += 180.0;
                        angle[idx] = deg;
                    }
                });
            }

            smooth.Unlock(false);
            smooth.Dispose();

            // 3. Supressão de Não-Máximos (NMS - Non-Maximum Suppression)
            byte[] nms = new byte[width * height];

            Parallel.For(1, height - 1, y =>
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int idx = y * width + x;
                    double mag = magnitude[idx];
                    double deg = angle[idx];

                    double q = 255, r = 255;

                    // Setor 0 graus (Horizontal: vizinhos leste e oeste)
                    if ((deg >= 0 && deg < 22.5) || (deg >= 157.5 && deg <= 180))
                    {
                        q = magnitude[y * width + (x + 1)];
                        r = magnitude[y * width + (x - 1)];
                    }
                    // Setor 45 graus (Diagonal / : vizinhos nordeste e sudoeste)
                    else if (deg >= 22.5 && deg < 67.5)
                    {
                        q = magnitude[(y - 1) * width + (x + 1)];
                        r = magnitude[(y + 1) * width + (x - 1)];
                    }
                    // Setor 90 graus (Vertical: vizinhos norte e sul)
                    else if (deg >= 67.5 && deg < 112.5)
                    {
                        q = magnitude[(y - 1) * width + x];
                        r = magnitude[(y + 1) * width + x];
                    }
                    // Setor 135 graus (Diagonal \ : vizinhos noroeste e sudeste)
                    else if (deg >= 112.5 && deg < 157.5)
                    {
                        q = magnitude[(y - 1) * width + (x - 1)];
                        r = magnitude[(y + 1) * width + (x + 1)];
                    }

                    // Se a magnitude atual for maior que os dois vizinhos ao longo do gradiente, preserva
                    if (mag >= q && mag >= r)
                    {
                        nms[idx] = (byte)Math.Clamp(mag, 0, 255);
                    }
                    else
                    {
                        nms[idx] = 0;
                    }
                }
            });

            // 4. Limiarização Dupla e Histerese
            byte[] result = new byte[width * height];
            Queue<int> edgeQueue = new Queue<int>();

            const byte STRONG_EDGE = 255;
            const byte WEAK_EDGE = 75;

            for (int i = 0; i < width * height; i++)
            {
                if (nms[i] >= highThreshold)
                {
                    result[i] = STRONG_EDGE;
                    edgeQueue.Enqueue(i);
                }
                else if (nms[i] >= lowThreshold)
                {
                    result[i] = WEAK_EDGE;
                }
                else
                {
                    result[i] = 0;
                }
            }

            // 5. Rastreamento por Histerese (BFS / Flood nas bordas fracas conectadas a fortes)
            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            while (edgeQueue.Count > 0)
            {
                int curr = edgeQueue.Dequeue();
                int cy = curr / width;
                int cx = curr % width;

                for (int k = 0; k < 8; k++)
                {
                    int nx = cx + dx[k];
                    int ny = cy + dy[k];

                    if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                    {
                        int nIdx = ny * width + nx;
                        if (result[nIdx] == WEAK_EDGE)
                        {
                            result[nIdx] = STRONG_EDGE;
                            edgeQueue.Enqueue(nIdx);
                        }
                    }
                }
            }

            // Suprime quaisquer bordas fracas restantes que não se conectaram a fortes
            DirectBitmap dst = new DirectBitmap(width, height);
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(dst.BackBuffer + (y * dst.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * width + x;
                        byte val = result[idx] == STRONG_EDGE ? (byte)255 : (byte)0;
                        row[x] = (uint)((255 << 24) | (val << 16) | (val << 8) | val);
                    }
                });
            }

            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Filtros Não-Lineares & Efeitos de Relevo

        /// <summary>
        /// Filtro da Mediana: Filtro não-linear que substitui o pixel pelo valor mediano da vizinhança.
        /// Extremamente eficaz contra ruído impulsivo (Sal e Pimenta), preservando bordas sem borrá-las.
        /// </summary>
        public static DirectBitmap MedianFilter(DirectBitmap src, int radius = 1)
        {
            int size = radius * 2 + 1;
            int total = size * size;
            int medianIndex = total / 2;

            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte[] windowR = new byte[total];
                    byte[] windowG = new byte[total];
                    byte[] windowB = new byte[total];

                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        int count = 0;
                        for (int ky = -radius; ky <= radius; ky++)
                        {
                            int py = Math.Clamp(y + ky, 0, height - 1);
                            byte* sRow = src.BackBuffer + (py * src.Stride);

                            for (int kx = -radius; kx <= radius; kx++)
                            {
                                int px = Math.Clamp(x + kx, 0, width - 1);
                                byte* p = sRow + (px * 4);

                                windowB[count] = p[0];
                                windowG[count] = p[1];
                                windowR[count] = p[2];
                                count++;
                            }
                        }

                        Array.Sort(windowB);
                        Array.Sort(windowG);
                        Array.Sort(windowR);

                        byte* pDst = dstRow + (x * 4);
                        pDst[0] = windowB[medianIndex];
                        pDst[1] = windowG[medianIndex];
                        pDst[2] = windowR[medianIndex];
                        pDst[3] = 255;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Efeito Relevo (Emboss): Cria uma ilusão tridimensional de relevo simulando uma fonte de luz direcional.
        /// Substitui regiões planas pelo cinza médio (128) e realça transições com sombras e luzes.
        /// </summary>
        public static DirectBitmap Emboss(DirectBitmap src, double angleDegrees = 45.0)
        {
            // Converte o ângulo em vetor de deslocamento
            double rad = angleDegrees * Math.PI / 180.0;
            int dx = (int)Math.Round(Math.Cos(rad));
            int dy = (int)Math.Round(Math.Sin(rad));

            double[,] kernel = new double[3, 3]
            {
                { -dy - dx,  -dy, -dy + dx },
                {     -dx,     1,      dx },
                {  dy - dx,   dy,  dy + dx }
            };

            return Convolve2D(src, kernel, 1.0, 128.0);
        }

        #endregion
    }
}
