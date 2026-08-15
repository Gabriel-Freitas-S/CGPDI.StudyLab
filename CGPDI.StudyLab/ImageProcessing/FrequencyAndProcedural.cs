using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Media;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.ImageProcessing
{
    /// <summary>
    /// Processamento no Domínio da Frequência (Transformada de Fourier 2D),
    /// Síntese Procedural de Ruído (Perlin Noise, Voronoi) e Fractais Matemáticos (Mandelbrot & Julia).
    /// </summary>
    public static class FrequencyAndProcedural
    {
        #region Transformada de Fourier 2D e Espectro de Magnitude

        /// <summary>
        /// Calcula o Espectro de Magnitude da Transformada Discreta de Fourier 2D (2D DFT).
        /// Inclui centralização de quadrantes (FFTShift) e compressão de faixa dinâmica logarítmica:
        /// S(u, v) = c * log(1 + |F(u, v)|)
        /// 
        /// TEORIA:
        /// F(u, v) = \sum_{x=0}^{M-1} \sum_{y=0}^{N-1} f(x, y) \cdot e^{-j 2\pi (\frac{ux}{M} + \frac{vy}{N})}
        /// Baixas frequências (informação global e cores) concentram-se no centro;
        /// Altas frequências (bordas, detalhes e ruídos) espalham-se na periferia.
        /// </summary>
        public static DirectBitmap ComputeFourierMagnitudeSpectrum(DirectBitmap src, int sampleSize = 128)
        {
            // Reamostra para tamanho adequado para 2D DFT em tempo real
            int n = sampleSize;
            double[,] spatial = new double[n, n];

            src.Lock();
            unsafe
            {
                for (int y = 0; y < n; y++)
                {
                    int srcY = (int)(y * (double)src.Height / n);
                    byte* row = src.BackBuffer + (srcY * src.Stride);

                    for (int x = 0; x < n; x++)
                    {
                        int srcX = (int)(x * (double)src.Width / n);
                        byte* p = row + (srcX * 4);
                        // Multiplica por (-1)^(x+y) para centralizar a componente DC (0,0) no centro do espectro!
                        double factor = ((x + y) % 2 == 0) ? 1.0 : -1.0;
                        double lum = (0.299 * p[2] + 0.587 * p[1] + 0.114 * p[0]);
                        spatial[y, x] = lum * factor;
                    }
                }
            }
            src.Unlock(false);

            // DFT 2D separável por Linhas e Colunas: O(N^3)
            Complex[,] complexFreq = new Complex[n, n];

            // 1. DFT 1D ao longo das linhas
            Parallel.For(0, n, y =>
            {
                for (int u = 0; u < n; u++)
                {
                    Complex sum = Complex.Zero;
                    for (int x = 0; x < n; x++)
                    {
                        double angle = -2.0 * Math.PI * u * x / n;
                        sum += spatial[y, x] * new Complex(Math.Cos(angle), Math.Sin(angle));
                    }
                    complexFreq[y, u] = sum;
                }
            });

            // 2. DFT 1D ao longo das colunas
            Complex[,] fourier2D = new Complex[n, n];
            Parallel.For(0, n, u =>
            {
                for (int v = 0; v < n; v++)
                {
                    Complex sum = Complex.Zero;
                    for (int y = 0; y < n; y++)
                    {
                        double angle = -2.0 * Math.PI * v * y / n;
                        sum += complexFreq[y, u] * new Complex(Math.Cos(angle), Math.Sin(angle));
                    }
                    fourier2D[v, u] = sum;
                }
            });

            // 3. Calcula magnitude logarítmica para visualização perceptiva
            double[,] logMag = new double[n, n];
            double maxLog = 0;

            for (int v = 0; v < n; v++)
            {
                for (int u = 0; u < n; u++)
                {
                    double mag = fourier2D[v, u].Magnitude;
                    double l = Math.Log(1.0 + mag);
                    logMag[v, u] = l;
                    if (l > maxLog) maxLog = l;
                }
            }

            if (maxLog <= 0) maxLog = 1.0;

            // Renderiza bitmap de visualização com mapa de cores espectral (Turbo/Inferno)
            DirectBitmap dst = new DirectBitmap(n, n);
            dst.Lock();

            unsafe
            {
                Parallel.For(0, n, y =>
                {
                    uint* row = (uint*)(dst.BackBuffer + (y * dst.Stride));
                    for (int x = 0; x < n; x++)
                    {
                        double norm = logMag[y, x] / maxLog;
                        // Mapa de cores azul -> ciano -> verde -> amarelo -> branco
                        Color c = ColorSpaces.HsvToRgb((1.0 - norm) * 240.0, Math.Min(1.0, norm * 1.5), norm > 0.1 ? 1.0 : norm * 10);
                        row[x] = (uint)((255 << 24) | (c.R << 16) | (c.G << 8) | c.B);
                    }
                });
            }

            dst.Unlock(true);
            return dst;
        }

        #endregion

        #region Ruído Procedural (Perlin Noise, Voronoi & Fractal Brownian Motion)

        private static readonly int[] Permutation = new int[512];

        static FrequencyAndProcedural()
        {
            int[] p = new int[256];
            for (int i = 0; i < 256; i++) p[i] = i;

            Random rnd = new Random(1337);
            for (int i = 255; i > 0; i--)
            {
                int swapIdx = rnd.Next(i + 1);
                (p[i], p[swapIdx]) = (p[swapIdx], p[i]);
            }

            for (int i = 0; i < 512; i++)
                Permutation[i] = p[i & 255];
        }

        private static double Fade(double t) => t * t * t * (t * (t * 6.0 - 15.0) + 10.0); // Quintic interpolant de Ken Perlin
        private static double Lerp(double t, double a, double b) => a + t * (b - a);

        private static double Grad(int hash, double x, double y)
        {
            int h = hash & 7;
            double u = h < 4 ? x : y;
            double v = h < 4 ? y : x;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }

        /// <summary>
        /// Ruído de Perlin 2D (Ken Perlin, Improved Noise 2002):
        /// Gera uma função pseudo-aleatória contínua e suave amplamente utilizada em shaders, texturização procedural
        /// e geração de terrenos em Computação Gráfica.
        /// </summary>
        public static double PerlinNoise2D(double x, double y)
        {
            int xi = (int)Math.Floor(x) & 255;
            int yi = (int)Math.Floor(y) & 255;

            double xf = x - Math.Floor(x);
            double yf = y - Math.Floor(y);

            double u = Fade(xf);
            double v = Fade(yf);

            int aa = Permutation[Permutation[xi] + yi];
            int ab = Permutation[Permutation[xi] + yi + 1];
            int ba = Permutation[Permutation[xi + 1] + yi];
            int bb = Permutation[Permutation[xi + 1] + yi + 1];

            double x1 = Lerp(u, Grad(aa, xf, yf), Grad(ba, xf - 1, yf));
            double x2 = Lerp(u, Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1));

            return (Lerp(v, x1, x2) + 1.0) * 0.5; // Normalizado em [0, 1]
        }

        /// <summary>
        /// Movimento Browniano Fracionário (fBm - Fractal Brownian Motion):
        /// Combina múltiplas camadas (oitavas) de ruído Perlin com frequências crescentes (lacunarity)
        /// e amplitudes decrescentes (persistence) para simular nuvens, fogo, madeira e terrenos hiper-realistas.
        /// </summary>
        public static double FractalBrownianMotion(double x, double y, int octaves = 5, double persistence = 0.5, double lacunarity = 2.0)
        {
            double total = 0.0;
            double frequency = 1.0;
            double amplitude = 1.0;
            double maxValue = 0.0;

            for (int i = 0; i < octaves; i++)
            {
                total += PerlinNoise2D(x * frequency, y * frequency) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= lacunarity;
            }

            return total / maxValue;
        }

        /// <summary>
        /// Gera imagem de Ruído Procedural fBm / Terreno.
        /// </summary>
        public static DirectBitmap GenerateProceduralNoiseImage(int width = 512, int height = 512, double scale = 8.0, int octaves = 6)
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
                        double nx = (double)x / width * scale;
                        double ny = (double)y / height * scale;

                        double val = FractalBrownianMotion(nx, ny, octaves, 0.5, 2.0);

                        // Mapeamento em relevo topográfico (Água, Areia, Floresta, Montanha, Neve)
                        Color c;
                        if (val < 0.4)
                        {
                            c = Color.FromRgb(30, 80, (byte)(160 + val * 150)); // Oceano
                        }
                        else if (val < 0.45)
                        {
                            c = Color.FromRgb(220, 200, 130); // Areia / Praia
                        }
                        else if (val < 0.7)
                        {
                            c = Color.FromRgb(40, (byte)(100 + val * 120), 50); // Floresta
                        }
                        else if (val < 0.85)
                        {
                            c = Color.FromRgb((byte)(110 + val * 60), (byte)(100 + val * 60), 90); // Montanha
                        }
                        else
                        {
                            c = Color.FromRgb(240, 245, 255); // Neve
                        }

                        row[x] = (uint)((255 << 24) | (c.R << 16) | (c.G << 8) | c.B);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Ruído Celular de Voronoi / Worley Noise:
        /// Calcula a distância para o ponto de semente mais próximo, gerando padrões orgânicos de pedras, escamas e tecidos biológicos.
        /// </summary>
        public static DirectBitmap GenerateVoronoiNoiseImage(int width = 512, int height = 512, int numCells = 32)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            Random rnd = new Random(777);
            double[] pointsX = new double[numCells];
            double[] pointsY = new double[numCells];
            Color[] cellColors = new Color[numCells];

            for (int i = 0; i < numCells; i++)
            {
                pointsX[i] = rnd.NextDouble() * width;
                pointsY[i] = rnd.NextDouble() * height;
                cellColors[i] = ColorSpaces.HsvToRgb(rnd.NextDouble() * 360.0, 0.7, 0.9);
            }

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    for (int x = 0; x < width; x++)
                    {
                        double minDist = double.MaxValue;
                        int closestIdx = 0;

                        for (int i = 0; i < numCells; i++)
                        {
                            double dx = x - pointsX[i];
                            double dy = y - pointsY[i];
                            double dist = dx * dx + dy * dy; // Distância Euclidiana quadrática

                            if (dist < minDist)
                            {
                                minDist = dist;
                                closestIdx = i;
                            }
                        }

                        Color c = cellColors[closestIdx];
                        double dNorm = Math.Sqrt(minDist) / (width / 4.0);
                        double shade = Math.Clamp(1.0 - dNorm * 0.5, 0.2, 1.0);

                        byte r = (byte)(c.R * shade);
                        byte g = (byte)(c.G * shade);
                        byte b = (byte)(c.B * shade);

                        row[x] = (uint)((255 << 24) | (r << 16) | (g << 8) | b);
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        #endregion

        #region Fractais Matemáticos (Mandelbrot & Julia Sets)

        /// <summary>
        /// Renderizador do Conjunto de Mandelbrot com algoritmo de coloração suave (Smooth Escape-Time Coloring):
        /// Z_{n+1} = Z_n^2 + C
        /// 
        /// FÓRMULA DE COLORAÇÃO CONTÍNUA:
        /// \nu = i + 1 - \frac{\ln(\ln |Z|)}{\ln 2}
        /// Elimina as bandas discretas de cor e cria gradientes contínuos perfeitos.
        /// </summary>
        public static DirectBitmap GenerateMandelbrot(int width = 512, int height = 512, double centerX = -0.75, double centerY = 0.0, double zoom = 1.0, int maxIter = 100)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            double rangeX = 3.0 / zoom;
            double rangeY = (3.0 * height / width) / zoom;
            double minX = centerX - rangeX / 2.0;
            double minY = centerY - rangeY / 2.0;

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * bmp.Stride));
                    double ci = minY + (y * rangeY / height);

                    for (int x = 0; x < width; x++)
                    {
                        double cr = minX + (x * rangeX / width);

                        double zr = 0, zi = 0;
                        int iter = 0;

                        while (zr * zr + zi * zi <= 4.0 && iter < maxIter)
                        {
                            double zrNew = zr * zr - zi * zi + cr;
                            zi = 2.0 * zr * zi + ci;
                            zr = zrNew;
                            iter++;
                        }

                        if (iter == maxIter)
                        {
                            // Interior do conjunto de Mandelbrot: Preto
                            row[x] = 0xFF000000;
                        }
                        else
                        {
                            // Coloração suave contínua
                            double modSq = zr * zr + zi * zi;
                            double nu = iter + 1 - Math.Log(Math.Log(Math.Sqrt(modSq))) / Math.Log(2.0);
                            double hue = (nu * 8.0) % 360.0;
                            Color c = ColorSpaces.HsvToRgb(hue, 0.85, 0.95);
                            row[x] = (uint)((255 << 24) | (c.R << 16) | (c.G << 8) | c.B);
                        }
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        /// <summary>
        /// Renderizador do Conjunto de Julia (Z_{n+1} = Z_n^2 + C_0 para constante fixa C_0):
        /// </summary>
        public static DirectBitmap GenerateJulia(int width = 512, int height = 512, double cr = -0.7, double ci = 0.27015, int maxIter = 100)
        {
            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();

            double range = 3.0;
            double minX = -range / 2.0;
            double minY = -range / 2.0;

            unsafe
            {
                Parallel.For(0, height, y =>
                {
                    uint* row = (uint*)(bmp.BackBuffer + (y * dstStrideSafe(bmp)));
                    double zi0 = minY + (y * range / height);

                    for (int x = 0; x < width; x++)
                    {
                        double zr = minX + (x * range / width);
                        double zi = zi0;
                        int iter = 0;

                        while (zr * zr + zi * zi <= 4.0 && iter < maxIter)
                        {
                            double zrNew = zr * zr - zi * zi + cr;
                            zi = 2.0 * zr * zi + ci;
                            zr = zrNew;
                            iter++;
                        }

                        if (iter == maxIter)
                        {
                            row[x] = 0xFF000000;
                        }
                        else
                        {
                            double modSq = zr * zr + zi * zi;
                            double nu = iter + 1 - Math.Log(Math.Log(Math.Sqrt(modSq))) / Math.Log(2.0);
                            double hue = (nu * 10.0 + 180.0) % 360.0;
                            Color c = ColorSpaces.HsvToRgb(hue, 0.9, 0.95);
                            row[x] = (uint)((255 << 24) | (c.R << 16) | (c.G << 8) | c.B);
                        }
                    }
                });
            }

            bmp.Unlock(true);
            return bmp;
        }

        private static int dstStrideSafe(DirectBitmap b) => b.Stride;

        #endregion
    }
}
