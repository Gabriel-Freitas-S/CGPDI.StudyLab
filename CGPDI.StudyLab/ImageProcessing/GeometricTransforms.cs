using System;
using System.Threading.Tasks;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.ImageProcessing
{
    /// <summary>
    /// Tipos de interpolação espacial para reamostragem de imagens.
    /// </summary>
    public enum InterpolationMode
    {
        /// <summary> Vizinho Mais Próximo: Mais rápido, mas produz serrilhamento (aliasing) e blocos. </summary>
        NearestNeighbor,
        /// <summary> Interpolação Bilinear: Média ponderada dos 4 pixels vizinhos mais próximos. Suave e rápida. </summary>
        Bilinear,
        /// <summary> Interpolação Bicúbica: Spline cúbica considerando vizinhança 4x4 (16 pixels). Máxima qualidade. </summary>
        Bicubic
    }

    /// <summary>
    /// Transformações Geométricas 2D e Distorções Não-Lineares em PDI.
    /// 
    /// TEORIA DO MAPEAMENTO INVERSO (Backward Mapping):
    /// Para evitar buracos (gaps/holes) na imagem de destino causados pelo mapeamento direto,
    /// percorre-se cada pixel (x_dst, y_dst) da imagem de saída e calcula-se sua coordenada
    /// correspondente na imagem original através da transformação inversa:
    /// (x_src, y_src) = T^{-1}(x_dst, y_dst)
    /// Em seguida, o valor da cor em (x_src, y_src) é interpolado.
    /// </summary>
    public static class GeometricTransforms
    {
        #region Interpolação de Pixels (Bilinear e Bicúbica)

        /// <summary>
        /// Amostra a cor em coordenadas contínuas (fx, fy) usando o método de interpolação especificado.
        /// </summary>
        public static unsafe void SamplePixel(byte* srcBuf, int width, int height, int stride, double fx, double fy, InterpolationMode mode, byte* outBgra)
        {
            if (fx < 0 || fx >= width - 1 || fy < 0 || fy >= height - 1)
            {
                // Borda / Clamp
                int cx = (int)Math.Clamp(Math.Round(fx), 0, width - 1);
                int cy = (int)Math.Clamp(Math.Round(fy), 0, height - 1);
                byte* p = srcBuf + (cy * stride) + (cx * 4);
                outBgra[0] = p[0]; outBgra[1] = p[1]; outBgra[2] = p[2]; outBgra[3] = p[3];
                return;
            }

            if (mode == InterpolationMode.NearestNeighbor)
            {
                int ix = (int)Math.Round(fx);
                int iy = (int)Math.Round(fy);
                byte* p = srcBuf + (iy * stride) + (ix * 4);
                outBgra[0] = p[0]; outBgra[1] = p[1]; outBgra[2] = p[2]; outBgra[3] = p[3];
            }
            else if (mode == InterpolationMode.Bilinear)
            {
                // Interpolação Bilinear nos 4 vizinhos (x0, y0), (x1, y0), (x0, y1), (x1, y1)
                int x0 = (int)Math.Floor(fx);
                int y0 = (int)Math.Floor(fy);
                int x1 = Math.Min(x0 + 1, width - 1);
                int y1 = Math.Min(y0 + 1, height - 1);

                double u = fx - x0; // Fração horizontal
                double v = fy - y0; // Fração vertical
                double w00 = (1.0 - u) * (1.0 - v);
                double w10 = u * (1.0 - v);
                double w01 = (1.0 - u) * v;
                double w11 = u * v;

                byte* p00 = srcBuf + (y0 * stride) + (x0 * 4);
                byte* p10 = srcBuf + (y0 * stride) + (x1 * 4);
                byte* p01 = srcBuf + (y1 * stride) + (x0 * 4);
                byte* p11 = srcBuf + (y1 * stride) + (x1 * 4);

                for (int c = 0; c < 4; c++)
                {
                    double val = p00[c] * w00 + p10[c] * w10 + p01[c] * w01 + p11[c] * w11;
                    outBgra[c] = (byte)Math.Clamp(val, 0, 255);
                }
            }
            else // Bicubic
            {
                // Interpolação Bicúbica 4x4 com Spline de Keys / Catmull-Rom
                int xBase = (int)Math.Floor(fx);
                int yBase = (int)Math.Floor(fy);
                double u = fx - xBase;
                double v = fy - yBase;

                double[] weightX = new double[4];
                double[] weightY = new double[4];

                for (int i = 0; i < 4; i++)
                {
                    weightX[i] = CubicSplineWeight(u - (i - 1));
                    weightY[i] = CubicSplineWeight(v - (i - 1));
                }

                double[] sum = new double[4];

                for (int j = 0; j < 4; j++)
                {
                    int py = Math.Clamp(yBase + j - 1, 0, height - 1);
                    byte* row = srcBuf + (py * stride);

                    for (int i = 0; i < 4; i++)
                    {
                        int px = Math.Clamp(xBase + i - 1, 0, width - 1);
                        byte* p = row + (px * 4);
                        double w = weightX[i] * weightY[j];

                        sum[0] += p[0] * w;
                        sum[1] += p[1] * w;
                        sum[2] += p[2] * w;
                        sum[3] += p[3] * w;
                    }
                }

                outBgra[0] = (byte)Math.Clamp(sum[0], 0, 255);
                outBgra[1] = (byte)Math.Clamp(sum[1], 0, 255);
                outBgra[2] = (byte)Math.Clamp(sum[2], 0, 255);
                outBgra[3] = (byte)Math.Clamp(sum[3], 0, 255);
            }
        }

        private static double CubicSplineWeight(double x)
        {
            x = Math.Abs(x);
            const double a = -0.5; // Coeficiente padrão Catmull-Rom
            if (x <= 1.0)
                return (a + 2.0) * x * x * x - (a + 3.0) * x * x + 1.0;
            else if (x < 2.0)
                return a * x * x * x - 5.0 * a * x * x + 8.0 * a * x - 4.0 * a;
            return 0.0;
        }

        #endregion

        #region Transformações Afins (Rotação, Escala, Shear, Flip)

        /// <summary>
        /// Rotação 2D por ângulo arbitrário em torno de um pivô (cx, cy):
        /// [x_src]   [ cos(-theta)  -sin(-theta) ] [x_dst - cx]   [cx]
        /// [y_src] = [ sin(-theta)   cos(-theta) ] [y_dst - cy] + [cy]
        /// </summary>
        public static DirectBitmap Rotate(DirectBitmap src, double angleDegrees, InterpolationMode mode = InterpolationMode.Bilinear)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            double rad = -angleDegrees * Math.PI / 180.0; // Inverso da rotação
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);
            double cx = width / 2.0;
            double cy = height / 2.0;

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
                    byte* sample = stackalloc byte[4];

                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;

                        double srcX = cx + (dx * cos - dy * sin);
                        double srcY = cy + (dx * sin + dy * cos);

                        if (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
                        {
                            SamplePixel(srcBuf, width, height, srcStride, srcX, srcY, mode, sample);
                            byte* pDst = dstRow + (x * 4);
                            pDst[0] = sample[0];
                            pDst[1] = sample[1];
                            pDst[2] = sample[2];
                            pDst[3] = sample[3];
                        }
                        else
                        {
                            // Fora dos limites: fundo preto transparente
                            uint* p = (uint*)(dstRow + (x * 4));
                            *p = 0xFF121214;
                        }
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Escala / Redimensionamento 2D (Zoom In / Zoom Out) com fatores Sx e Sy.
        /// </summary>
        public static DirectBitmap Scale(DirectBitmap src, double scaleX, double scaleY, InterpolationMode mode = InterpolationMode.Bilinear)
        {
            scaleX = Math.Max(0.1, scaleX);
            scaleY = Math.Max(0.1, scaleY);

            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            double invSx = 1.0 / scaleX;
            double invSy = 1.0 / scaleY;
            double cx = width / 2.0;
            double cy = height / 2.0;

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);
                    byte* sample = stackalloc byte[4];

                    for (int x = 0; x < width; x++)
                    {
                        double srcX = cx + (x - cx) * invSx;
                        double srcY = cy + (y - cy) * invSy;

                        if (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
                        {
                            SamplePixel(src.BackBuffer, width, height, src.Stride, srcX, srcY, mode, sample);
                            byte* pDst = dstRow + (x * 4);
                            pDst[0] = sample[0]; pDst[1] = sample[1]; pDst[2] = sample[2]; pDst[3] = sample[3];
                        }
                        else
                        {
                            uint* p = (uint*)(dstRow + (x * 4));
                            *p = 0xFF121214;
                        }
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Cisalhamento (Shear / Skew): Inclina a imagem horizontal e verticalmente.
        /// x_src = x_dst - shX * y_dst
        /// y_src = y_dst - shY * x_dst
        /// </summary>
        public static DirectBitmap Shear(DirectBitmap src, double shX, double shY, InterpolationMode mode = InterpolationMode.Bilinear)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            double cx = width / 2.0;
            double cy = height / 2.0;

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);
                    byte* sample = stackalloc byte[4];

                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;

                        double srcX = cx + dx - shX * dy;
                        double srcY = cy + dy - shY * dx;

                        if (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
                        {
                            SamplePixel(src.BackBuffer, width, height, src.Stride, srcX, srcY, mode, sample);
                            byte* pDst = dstRow + (x * 4);
                            pDst[0] = sample[0]; pDst[1] = sample[1]; pDst[2] = sample[2]; pDst[3] = sample[3];
                        }
                        else
                        {
                            uint* p = (uint*)(dstRow + (x * 4));
                            *p = 0xFF121214;
                        }
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Espelhamento / Flip Horizontal e Vertical.
        /// </summary>
        public static DirectBitmap Flip(DirectBitmap src, bool horizontal, bool vertical)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    int srcY = vertical ? (height - 1 - y) : y;
                    byte* srcRow = src.BackBuffer + (srcY * src.Stride);
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);

                    for (int x = 0; x < width; x++)
                    {
                        int srcX = horizontal ? (width - 1 - x) : x;
                        uint* pSrc = (uint*)(srcRow + (srcX * 4));
                        uint* pDst = (uint*)(dstRow + (x * 4));
                        *pDst = *pSrc;
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Distorções Não-Lineares (Swirl, Ripple, Fisheye)

        /// <summary>
        /// Distorção em Redemoinho (Swirl / Vortex):
        /// Aplica rotação não-linear cuja intensidade decresce quadraticamente com a distância do raio central R:
        /// theta = theta_0 + factor * (1 - r / R)^2
        /// </summary>
        public static DirectBitmap Swirl(DirectBitmap src, double radius = 200.0, double strength = 3.0, InterpolationMode mode = InterpolationMode.Bilinear)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            double cx = width / 2.0;
            double cy = height / 2.0;

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);
                    byte* sample = stackalloc byte[4];

                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;
                        double r = Math.Sqrt(dx * dx + dy * dy);

                        if (r < radius)
                        {
                            double factor = (1.0 - r / radius);
                            double angle = strength * factor * factor;

                            double cos = Math.Cos(angle);
                            double sin = Math.Sin(angle);

                            double srcX = cx + (dx * cos - dy * sin);
                            double srcY = cy + (dx * sin + dy * cos);

                            SamplePixel(src.BackBuffer, width, height, src.Stride, srcX, srcY, mode, sample);
                        }
                        else
                        {
                            byte* p = src.BackBuffer + (y * src.Stride) + (x * 4);
                            sample[0] = p[0]; sample[1] = p[1]; sample[2] = p[2]; sample[3] = p[3];
                        }

                        byte* pDst = dstRow + (x * 4);
                        pDst[0] = sample[0]; pDst[1] = sample[1]; pDst[2] = sample[2]; pDst[3] = sample[3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Distorção em Ondas / Ripple:
        /// Desloca os pixels senoidalmente criando efeito de reflexo na água:
        /// x_src = x + A_x * sin(2*pi*y / lambda_y)
        /// y_src = y + A_y * cos(2*pi*x / lambda_x)
        /// </summary>
        public static DirectBitmap Wave(DirectBitmap src, double amplitude = 12.0, double frequency = 0.05, InterpolationMode mode = InterpolationMode.Bilinear)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);
                    byte* sample = stackalloc byte[4];

                    for (int x = 0; x < width; x++)
                    {
                        double srcX = x + amplitude * Math.Sin(y * frequency);
                        double srcY = y + amplitude * Math.Cos(x * frequency);

                        SamplePixel(src.BackBuffer, width, height, src.Stride, srcX, srcY, mode, sample);

                        byte* pDst = dstRow + (x * 4);
                        pDst[0] = sample[0]; pDst[1] = sample[1]; pDst[2] = sample[2]; pDst[3] = sample[3];
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        /// <summary>
        /// Distorção Olho de Peixe / Barril (Fisheye / Barrel Lens Distortion):
        /// Modela a curvatura de lentes grande-angulares fotográficas:
        /// r_src = r_dst * (1 + k * r_dst^2)
        /// </summary>
        public static DirectBitmap Fisheye(DirectBitmap src, double distortionK = 0.000008, InterpolationMode mode = InterpolationMode.Bilinear)
        {
            int width = src.Width;
            int height = src.Height;
            DirectBitmap dst = new DirectBitmap(width, height);

            double cx = width / 2.0;
            double cy = height / 2.0;

            src.Lock();
            dst.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    byte* dstRow = dst.BackBuffer + (y * dst.Stride);
                    byte* sample = stackalloc byte[4];

                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;
                        double rSq = dx * dx + dy * dy;

                        double factor = 1.0 + distortionK * rSq;
                        double srcX = cx + dx * factor;
                        double srcY = cy + dy * factor;

                        if (srcX >= 0 && srcX < width && srcY >= 0 && srcY < height)
                        {
                            SamplePixel(src.BackBuffer, width, height, src.Stride, srcX, srcY, mode, sample);
                            byte* pDst = dstRow + (x * 4);
                            pDst[0] = sample[0]; pDst[1] = sample[1]; pDst[2] = sample[2]; pDst[3] = sample[3];
                        }
                        else
                        {
                            uint* p = (uint*)(dstRow + (x * 4));
                            *p = 0xFF121214;
                        }
                    }
                });
            }

            src.Unlock(false);
            dst.Unlock(true);
            return dst;
        }

        #endregion
    }
}
