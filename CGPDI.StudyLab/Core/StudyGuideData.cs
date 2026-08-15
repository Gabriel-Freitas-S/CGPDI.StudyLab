using System;
using System.Collections.Generic;

namespace CGPDI.StudyLab.Core
{
    public class DocReference
    {
        public string Title { get; set; } = "";
        public string Url { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class StudyTopic
    {
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Summary { get; set; } = "";
        public string MathFormulas { get; set; } = "";
        public string CodeExplanation { get; set; } = "";
        public string CodeSnippet { get; set; } = "";
        public string ComplexityAndTips { get; set; } = "";
        public string WhereToTest { get; set; } = "";
        public List<DocReference> MicrosoftReferences { get; set; } = new List<DocReference>();
    }

    /// <summary>
    /// Base de conhecimento e documentação pedagógica completa integrada ao software.
    /// Explica passo a passo a teoria matemática, a implementação em C# e fornece referências oficiais da Microsoft Learn.
    /// </summary>
    public static class StudyGuideData
    {
        public static List<StudyTopic> GetTopics()
        {
            return new List<StudyTopic>
            {
                #region 1. Fundamentos & Estrutura de Memória
                new StudyTopic
                {
                    Id = "mem_directbitmap",
                    Category = "1. Fundamentos & Memória",
                    Title = "Manipulação Direta de Memória com Ponteiros (Unsafe Pointers & Stride)",
                    Summary = "Como manipular pixels diretamente na memória RAM a 60+ FPS sem o overhead do GetPixel/SetPixel clássico.",
                    MathFormulas = 
                        "• Cálculo do Endereço de Memória do Pixel (x, y) no Buffer BGRA32:\n" +
                        "  Endereço = BaseBuffer + (y * Stride) + (x * 4)\n\n" +
                        "• Onde:\n" +
                        "  - BaseBuffer: Ponteiro para o byte 0 da imagem na memória.\n" +
                        "  - Stride (Largura em Bytes): Width * 4 (alinhado a 4 ou 16 bytes pela GPU).\n" +
                        "  - 4 Bytes por Pixel: Byte 0 = Azul (B), Byte 1 = Verde (G), Byte 2 = Vermelho (R), Byte 3 = Alpha (A).",
                    CodeExplanation =
                        "1. O método Bitmap.Lock() bloqueia o buffer traseiro do WriteableBitmap para evitar que o Garbage Collector (GC) o mova.\n" +
                        "2. Obtemos um ponteiro bruto (byte*) via Bitmap.BackBuffer.ToPointer().\n" +
                        "3. Usamos Parallel.For(0, Height, y => ...) para paralelizar o processamento em todas as CPUs.\n" +
                        "4. Bitmap.AddDirtyRect() notifica o DirectX do WPF que a área foi alterada para redesenho com aceleração de hardware.",
                    CodeSnippet =
@"public unsafe class DirectBitmap
{
    // Acesso direto via ponteiro sem overhead
    byte* pixelPtr = _backBuffer + (y * Stride) + (x * 4);
    byte b = pixelPtr[0]; // Azul
    byte g = pixelPtr[1]; // Verde
    byte r = pixelPtr[2]; // Vermelho
    byte a = pixelPtr[3]; // Alpha
}",
                    ComplexityAndTips = "• Complexidade: O(W * H) com paralelismo multinúcleo O((W*H)/N_cores). 100x mais rápido que GetPixel do GDI+.",
                    WhereToTest = "Base de todo o processamento de imagens (PDI), Rasterização 2D e Ray Tracer.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Código não seguro e ponteiros no C# (unsafe)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/unsafe-code",
                            Description = "Guia oficial de ponteiros brutos, tipos não gerenciados e a palavra-chave unsafe no C#."
                        },
                        new DocReference
                        {
                            Title = "Classe WriteableBitmap (System.Windows.Media.Imaging)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.imaging.writeablebitmap",
                            Description = "Documentação oficial dos métodos Lock, Unlock, AddDirtyRect e propriedade BackBuffer."
                        },
                        new DocReference
                        {
                            Title = "Gerenciamento de Memória & Garbage Collection no .NET",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/standard/garbage-collection/",
                            Description = "Como o GC gerencia o Heap gerenciado e por que fixamos ponteiros para evitar realocações."
                        }
                    }
                },
                #endregion

                #region 2. Espaços de Cores
                new StudyTopic
                {
                    Id = "color_models",
                    Category = "2. Teoria da Cor & Percepção",
                    Title = "Modelos de Cor (RGB, HSV, YCbCr, CMYK e Escala de Cinza Perceptiva)",
                    Summary = "A física da luz, fisiologia dos fotorreceptores humanos (cones) e conversão entre modelos cromáticos.",
                    MathFormulas =
                        "• Luminância sRGB (ITU-R BT.709):\n" +
                        "  Y = 0.2126·R + 0.7152·G + 0.0722·B\n\n" +
                        "• Luminância NTSC/PAL (ITU-R BT.601):\n" +
                        "  Y = 0.299·R + 0.587·G + 0.114·B\n\n" +
                        "• Espaço Cilíndrico HSV (Hue, Saturation, Value):\n" +
                        "  V = max(R, G, B)\n" +
                        "  S = (V - min(R, G, B)) / V\n" +
                        "  H = 60° · ((G - B)/Δ mod 6) [se max=R]\n\n" +
                        "• Compressão JPEG YCbCr:\n" +
                        "  Y  =  0.299·R + 0.587·G + 0.114·B\n" +
                        "  Cb = 128 - 0.1687·R - 0.3313·G + 0.5·B\n" +
                        "  Cr = 128 + 0.5·R - 0.4187·G - 0.0813·B",
                    CodeExplanation =
                        "1. O olho humano possui 3 tipos de cones: L (vermelho), M (verde) e S (azul). Temos quase o dobro de cones sensíveis ao espectro verde.\n" +
                        "2. Por isso, a conversão para escala de cinza dá mais de 71% de peso ao canal Verde (BT.709).\n" +
                        "3. No modelo YCbCr, a visão humana é menos sensível a variações de cor do que de brilho, permitindo descartar metade da informação de crominância (Chroma Subsampling 4:2:0 em JPEG/MPEG).",
                    CodeSnippet =
@"// Conversão rápida de luminância inteira sem ponto flutuante:
byte lum = (byte)((r * 2126 + g * 7152 + b * 722) / 10000);",
                    ComplexityAndTips = "• Complexidade: O(1) por pixel. LUTs podem pré-computar transformações.",
                    WhereToTest = "Aba PDI -> Seção 1: 'Modelos de Cor & Canais'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "PixelFormats.Bgra32 Property (System.Windows.Media)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.pixelformats.bgra32",
                            Description = "Especificação do formato Bgra32 com 32 bits por pixel e suporte nativo a canal alfa."
                        },
                        new DocReference
                        {
                            Title = "Estrutura Color (System.Windows.Media)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.color",
                            Description = "Estrutura padrão do WPF para representação de cores com canais A, R, G, B."
                        }
                    }
                },
                #endregion

