using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Classe de alto desempenho para manipulação direta de pixels na memória.
    /// Utiliza ponteiros inseguros (unsafe pointers) sobre o buffer de um WriteableBitmap,
    /// eliminando o overhead de chamadas lentas como GetPixel/SetPixel do System.Drawing
    /// e permitindo processamento em tempo real a 60+ FPS com multithreading (Parallel.For).
    /// 
    /// FORMATO DE PIXEL PADRÃO: Bgr32 / Bgra32 (4 bytes por pixel: Blue, Green, Red, Alpha).
    /// Cada pixel ocupa 32 bits (4 bytes), permitindo acesso como uint* ou byte*.
    /// </summary>
    public sealed unsafe class DirectBitmap : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public int Stride { get; }
        public WriteableBitmap Bitmap { get; }

        private byte* _backBuffer;
        private bool _isLocked;
        private bool _disposed;

        public DirectBitmap(int width, int height)
        {
            if (width <= 0 || height <= 0)
                throw new ArgumentException("As dimensões da imagem devem ser maiores que zero.");

            Width = width;
            Height = height;
            
            // Criação do WriteableBitmap com DPI padrão (96 DPI) e formato Bgra32
            // Bgra32 é o formato nativo mais eficiente para aceleração DirectX no WPF.
            Bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgra32, null);
            Stride = Bitmap.BackBufferStride;
        }

        /// <summary>
        /// Bloqueia o buffer traseiro do WriteableBitmap para escrita direta via ponteiro.
        /// DEVE ser chamado antes de ler ou alterar os pixels com GetPixel / SetPixel / ProcessParallel.
        /// </summary>
        public void Lock()
        {
            if (!_isLocked)
            {
                Bitmap.Lock();
                _backBuffer = (byte*)Bitmap.BackBuffer.ToPointer();
                _isLocked = true;
            }
        }

        /// <summary>
        /// Desbloqueia o buffer e notifica o subsistema de renderização do WPF que a região foi alterada.
        /// </summary>
        public void Unlock(bool markDirty = true)
        {
            if (_isLocked)
            {
                if (markDirty)
                {
                    // Notifica o WPF que toda a área da imagem foi alterada para redesenho
                    Bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                }
                Bitmap.Unlock();
                _backBuffer = null;
                _isLocked = false;
            }
        }

        /// <summary>
        /// Retorna um ponteiro direto para o início do buffer de pixels (Bgra32).
        /// </summary>
        public byte* BackBuffer => _backBuffer;

        /// <summary>
        /// Obtém a cor de um pixel específico (x, y).
        /// </summary>
        public Color GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return Colors.Black;

            byte* pixelPtr = _backBuffer + (y * Stride) + (x * 4);
            byte b = pixelPtr[0];
            byte g = pixelPtr[1];
            byte r = pixelPtr[2];
            byte a = pixelPtr[3];

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Define a cor de um pixel específico (x, y).
        /// </summary>
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            byte* pixelPtr = _backBuffer + (y * Stride) + (x * 4);
            pixelPtr[0] = color.B;
            pixelPtr[1] = color.G;
            pixelPtr[2] = color.R;
            pixelPtr[3] = color.A;
        }

        /// <summary>
        /// Define a cor de um pixel via uint (formato 0xAARRGGBB ou 0xFFRRGGBB).
        /// </summary>
        public void SetPixelFast(int x, int y, uint bgra)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
            {
                uint* row = (uint*)(_backBuffer + (y * Stride));
                row[x] = bgra;
            }
        }

        /// <summary>
        /// Limpa toda a imagem com uma cor específica.
        /// </summary>
        public void Clear(Color color)
        {
            uint bgra = (uint)((color.A << 24) | (color.R << 16) | (color.G << 8) | color.B);
            Parallel.For(0, Height, y =>
            {
                uint* row = (uint*)(_backBuffer + (y * Stride));
                for (int x = 0; x < Width; x++)
                {
                    row[x] = bgra;
                }
            });
        }

        /// <summary>
        /// Clona a imagem atual para uma nova instância de DirectBitmap.
        /// </summary>
        public DirectBitmap Clone()
        {
            DirectBitmap clone = new DirectBitmap(Width, Height);
            this.Lock();
            clone.Lock();

            // Cópia ultra-rápida de memória linha a linha
            Parallel.For(0, Height, y =>
            {
                byte* srcRow = this.BackBuffer + (y * this.Stride);
                byte* dstRow = clone.BackBuffer + (y * clone.Stride);
                Buffer.MemoryCopy(srcRow, dstRow, clone.Stride, this.Stride);
            });

            this.Unlock(false);
            clone.Unlock(true);
            return clone;
        }

        /// <summary>
        /// Cria um DirectBitmap a partir de qualquer BitmapSource (PNG, JPG, BMP).
        /// Converte automaticamente para Bgra32.
        /// </summary>
        public static DirectBitmap FromBitmapSource(BitmapSource source)
        {
            // Garante formato Bgra32
            FormatConvertedBitmap converted = new FormatConvertedBitmap();
            converted.BeginInit();
            converted.Source = source;
            converted.DestinationFormat = PixelFormats.Bgra32;
            converted.EndInit();

            int width = converted.PixelWidth;
            int height = converted.PixelHeight;

            DirectBitmap bmp = new DirectBitmap(width, height);
            bmp.Lock();
            converted.CopyPixels(new Int32Rect(0, 0, width, height), (IntPtr)bmp.BackBuffer, bmp.Stride * height, bmp.Stride);
            bmp.Unlock(true);

            return bmp;
        }

        /// <summary>
        /// Carrega uma imagem do disco a partir de um arquivo.
        /// </summary>
        public static DirectBitmap FromFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Arquivo de imagem não encontrado.", filePath);

            BitmapImage img = new BitmapImage();
            img.BeginInit();
            img.CacheOption = BitmapCacheOption.OnLoad;
            img.UriSource = new Uri(filePath, UriKind.Absolute);
            img.EndInit();
            img.Freeze();

            return FromBitmapSource(img);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_isLocked)
                    Unlock(false);
                _disposed = true;
            }
        }
    }
}
