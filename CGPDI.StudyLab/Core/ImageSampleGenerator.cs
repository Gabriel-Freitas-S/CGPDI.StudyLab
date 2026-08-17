using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Gerador procedural de padrões e imagens sintéticas de calibração para testes de PDI e Computação Gráfica.
    /// Permite testar todos os algoritmos instantaneamente sem necessidade de carregar arquivos externos.
    /// </summary>
    public static class ImageSampleGenerator
    {
        /// <summary>
        /// Gera uma cena de calibração completa contendo:
        /// - Gradientes de cor contínuos (teste de quantização e posterização)
        /// - Regiões de alta frequência espacial (teste de filtros de nitidez e blur)
        /// - Círculo cromático HSV (teste de segmentação e conversão de cores)
        /// - Formas geométricas com bordas nítidas (teste de Sobel, Canny e Morfologia)
        /// - Textura xadrez e ruído (teste de filtros passa-baixa e mediana)
        /// </summary>
        public static DirectBitmap GenerateCalibrationScene(int width = 512, int height = 512)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            double centerX = width / 2.0;
            double centerY = height / 2.0;

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - centerX;
                        double dy = y - centerY;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        byte r = 0, g = 0, b = 0;

                        // 1. Círculo Central Cromático HSV (Roda de Cores)
                        if (dist < width * 0.22)
                        {
                            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
                            if (angle < 0) angle += 360.0;
                            double sat = Math.Min(1.0, dist / (width * 0.22));
                            Color c = ColorSpaces.HsvToRgb(angle, sat, 0.95);
                            r = c.R; g = c.G; b = c.B;
                        }
                        // 2. Anel concêntrico de calibração de frequência (Padrão Siemens / Chirp)
                        else if (dist >= width * 0.22 && dist < width * 0.32)
                        {
                            double angle = Math.Atan2(dy, dx);
                            double freq = Math.Sin(angle * 32.0); // 32 raios radiais
                            byte val = (byte)(freq > 0 ? 240 : 20);
                            r = val; g = val; b = val;
                        }
                        // 3. Quatro Quadrantes Temáticos ao redor
                        else
                        {
                            // Quadrante Superior Esquerdo: Gradiente Suave RGB
                            if (x < centerX && y < centerY)
                            {
                                r = (byte)(x * 255.0 / centerX);
                                g = (byte)(y * 255.0 / centerY);
                                b = 180;
                            }
                            // Quadrante Superior Direito: Padrão Xadrez e Textura Fina
                            else if (x >= centerX && y < centerY)
                            {
                                int checkSize = 16;
                                bool isEven = ((x / checkSize) + (y / checkSize)) % 2 == 0;
                                byte val = (byte)(isEven ? 230 : 60);
                                r = val; g = (byte)(val * 0.8); b = (byte)(val * 0.4);
                            }
                            // Quadrante Inferior Esquerdo: Formas Geométricas para Detecção de Bordas
                            else if (x < centerX && y >= centerY)
                            {
                                int relX = x;
                                int relY = y - (int)centerY;
                                // Círculo e retângulo
                                double cDist = Math.Sqrt((relX - 100) * (relX - 100) + (relY - 100) * (relY - 100));
                                if (cDist < 50)
                                {
                                    r = 255; g = 80; b = 80;
                                }
                                else if (relX > 160 && relX < 230 && relY > 50 && relY < 180)
                                {
                                    r = 80; g = 220; b = 100;
                                }
                                else
                                {
                                    r = 40; g = 40; b = 50;
                                }
                            }
                            // Quadrante Inferior Direito: Gradiente de Luminância com Faixas de Frequência
                            else
                            {
                                double wave = Math.Sin((x - centerX) * 0.2) * Math.Cos((y - centerY) * 0.2);
                                byte lum = (byte)Math.Clamp(128 + wave * 100, 0, 255);
                                r = lum; g = lum; b = lum;
                            }
                        }

                        // Formato BGRA32: 0xAARRGGBB
                        row[x] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Gera uma roda de cores contínua pura no espaço HSV.
        /// </summary>
        public static DirectBitmap GenerateColorWheel(int width = 512, int height = 512)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            double cx = width / 2.0;
            double cy = height / 2.0;
            double radius = Math.Min(cx, cy) * 0.95;

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        if (dist <= radius)
                        {
                            double angle = Math.Atan2(dy, dx) * 180.0 / Math.PI;
                            if (angle < 0) angle += 360.0;
                            double sat = dist / radius;
                            Color c = ColorSpaces.HsvToRgb(angle, sat, 1.0);
                            row[x] = (uint)((255 << 24) | (c.R << 16) | (c.G << 8) | c.B);
                        }
                        else
                        {
                            // Fundo cinza escuro
                            row[x] = 0xFF181818;
                        }
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Gera padrão de Estrela de Siemens e frequência senoidal (chirp) para análise de MTF e Teorema de Nyquist.
        /// </summary>
        public static DirectBitmap GenerateFrequencyPattern(int width = 512, int height = 512)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            double cx = width / 2.0;
            double cy = height / 2.0;

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        double dx = x - cx;
                        double dy = y - cy;
                        double dist = Math.Sqrt(dx * dx + dy * dy);

                        // Onda senoidal com frequência que cresce linearmente com a distância ao centro
                        double wave = Math.Cos(dist * dist * 0.005);
                        byte val = (byte)Math.Clamp((wave + 1.0) * 127.5, 0, 255);

                        row[x] = (uint)((255 << 24) | (val << 16) | (val << 8) | val);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Gera uma imagem com ruído Gaussiano e ruído impulsivo (Sal e Pimenta) para demonstrar filtros.
        /// </summary>
        public static DirectBitmap GenerateNoisyImage(int width = 512, int height = 512, double noiseLevel = 0.2)
        {
            DirectBitmap bmp = GenerateCalibrationScene(width, height);
            bmp.Lock();

            Random rnd = new Random(42);

            unsafe
            {
                for (int y = 0; y < height; y++)
                {
                    byte* row = bmp.BackBuffer + (y * bmp.Stride);
                    for (int x = 0; x < width; x++)
                    {
                        byte* p = row + (x * 4);

                        // Ruído Sal e Pimenta (Impulsivo)
                        double roll = rnd.NextDouble();
                        if (roll < noiseLevel * 0.5)
                        {
                            p[0] = 0; p[1] = 0; p[2] = 0; // Pimenta (Preto)
                        }
                        else if (roll < noiseLevel)
                        {
                            p[0] = 255; p[1] = 255; p[2] = 255; // Sal (Branco)
                        }
                        else
                        {
                            // Ruído Gaussiano aditivo leve
                            int gNoise = (int)((rnd.NextDouble() - 0.5) * 50);
                            p[0] = (byte)Math.Clamp(p[0] + gNoise, 0, 255);
                            p[1] = (byte)Math.Clamp(p[1] + gNoise, 0, 255);
                            p[2] = (byte)Math.Clamp(p[2] + gNoise, 0, 255);
                        }
                    }
                }
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Gera uma textura procedural de pedra/granito com ruído fractal multifrequencial para mapeamento 3D com TileMode="Tile".
        /// </summary>
        public static DirectBitmap GenerateStoneGraniteTexture(int width = 256, int height = 256)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        double n1 = Math.Sin(x * 0.15) * Math.Cos(y * 0.15);
                        double n2 = Math.Sin(x * 0.42 + y * 0.31) * 0.5;
                        double n3 = Math.Sin((x ^ y) * 0.73) * 0.25;
                        double val = (n1 + n2 + n3) / 1.75; // -1 .. 1

                        int baseLum = (int)(140 + val * 55);
                        int grain = ((x * 37 + y * 59) % 29) - 14;

                        byte r = (byte)Math.Clamp(baseLum + grain + 10, 0, 255);
                        byte g = (byte)Math.Clamp(baseLum + grain + 8, 0, 255);
                        byte b = (byte)Math.Clamp(baseLum + grain + 15, 0, 255);

                        row[x] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Gera uma textura procedural de terreno desértico/dunas de areia para cenários 3D.
        /// </summary>
        public static DirectBitmap GenerateDesertSandTexture(int width = 256, int height = 256)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        double dune = Math.Sin(x * 0.08 + Math.Sin(y * 0.04) * 2.0);
                        double micro = Math.Sin(x * 0.35 + y * 0.25) * 0.2;
                        double total = (dune + micro) * 0.5; // -0.6 .. 0.6

                        int baseR = (int)(215 + total * 35);
                        int baseG = (int)(175 + total * 30);
                        int baseB = (int)(110 + total * 25);
                        int grain = ((x * 43 + y * 71) % 17) - 8;

                        byte r = (byte)Math.Clamp(baseR + grain, 0, 255);
                        byte g = (byte)Math.Clamp(baseG + grain, 0, 255);
                        byte b = (byte)Math.Clamp(baseB + grain, 0, 255);

                        row[x] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }
    }
}