                #region 3. Convoluções & Filtros Espaciais
                new StudyTopic
                {
                    Id = "spatial_convolution",
                    Category = "3. Filtros Espaciais & Convoluções",
                    Title = "Convolução Espacial 2D, Gaussiano, Unsharp Mask e Mediana",
                    Summary = "Filtragem linear no domínio espacial através de máscaras/kernels e filtros não-lineares de ordenação.",
                    MathFormulas =
                        "• Equação da Convolução Discreta 2D:\n" +
                        "  g(x, y) = \\sum_{u=-k}^{k} \\sum_{v=-k}^{k} f(x - u, y - v) · K(u, v)\n\n" +
                        "• Distribuição Gaussiana 2D (Filtro Passa-Baixa):\n" +
                        "  G(x, y) = \\frac{1}{2\\pi\\sigma^2} e^{-\\frac{x^2 + y^2}{2\\sigma^2}}\n\n" +
                        "• Máscara de Desfoque (Unsharp Masking):\n" +
                        "  Resultado = Original + Ganho · (Original - Gaussiano)",
                    CodeExplanation =
                        "1. O kernel K desliza sobre a imagem. Para cada pixel central (x, y), multiplica seus vizinhos pelos pesos correspondentes da matriz.\n" +
                        "2. Tratamento de borda com Clamp: Math.Clamp(x + kx, 0, width - 1) para evitar índices fora do buffer.\n" +
                        "3. Filtro da Mediana (Não-Linear): Coleta a vizinhança numa lista, ordena (Array.Sort) e seleciona o elemento central. Não borra bordas e elimina ruído Sal & Pimenta perfeitamente.",
                    CodeSnippet =
@"// Convolução paralela genérica 2D:
Parallel.For(0, height, y => {
    for (int x = 0; x < width; x++) {
        double sumR = 0, sumG = 0, sumB = 0;
        for (int ky = -r; ky <= r; ky++) {
            for (int kx = -r; kx <= r; kx++) {
                byte* p = srcRow + (px * 4);
                double w = kernel[ky + r, kx + r];
                sumB += p[0] * w; sumG += p[1] * w; sumR += p[2] * w;
            }
        }
        dstRow[x * 4 + 0] = (byte)Math.Clamp(sumB / divisor + bias, 0, 255);
    }
});",
                    ComplexityAndTips = "• Complexidade: O(W * H * K^2). Filtros Gaussianos grandes podem ser separados em 1D horizontal + 1D vertical reduzindo para O(W * H * 2K).",
                    WhereToTest = "Aba PDI -> Seção 4: 'Filtros Espaciais (Convoluções)'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Parallel (System.Threading.Tasks)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.threading.tasks.parallel",
                            Description = "Execução de laços paralelos em múltiplos núcleos de CPU com particionamento automático de carga."
                        },
                        new DocReference
                        {
                            Title = "Método Math.Clamp no .NET",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.math.clamp",
                            Description = "Tratamento de contorno e limites numéricos sem desvios condicionais lentos."
                        }
                    }
                },
                #endregion

                #region 4. Detecção de Bordas & Canny
                new StudyTopic
                {
                    Id = "edge_canny",
                    Category = "4. Detecção de Bordas & Visão",
                    Title = "Operadores de Gradiente (Sobel, Scharr, Laplace) e Algoritmo Canny Completo",
                    Summary = "Detecção de contornos através da primeira e segunda derivada espacial e o algoritmo Canny em 5 etapas.",
                    MathFormulas =
                        "• Operador Sobel (Primeira Derivada Discreta):\n" +
                        "  Gx = [[-1, 0, 1], [-2, 0, 2], [-1, 0, 1]] * f\n" +
                        "  Gy = [[-1, -2, -1], [ 0,  0,  0], [ 1,  2,  1]] * f\n" +
                        "  Magnitude: G = \\sqrt{Gx^2 + Gy^2}\n" +
                        "  Orientação: \\theta = \\text{atan2}(Gy, Gx)\n\n" +
                        "• Operador Laplaciano (Segunda Derivada Isocrônica):\n" +
                        "  \\nabla^2 f = \\frac{\\partial^2 f}{\\partial x^2} + \\frac{\\partial^2 f}{\\partial y^2}\n" +
                        "  Kernel 8-vizinhos: [[-1, -1, -1], [-1, 8, -1], [-1, -1, -1]]",
                    CodeExplanation =
                        "PIPELINE DO ALGORITMO CANNY (5 ETAPAS):\n" +
                        "1. Suavização Gaussiana: Remove ruídos de alta frequência que causariam falsas bordas.\n" +
                        "2. Cálculo do Gradiente: Obtém magnitude G e direção angular θ em cada pixel via Sobel.\n" +
                        "3. Supressão de Não-Máximos (NMS): Compara o pixel com seus dois vizinhos ao longo da reta do gradiente (setores 0°, 45°, 90°, 135°). Se não for o pico local, zera o valor (afina as bordas para 1 pixel de espessura).\n" +
                        "4. Limiarização Dupla (Double Threshold): Classifica em Bordas Fortes (>= HighThreshold) e Bordas Fracas (>= LowThreshold).\n" +
                        "5. Rastreamento por Histerese: Executa busca em largura/profundidade (BFS). Bordas fracas conectadas a bordas fortes são preservadas; ilhas isoladas são descartadas.",
                    CodeSnippet =
@"// Histerese de Canny com fila BFS:
while (edgeQueue.Count > 0) {
    int curr = edgeQueue.Dequeue();
    int cy = curr / width, cx = curr % width;
    for (int k = 0; k < 8; k++) {
        int nx = cx + dx[k], ny = cy + dy[k];
        int nIdx = ny * width + nx;
        if (result[nIdx] == WEAK_EDGE) {
            result[nIdx] = STRONG_EDGE;
            edgeQueue.Enqueue(nIdx); // Conecta e propaga
        }
    }
}",
                    ComplexityAndTips = "• Complexidade: O(W * H). Considerado o algoritmo padrão-ouro para segmentação de contornos.",
                    WhereToTest = "Aba PDI -> Seção 5: 'Detecção de Bordas & Gradientes'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Queue<T> (System.Collections.Generic)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.collections.generic.queue-1",
                            Description = "Fila FIFO de alta performance para busca em largura (BFS) na histerese do Canny."
                        },
                        new DocReference
                        {
                            Title = "Otimização de Métodos com AggressiveInlining",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.runtime.compilerservices.methodimploptions",
                            Description = "Como orientar o compilador JIT a embutir rotinas críticas de cálculo de gradiente."
                        }
                    }
                },
                #endregion

                #region 5. Histograma & Morfologia
                new StudyTopic
                {
                    Id = "hist_morph",
                    Category = "5. Histograma & Morfologia",
                    Title = "Equalização de Histograma (CDF), Limiarização de Otsu e Morfologia Matemática",
                    Summary = "Maximização de contraste global, binarização automática ótima e teoria dos conjuntos morfológica.",
                    MathFormulas =
                        "• Função de Distribuição Acumulada (CDF) e Equalização:\n" +
                        "  CDF(i) = \\sum_{j=0}^{i} P(j)\n" +
                        "  h_{eq}(v) = \\text{round}\\left( \\frac{CDF(v) - CDF_{min}}{(W \\times H) - CDF_{min}} \\times 255 \\right)\n\n" +
                        "• Critério de Variância Inter-Classes de Otsu:\n" +
                        "  \\sigma_B^2(t) = \\omega_0(t) \\cdot \\omega_1(t) \\cdot [\\mu_0(t) - \\mu_1(t)]^2\n" +
                        "  Onde \\omega_0, \\omega_1 são os pesos acumulados e \\mu_0, \\mu_1 são as médias das classes.\n\n" +
                        "• Operadores Morfológicos:\n" +
                        "  Erosão: (A \\ominus B)(x, y) = \\min_{(i,j) \\in B} f(x+i, y+j)\n" +
                        "  Dilatação: (A \\oplus B)(x, y) = \\max_{(i,j) \\in B} f(x-i, y-j)\n" +
                        "  Abertura: (A \\ominus B) \\oplus B  (Remove pequenos ruídos claros)\n" +
                        "  Fechamento: (A \\oplus B) \\ominus B  (Preenche buracos escuros)",
                    CodeExplanation =
                        "1. A Equalização de Histograma estende a faixa dinâmica da imagem para que a distribuição de tons se torne uniforme.\n" +
                        "2. O algoritmo de Otsu busca o limiar T* de 0 a 255 que maximiza a separação estatística entre o fundo e os objetos.\n" +
                        "3. A Morfologia opera investigando a imagem com um elemento estruturante B (Quadrado 3x3, Cruz ou Disco 5x5).",
                    CodeSnippet =
@"// Erosão Morfológica em Escala de Cinza:
byte minVal = 255;
for (int ky = -radius; ky <= radius; ky++) {
    for (int kx = -radius; kx <= radius; kx++) {
        if (se[ky + radius, kx + radius]) {
            byte val = srcPixel[kx, ky];
            if (val < minVal) minVal = val;
        }
    }
}
dstPixel = minVal;",
                    ComplexityAndTips = "• Complexidade Otsu: O(W*H + 256). Extremamente eficiente para segmentação automática.",
                    WhereToTest = "Aba PDI -> Seção 3 e Seção 6.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Array (System.Array)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.array",
                            Description = "Alocação e manipulação de vetores contíguos para cálculo de histogramas de 256 posições."
                        }
                    }
                },
                #endregion

                #region 6. Rasterização 2D dos Primeiros Princípios
                new StudyTopic
                {
                    Id = "raster_2d",
                    Category = "6. Rasterização 2D (Primeiros Princípios)",
                    Title = "Algoritmo de Reta de Bresenham, Círculo Ponto Médio, Curvas de Bézier e Scanline Fill",
                    Summary = "Como transformar primitivas vetoriais geométricas em matrizes de pixels usando matemática discreta.",
                    MathFormulas =
                        "• Reta de Bresenham (Aritmética 100% Inteira):\n" +
                        "  e = 2\\Delta y - \\Delta x\n" +
                        "  Se e >= 0: y = y + sy; e = e + 2(\\Delta y - \\Delta x)\n" +
                        "  Se e < 0:  e = e + 2\\Delta y\n\n" +
                        "• Círculo do Ponto Médio (Simetria em 8 Octantes):\n" +
                        "  Variável de decisão: d = 1 - r\n" +
                        "  Se d < 0: d = d + 2x + 3\n" +
                        "  Se d >= 0: y = y - 1; d = d + 2(x - y) + 5\n\n" +
                        "• Curva de Bézier Cúbica (Polinômio de Bernstein):\n" +
                        "  B(t) = (1-t)^3 P_0 + 3(1-t)^2 t P_1 + 3(1-t)t^2 P_2 + t^3 P_3, \\quad t \\in [0, 1]\n\n" +
                        "• Recorte Cohen-Sutherland (Outcodes de 4 bits):\n" +
                        "  Top (1000), Bottom (0100), Right (0010), Left (0001)",
                    CodeExplanation =
                        "1. Bresenham (1965): Elimina divisões e ponto flutuante usando apenas adições e subtrações, viabilizando GPUs primitivas.\n" +
                        "2. Círculo do Ponto Médio: Calcula apenas 1/8 do círculo (45 graus) e plota os 8 pontos simétricos (+-x, +-y) e (+-y, +-x).\n" +
                        "3. Scanline Polygon Fill: Ordena as arestas na Tabela de Arestas Ativas (AET) e preenche os spans de pixels entre pares de interseções com a regra da paridade (Par-Ímpar).\n" +
                        "4. Cohen-Sutherland: Testa se ambos os pontos estão dentro da janela (code0 | code1 == 0) ou fora no mesmo lado (code0 & code1 != 0), recortando analiticamente quando necessário.",
                    CodeSnippet =
@"// Reta de Bresenham Clássica:
int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
int err = (dx > dy ? dx : -dy) / 2;

while (true) {
    bmp.SetPixel(x0, y0, color);
    if (x0 == x1 && y0 == y1) break;
    int e2 = err;
    if (e2 > -dx) { err -= dy; x0 += sx; }
    if (e2 < dy) { err += dx; y0 += sy; }
}",
                    ComplexityAndTips = "• Complexidade: Bresenham é O(max(Δx, Δy)). É a base da rasterização de triângulos em placas gráficas.",
                    WhereToTest = "Aba '✏️ Computação Gráfica 2D (Rasterização)'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Point (System.Windows)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.point",
                            Description = "Representação de coordenadas cartesianas (X, Y) no espaço bidimensional."
                        },
                        new DocReference
                        {
                            Title = "Matrizes de Transformação 2D no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.matrix",
                            Description = "Estrutura Matrix 3x3 homogênea para transformações afins no WPF."
                        }
                    }
                },
                #endregion

                #region 7. Computação Gráfica 3D & Pipeline
                new StudyTopic
                {
                    Id = "cg_3d_pipeline",
                    Category = "7. Computação Gráfica 3D & Pipeline",
                    Title = "Pipeline 3D Completo, Matrizes MVP, Z-Buffering, Iluminação Phong e Câmeras",
                    Summary = "A jornada matemática do vértice 3D no espaço de objeto até o pixel 2D na tela.",
                    MathFormulas =
                        "• Transformação MVP (Model -> View -> Projection):\n" +
                        "  v_{clip} = M_{proj} \\times M_{view} \\times M_{model} \\times v_{local}\n\n" +
                        "• Divisão Perspectiva (NDC [-1, 1]):\n" +
                        "  v_{ndc} = (x / w, \\; y / w, \\; z / w)\n\n" +
                        "• Descarte de Faces Ocultas (Back-face Culling):\n" +
                        "  \\vec{N}_{face} \\cdot \\vec{V}_{view} < 0 \\implies \\text{Face voltada para frente (visível)}\n\n" +
                        "• Modelo de Iluminação de Phong / Blinn-Phong:\n" +
                        "  I = I_a k_a + I_d k_d (\\vec{N} \\cdot \\vec{L}) + I_s k_s (\\vec{N} \\cdot \\vec{H})^{\\alpha}\n" +
                        "  Onde \\vec{H} = \\frac{\\vec{L} + \\vec{V}}{|\\vec{L} + \\vec{V}|} é o half-vector de Blinn.",
                    CodeExplanation =
                        "ETAPAS DO PIPELINE GRÁFICO 3D:\n" +
                        "1. Espaço de Objeto -> Mundo (Model Matrix): Aplica translação, rotação e escala do modelo.\n" +
                        "2. Espaço de Mundo -> Câmera (View Matrix / LookAt): Transforma a cena para a perspectiva dos olhos do observador.\n" +
                        "3. Espaço de Câmera -> Projeção (Projection Matrix): Aplica o cone de visão (Frustum) e escala os objetos distantes por 1/Z (Perspectiva) ou mantém paralelos (Ortográfica).\n" +
                        "4. Divisão Perspectiva: Divide por W para obter coordenadas normalizadas (NDC [-1, 1]).\n" +
                        "5. Mapeamento para Tela (Viewport): Converte NDC para coordenadas reais de pixels em tela (ex: 1920x1080).\n" +
                        "6. Rasterização por Coordenadas Baricêntricas & Z-Buffer: Para cada pixel dentro do triângulo, interpola a profundidade Z. Se for mais próximo que o valor no Z-Buffer, desenha e atualiza o buffer.",
                    CodeSnippet =
@"// Teste de Z-Buffer com Coordenadas Baricêntricas:
float z = (float)(w0 * z0 + w1 * z1 + w2 * z2);
int zIdx = y * width + x;
if (z < zBuffer[zIdx]) {
    zBuffer[zIdx] = z; // Atualiza profundidade
    row[x] = colorBgra; // Escreve cor
}",
                    ComplexityAndTips = "• Complexidade: O(Triângulos * Pixels/Triângulo). Paralelizável nativamente em GPUs.",
                    WhereToTest = "Aba '🧊 CG 3D' e Aba '⚡ Software 3D & Ray Tracing'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão geral de gráficos 3D no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/3-d-graphics-overview",
                            Description = "Guia completo de geometria 3D, câmeras, luzes e materiais acelerados por hardware no WPF."
                        },
                        new DocReference
                        {
                            Title = "System.Numerics.Matrix4x4 Struct",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.numerics.matrix4x4",
                            Description = "Matriz de transformação 4x4 otimizada com aceleração por instruções de hardware SIMD."
                        },
                        new DocReference
                        {
                            Title = "Classe MeshGeometry3D (System.Windows.Media.Media3D)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.meshgeometry3d",
                            Description = "Construção de malhas triangulares através de posições, normais e índices de triângulos."
                        }
                    }
                },
                #endregion

                #region 8. Modelagem Hierárquica & Cinemática
                new StudyTopic
                {
                    Id = "hierarchical_modeling",
                    Category = "8. Modelagem Hierárquica (Unidade 3)",
                    Title = "Grafos de Cena (Scene Graph), Design Top-Down e Cinemática Direta (Robô Articulado)",
                    Summary = "Composição de objetos complexos articulados e propagação de transformações geométricas pai-filho.",
                    MathFormulas =
                        "• Propagação Matricial em Árvore (Scene Graph):\n" +
                        "  M_{global, filho} = M_{global, pai} \\times M_{local, filho}\n\n" +
                        "• Cinemática Direta do Braço Robótico (4 Níveis):\n" +
                        "  M_{garra} = T_{base} \\times R_y(\\theta_{base}) \\times T_{ombro} \\times R_z(\\theta_{ombro}) \\times T_{cotovelo} \\times R_z(\\theta_{cotovelo}) \\times R_x(\\theta_{pulso})",
                    CodeExplanation =
                        "1. Motivação: Em vez de recalcular manualmente a posição absoluta de cada articulação, criamos uma hierarquia onde cada parte é filha da anterior.\n" +
                        "2. Ao girar a base, o braço, antebraço e garras giram juntos automaticamente.\n" +
                        "3. Ao dobrar o ombro, o cotovelo e garra acompanham o movimento perfeitamente sem deformar seus comprimentos.",
                    CodeSnippet =
@"// Criação de Nós Hierárquicos no WPF:
SceneNode3D baseNode = new SceneNode3D(""Base"");
SceneNode3D armNode = new SceneNode3D(""Braco"");
SceneNode3D forearmNode = new SceneNode3D(""Antebraco"");

baseNode.AddChild(armNode);
armNode.AddChild(forearmNode);
// A matriz de transformação do pai afeta automaticamente os filhos!",
                    ComplexityAndTips = "• Design Top-Down: Planeja o sistema completo. Construção Bottom-Up: Monta as peças primitivas e conecta nas juntas.",
                    WhereToTest = "Aba '🧊 Computação Gráfica 3D' -> Seção 3: 'Modelagem Hierárquica (Unidade 3)'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Transform3DGroup (System.Windows.Media.Media3D)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.transform3dgroup",
                            Description = "Agrupamento e composição de transformações hierárquicas compostas no WPF."
                        },
                        new DocReference
                        {
                            Title = "Classe Model3DGroup (System.Windows.Media.Media3D)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.model3dgroup",
                            Description = "Coleção de modelos tridimensionais tratados como uma única unidade lógica."
                        }
                    }
                },
                #endregion

                #region 9. Ray Tracing Realístico
                new StudyTopic
                {
                    Id = "ray_tracing",
                    Category = "9. Renderização Realística & Ray Tracing",
                    Title = "Traçado de Raios (Whitted Ray Tracer), Sombras, Reflexões e Refração de Snell",
                    Summary = "Simulação física do transporte de luz traçando raios da câmera até as fontes luminosas.",
                    MathFormulas =
                        "• Equação do Raio: \\vec{r}(t) = \\vec{O} + t \\cdot \\vec{D}, \\quad t > 0\n\n" +
                        "• Interseção Raio-Esfera (|\\vec{O} + t\\vec{D} - \\vec{C}|^2 = R^2):\n" +
                        "  a t^2 + b t + c = 0 \\implies t = \\frac{-b \\pm \\sqrt{b^2 - 4ac}}{2a}\n\n" +
                        "• Reflexão Especular:\n" +
                        "  \\vec{R} = \\vec{D} - 2(\\vec{D} \\cdot \\vec{N})\\vec{N}\n\n" +
                        "• Lei de Refração de Snell (Vidro/Água):\n" +
                        "  n_1 \\sin(\\theta_1) = n_2 \\sin(\\theta_2)",
                    CodeExplanation =
                        "1. Raio Primário: Sai da câmera passando por cada pixel da tela em direção à cena.\n" +
                        "2. Teste de Interseção: Encontra o objeto mais próximo (menor t > 0).\n" +
                        "3. Raio de Sombra (Shadow Ray): Traça um raio do ponto de impacto até cada luz. Se houver obstáculo, o ponto fica na sombra.\n" +
                        "4. Reflexão Recursiva: Se o material for reflexivo (espelho/cromo), lança um novo raio na direção refletida R.\n" +
                        "5. Refração: Se o material for transparente (vidro), curva o raio pela Lei de Snell e calcula a mistura de Fresnel.",
                    CodeSnippet =
@"// Traçado de raio recursivo com reflexão:
if (mat.Reflectivity > 0 && depth < maxDepth) {
    Vec3 reflectDir = Vec3.Reflect(ray.Direction, hitNormal);
    Ray3D reflectRay = new Ray3D(hitPoint + hitNormal * 1e-4, reflectDir);
    Vec3 reflectColor = TraceRay(reflectRay, depth + 1);
    finalColor = Vec3.Lerp(finalColor, reflectColor, mat.Reflectivity);
}",
                    ComplexityAndTips = "• Complexidade: O(Pixels * Objetos * Profundidade_Rebatimento * Luzes). Gera fotorrealismo espetacular com reflexões e vidros perfeitos.",
                    WhereToTest = "Aba '⚡ Software 3D & Ray Tracing' -> Seção 2: 'Ray Tracer Matemático'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Estrutura Vector3 (System.Numerics)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.numerics.vector3",
                            Description = "Operações vetoriais de produto escalar, produto vetorial, reflexão e normalização."
                        },
                        new DocReference
                        {
                            Title = "Instruções Intrínsecas de Hardware no .NET (AVX/SSE)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.runtime.intrinsics.x86",
                            Description = "Como o .NET utiliza instruções vetoriais da CPU para acelerar cálculos matemáticos de Ray Tracing."
                        }
                    }
                },
                #endregion

                #region 10. Arquitetura do Software & DirectX Tier
                new StudyTopic
                {
                    Id = "arch_wpf_directx",
                    Category = "10. Arquitetura & Aceleração GPU",
                    Title = "Arquitetura do Software, Subsistema milcore e DirectX Rendering Tier",
                    Summary = "Como o WPF orquestra a comunicação entre a CPU gerenciada e a placa de vídeo via DirectX.",
                    MathFormulas =
                        "• Hierarquia de Renderização WPF:\n" +
                        "  C# Gerenciado (UI Thread) -> Render Thread -> milcore (C++ Não Gerenciado) -> Direct3D (GPU)\n\n" +
                        "• Níveis de Aceleração por Hardware (Graphics Tiers):\n" +
                        "  - Tier 0: Sem aceleração por hardware (DirectX < 9.0 ou desativado).\n" +
                        "  - Tier 1: Aceleração parcial por hardware (DirectX 9.0 Shader Model 2.0).\n" +
                        "  - Tier 2: Aceleração completa por hardware (DirectX 9.0Ex / 11 / Shader Model 3.0+).",
                    CodeExplanation =
                        "1. O CGPDI.StudyLab desacopla a interface do usuário (XAML) do motor de computação gráfica.\n" +
                        "2. Os pixels gerados no WriteableBitmap são copiados diretamente para superfícies de textura Direct3D sem passar pela CPU intermediária do Windows GDI.\n" +
                        "3. A medição de desempenho é realizada através de contadores de alta frequência (QueryPerformanceCounter) via System.Diagnostics.Stopwatch.",
                    CodeSnippet =
@"// Consulta do nível de aceleração por hardware da GPU:
int renderingTier = (RenderCapability.Tier >> 16);
if (renderingTier >= 2)
{
    // Aceleração total por hardware ativada (Tier 2)!
}",
                    ComplexityAndTips = "• Dica: Manter zero alocações (GC Gen0 = 0) garante taxa constante de 60 FPS sem engasgos de renderização.",
                    WhereToTest = "HUD de Status no rodapé do aplicativo com medição de tempo em tempo real.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Níveis de Renderização de Gráficos (Graphics Tiers)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/graphics-rendering-tiers",
                            Description = "Documentação oficial dos níveis de aceleração por hardware e recursos de GPU no WPF."
                        },
                        new DocReference
                        {
                            Title = "Classe RenderCapability (System.Windows.Media)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rendercapability",
                            Description = "API para inspeção do nível de suporte gráfico do hardware em tempo de execução."
                        }
                    }
                },
                #endregion

                #region 11. Guia Acadêmico & Trabalhos Práticos
                new StudyTopic
                {
                    Id = "academic_t1_t2_t3",
                    Category = "11. Plano de Ensino & Avaliações",
                    Title = "Roteiro de Estudos para os Trabalhos Acadêmicos (T1, T2 e T3)",
                    Summary = "Guia passo a passo para preparação e desenvolvimento das avaliações práticas da disciplina universitária.",
                    MathFormulas =
                        "• Divisão do Conteúdo Programático:\n" +
                        "  - T1: Processamento Digital de Imagens (Canais, Filtros Convolucionais, Canny, Histograma e Morfologia).\n" +
                        "  - T2: Computação Gráfica 2D e 3D (Bresenham, Bézier, MVP, Pipeline em Software e Z-Buffer).\n" +
                        "  - T3: Modelagem Hierárquica (Grafos de Cena, Robô Articulado) e Ray Tracing Fotorrealista.",
                    CodeExplanation =
                        "1. Para o Trabalho T1: Explore os módulos em ImageProcessing/ e teste cada filtro na Aba PDI.\n" +
                        "2. Para o Trabalho T2: Analise Graphics2D/Rasterizer2D.cs e Graphics3D/SoftwareRenderer3D.cs para entender a rasterização.\n" +
                        "3. Para o Trabalho T3: Experimente a articulação do robô em Graphics3D/HierarchicalModeling.cs e o traçado de raios em Graphics3D/Raytracer3D.cs.",
                    CodeSnippet =
@"// Roteiro de estudos interativo:
// Navegue pelas abas superiores do programa e compare os resultados
// visuais com as fórmulas matemáticas da documentação!",
                    ComplexityAndTips = "• Consulte o site online para explicações teóricas detalhadas com analogias do dia a dia.",
                    WhereToTest = "Aba '📖 Central de Estudos' e documentação online.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Roteiros de Aprendizagem do .NET no Microsoft Learn",
                            Url = "https://learn.microsoft.com/pt-br/training/dotnet/",
                            Description = "Cursos e trilhas de aprendizagem oficiais gratuitas da Microsoft."
                        },
                        new DocReference
                        {
                            Title = "Documentação Oficial do .NET",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/",
                            Description = "Portal principal com tutoriais, guias conceituais e documentação de APIs."
                        }
                    }
                },
                #endregion

                #region 12. Estúdio Interativo de Código & Compilador Roslyn
                new StudyTopic
                {
                    Id = "roslyn_live_studio",
                    Category = "12. Estúdio de Código & Roslyn",
                    Title = "Estúdio de Código C#, Compilação Roslyn & Renderização Dinâmica em Tempo Real",
                    Summary = "Como o StudyLab utiliza o Microsoft.CodeAnalysis (Roslyn) para compilar, testar e renderizar dinamicamente algoritmos escritos pelo aluno em tempo de execução.",
                    MathFormulas =
                        "• Pipeline de Execução Dinâmica:\n" +
                        "  Código C# do Aluno -> Compilação Roslyn (CSharpScript.EvaluateAsync) -> Testes Unitários -> Injeção Gráfica em DirectBitmap -> Renderização a 60 FPS\n\n" +
                        "• Isolamento e Desempenho:\n" +
                        "  - Tempo médio de compilação e execução de testes: < 80 ms.\n" +
                        "  - Renderização direta no DirectBitmap usando o retorno do código do aluno.\n" +
                        "  - Feedback instantâneo no Canvas e no mapa de memória RAM.",
                    CodeExplanation =
                        "1. O editor de código C# permite que o aluno escreva do zero ou modifique valores, fórmulas e matrizes.\n" +
                        "2. O motor Roslyn avalia a função C# e extrai os resultados matemáticos e gráficos.\n" +
                        "3. O Canvas e as células de memória RAM são redesenhados imediatamente com base nos valores retornados pelo código do aluno.\n" +
                        "4. Baterias de testes unitários automatizados fornecem diagnóstico pedagógico em tempo real.",
                    CodeSnippet =
@"// Exemplo de compilação dinâmica com Roslyn Scripting:
var scriptOptions = ScriptOptions.Default
    .WithReferences(typeof(System.Math).Assembly)
    .WithImports(""System"", ""System.Math"");

var resultado = await CSharpScript.EvaluateAsync<uint>(userCode, scriptOptions);
// O valor retornado é injetado diretamente nos pixels do DirectBitmap!",
                    ComplexityAndTips = "• Dica: Experimente alterar os canais de cor, fatores de escala ou matrizes para ver o resultado visual mudar na hora no Canvas!",
                    WhereToTest = "Aba '🎓 Trilha & Estúdio C#' e botão '🗖 Estúdio em Nova Janela'.",
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão Geral dos Compiladores .NET (Roslyn APIs)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/roslyn-overview",
                            Description = "Guia oficial da plataforma de compiladores .NET e APIs de análise sintática e semântica."
                        },
                        new DocReference
                        {
                            Title = "Microsoft.CodeAnalysis.CSharp.Scripting Namespace",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/microsoft.codeanalysis.csharp.scripting",
                            Description = "Documentação oficial da biblioteca de execução e avaliação de scripts C# em tempo de execução."
                        }
                    }
                }
                #endregion
            };
        }
    }
}
