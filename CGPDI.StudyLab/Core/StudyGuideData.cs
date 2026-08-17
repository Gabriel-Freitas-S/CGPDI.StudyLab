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

    public class StudyQuiz
    {
        public string Question { get; set; } = "";
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; } = 0;
        public string Explanation { get; set; } = "";
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
        public int TargetLessonNumber { get; set; } = 1;
        public StudyQuiz? Quiz { get; set; }
        public List<DocReference> MicrosoftReferences { get; set; } = new List<DocReference>();
    }

    /// <summary>
    /// Base de conhecimento e documentação pedagógica completa integrada ao software.
    /// Explica passo a passo a teoria matemática, a implementação em C# e fornece referências oficiais da Microsoft Learn.
    /// </summary>
    public static class StudyGuideData
    {
        private const string Category3D = "6. Computação Gráfica 3D (Pipeline MVP & Iluminação)";

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
                    TargetLessonNumber = 1,
                    MathFormulas = 
                        "• Cálculo do Endereço de Memória do Pixel (x, y) no Buffer BGRA32:\n" +
                        "  Endereço = BaseBuffer + (y · Stride) + (x · 4)\n\n" +
                        "• Onde:\n" +
                        "  - BaseBuffer: Ponteiro bruto para o byte 0 da imagem na RAM.\n" +
                        "  - Stride (Largura em Bytes): Width · 4 (alinhado a 4 ou 16 bytes pela GPU).\n" +
                        "  - 4 Bytes por Pixel: Byte 0 = Azul (B), Byte 1 = Verde (G), Byte 2 = Vermelho (R), Byte 3 = Alpha (A).\n\n" +
                        "• Aceleração de Hardware:\n" +
                        "  WriteableBitmap.Lock() → Escrita em ponteiro byte* → WriteableBitmap.AddDirtyRect() → Unlock()",
                    CodeExplanation =
                        "1. O método Bitmap.Lock() bloqueia o buffer traseiro do WriteableBitmap para evitar que o Garbage Collector (GC) o mova durante a execução.\n" +
                        "2. Obtemos um ponteiro bruto (byte*) via Bitmap.BackBuffer.ToPointer() sem overhead de marshaling.\n" +
                        "3. Usamos Parallel.For(0, Height, y => ...) para paralelizar o processamento em todas as CPUs disponíveis.\n" +
                        "4. Bitmap.AddDirtyRect() notifica o DirectX do WPF que a área foi alterada para redesenho com aceleração de hardware nativa.",
                    CodeSnippet =
@"public unsafe class DirectBitmap
{
    // Acesso direto via ponteiro sem overhead
    byte* pixelPtr = _backBuffer + (y * Stride) + (x * 4);
    byte b = pixelPtr[0]; // Canal Azul (B)
    byte g = pixelPtr[1]; // Canal Verde (G)
    byte r = pixelPtr[2]; // Canal Vermelho (R)
    byte a = pixelPtr[3]; // Canal Alpha (A)
}",
                    ComplexityAndTips = "• Complexidade: O(W · H) com paralelismo multinúcleo O((W · H) / N_cores). Mais de 100x mais rápido que GetPixel do GDI+.",
                    WhereToTest = "Base de todo o processamento de imagens (PDI), Rasterização 2D e Ray Tracer.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Por que o cálculo do endereço de memória de um pixel utiliza (y * Stride + x * 4) no formato BGRA32?",
                        Options = new List<string>
                        {
                            "Porque a imagem é armazenada como uma matriz unidimensional contígua, onde cada linha tem tamanho Stride e cada pixel ocupa 4 bytes (B, G, R, A).",
                            "Porque cada pixel ocupa 1 byte e o Stride representa a altura da imagem.",
                            "Porque o Garbage Collector exige que o índice X seja multiplicado por 4 para alinhamento de 64 bits."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Na memória RAM/VRAM, uma imagem 2D é um bloco contíguo de bytes. Cada linha ocupa 'Stride' bytes e cada pixel no formato BGRA32 possui 4 bytes (Azul, Verde, Vermelho, Alpha)."
                    },
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
                    TargetLessonNumber = 2,
                    MathFormulas =
                        "• Luminância sRGB Perceptiva (ITU-R BT.709):\n" +
                        "  Y = 0.2126·R + 0.7152·G + 0.0722·B\n\n" +
                        "• Luminância NTSC/PAL (ITU-R BT.601):\n" +
                        "  Y = 0.299·R + 0.587·G + 0.114·B\n\n" +
                        "• Espaço Cilíndrico HSV (Hue, Saturation, Value):\n" +
                        "  V = max(R, G, B)\n" +
                        "  S = (V - min(R, G, B)) / V  (se V > 0)\n" +
                        "  H = 60° · ((G - B) / Δ mod 6)  [se max = R]\n\n" +
                        "• Separação de Crominância YCbCr (Padrão JPEG/MPEG):\n" +
                        "  Y  =  0.299·R + 0.587·G + 0.114·B\n" +
                        "  Cb = 128 - 0.1687·R - 0.3313·G + 0.5·B\n" +
                        "  Cr = 128 + 0.5·R - 0.4187·G - 0.0813·B",
                    CodeExplanation =
                        "1. O olho humano possui 3 tipos de cones: L (vermelho), M (verde) e S (azul). Temos quase o dobro de cones sensíveis ao espectro verde.\n" +
                        "2. Por isso, a conversão para escala de cinza dá mais de 71% de peso ao canal Verde no padrão sRGB (BT.709).\n" +
                        "3. No modelo YCbCr, a visão humana é menos sensível a variações de cor do que de brilho, permitindo descartar metade da informação de crominância (Chroma Subsampling 4:2:0 em JPEG/MPEG).",
                    CodeSnippet =
@"// Conversão rápida de luminância inteira sem ponto flutuante:
byte lum = (byte)((r * 2126 + g * 7152 + b * 722) / 10000);",
                    ComplexityAndTips = "• Complexidade: O(1) por pixel. LUTs (Look-Up Tables) podem pré-computar transformações instantâneas.",
                    WhereToTest = "Aba PDI -> Seção 1: 'Modelos de Cor & Canais'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Por que o canal Verde (G) recebe o maior peso (71.52%) na conversão para escala de cinza no padrão ITU-R BT.709?",
                        Options = new List<string>
                        {
                            "Porque os fotorreceptores da retina humana (cones M) possuem pico de sensibilidade na faixa do espectro verde.",
                            "Porque o canal Verde ocupa mais memória do que os canais Vermelho e Azul no formato BGRA32.",
                            "Porque as placas de vídeo não conseguem processar o canal Azul com precisão de 8 bits."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! A resposta fototópica do sistema visual humano é muito mais sensível aos comprimentos de onda correspondentes à luz verde, conferindo-lhe maior luminância percebida."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "PixelFormats.Bgra32 Property (System.Windows.Media)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.pixelformats.bgra32",
                            Description = "Especificação do formato Bgra32 com 32 bits por pixel e suporte nativo a canal alfa."
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
                    TargetLessonNumber = 3,
                    MathFormulas =
                        "• Equação da Convolução Discreta 2D:\n" +
                        "  g(x, y) = ∑_{u=-k}^{k} ∑_{v=-k}^{k} f(x - u, y - v) · K(u, v)\n\n" +
                        "• Distribuição Gaussiana 2D (Filtro Passa-Baixa Suavizador):\n" +
                        "  G(x, y) = (1 / (2πσ²)) · exp(-(x² + y²) / (2σ²))\n\n" +
                        "• Máscara de Nitidez (Unsharp Masking):\n" +
                        "  Resultado = Original + Ganho · (Original - Gaussiano)",
                    CodeExplanation =
                        "1. O kernel K desliza sobre a imagem. Para cada pixel central (x, y), multiplica seus vizinhos pelos pesos correspondentes da matriz.\n" +
                        "2. Tratamento de borda com Clamp: Math.Clamp(x + kx, 0, width - 1) para evitar índices fora do buffer.\n" +
                        "3. Filtro da Mediana (Não-Linear): Coleta a vizinhança numa lista, ordena e seleciona o elemento central. Não borra bordas e elimina ruído Sal & Pimenta perfeitamente.",
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
                    ComplexityAndTips = "• Complexidade: O(W · H · K²). Filtros Gaussianos grandes podem ser separados em 1D horizontal + 1D vertical reduzindo para O(W · H · 2K).",
                    WhereToTest = "Aba PDI -> Seção 4: 'Filtros Espaciais (Convoluções)'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual é a principal vantagem do Filtro da Mediana em relação ao Filtro de Média (Box Blur) ao remover ruído do tipo 'Sal e Pimenta'?",
                        Options = new List<string>
                        {
                            "O filtro da mediana preserva bordas nítidas porque não calcula a média matemática dos pixels discrepantes, substituindo o valor pelo termo central ordenado.",
                            "O filtro da mediana consome menos operações de CPU do que uma convolução linear 3x3.",
                            "O filtro da mediana utiliza aceleração direta de ponto flutuante na GPU sem necessidade de ordenação."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Por ser um filtro estatístico não-linear de ordenação, a mediana descarta os valores extremos (0 ou 255 causados pelo ruído) sem suavizar ou borrar as transições de bordas do objeto."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Parallel (System.Threading.Tasks)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.threading.tasks.parallel",
                            Description = "Execução de laços paralelos em múltiplos núcleos de CPU com particionamento automático de carga."
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
                    TargetLessonNumber = 4,
                    MathFormulas =
                        "• Operador Sobel (Primeira Derivada Discreta):\n" +
                        "  Gx = [[-1, 0, 1], [-2, 0, 2], [-1, 0, 1]] * f\n" +
                        "  Gy = [[-1, -2, -1], [ 0,  0,  0], [ 1,  2,  1]] * f\n" +
                        "  Magnitude: G = √(Gx² + Gy²)\n" +
                        "  Orientação: θ = atan2(Gy, Gx)\n\n" +
                        "• Operador Laplaciano (Segunda Derivada Isocrônica):\n" +
                        "  ∇²f = (∂²f / ∂x²) + (∂²f / ∂y²)\n" +
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
                    ComplexityAndTips = "• Complexidade: O(W · H). Considerado o algoritmo padrão-ouro para segmentação de contornos.",
                    WhereToTest = "Aba PDI -> Seção 5: 'Detecção de Bordas & Gradientes'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual é a função da etapa de 'Supressão de Não-Máximos' (NMS) no detector de bordas de Canny?",
                        Options = new List<string>
                        {
                            "Afinar as bordas detectadas para exatamente 1 pixel de espessura, mantendo apenas os picos locais na direção do gradiente.",
                            "Eliminar o ruído de alta frequência através de uma convolução Gaussiana.",
                            "Classificar os pixels em bordas fortes e fracas através de dois limiares."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! O NMS compara a magnitude do gradiente do pixel central com os vizinhos na direção perpendicular à borda; se o pixel central não for o máximo local, seu valor é suprimido a 0."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Queue<T> (System.Collections.Generic)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.collections.generic.queue-1",
                            Description = "Fila FIFO de alta performance para busca em largura (BFS) na histerese do Canny."
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
                    TargetLessonNumber = 5,
                    MathFormulas =
                        "• Função de Distribuição Acumulada (CDF) e Equalização:\n" +
                        "  CDF(i) = ∑_{j=0}^{i} P(j)\n" +
                        "  h_{eq}(v) = round(((CDF(v) - CDF_{min}) / ((W · H) - CDF_{min})) · 255)\n\n" +
                        "• Critério de Variância Inter-Classes de Otsu:\n" +
                        "  σ_B²(t) = ω_0(t) · ω_1(t) · [μ_0(t) - μ_1(t)]²\n" +
                        "  Onde ω_0, ω_1 são os pesos acumulados e μ_0, μ_1 são as médias das classes.\n\n" +
                        "• Operadores Morfológicos:\n" +
                        "  Erosão: (A ⊖ B)(x, y) = min_{(i,j) ∈ B} f(x+i, y+j)\n" +
                        "  Dilatação: (A ⊕ B)(x, y) = max_{(i,j) ∈ B} f(x-i, y-j)\n" +
                        "  Abertura: (A ⊖ B) ⊕ B  (Remove pequenos ruídos claros)\n" +
                        "  Fechamento: (A ⊕ B) ⊖ B  (Preenche buracos escuros)",
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
                    ComplexityAndTips = "• Complexidade Otsu: O(W · H + 256). Extremamente eficiente para segmentação automática.",
                    WhereToTest = "Aba PDI -> Seção 3 e Seção 6.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual operação morfológica é ideal para eliminar pequenos pontos de ruído brilhantes isolados sem alterar o tamanho geral dos objetos principais?",
                        Options = new List<string>
                        {
                            "Abertura (Erosão seguida de Dilatação com o mesmo elemento estruturante).",
                            "Fechamento (Dilatação seguida de Erosão).",
                            "Subtração linear de histograma."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! A Abertura elimina saliências finas e pequenos objetos brilhantes na etapa de erosão, restaurando o contorno dos objetos maiores na etapa de dilatação subsequente."
                    },
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
                    TargetLessonNumber = 6,
                    MathFormulas =
                        "• Reta de Bresenham (Aritmética 100% Inteira):\n" +
                        "  e = 2·Δy - Δx\n" +
                        "  Se e >= 0: y = y + sy; e = e + 2·(Δy - Δx)\n" +
                        "  Se e < 0:  e = e + 2·Δy\n\n" +
                        "• Círculo do Ponto Médio (Simetria em 8 Octantes):\n" +
                        "  Variável de decisão: d = 1 - r\n" +
                        "  Se d < 0: d = d + 2x + 3\n" +
                        "  Se d >= 0: y = y - 1; d = d + 2(x - y) + 5\n\n" +
                        "• Curva de Bézier Cúbica (Polinômio de Bernstein):\n" +
                        "  B(t) = (1-t)³·P0 + 3(1-t)²·t·P1 + 3(1-t)·t²·P2 + t³·P3,  t ∈ [0, 1]\n\n" +
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
                    WhereToTest = "Aba 'Rasterização 2D'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Por que o algoritmo de Bresenham foi um marco revolucionário na computação gráfica?",
                        Options = new List<string>
                        {
                            "Porque elimina completamente cálculos em ponto flutuante e divisões, operando exclusivamente com adições, subtrações e deslocamentos inteiros rápidos.",
                            "Porque foi o primeiro algoritmo a utilizar aceleração Ray Tracing em tempo real.",
                            "Porque calcula curvas tridimensionais sem precisar de matrizes de projeção."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Jack Bresenham demonstrou que a decisão de qual pixel vizinho acender pode ser tomada apenas rastreando o erro acumulado com aritmética inteira simples."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Point (System.Windows)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.point",
                            Description = "Representação de coordenadas cartesianas (X, Y) no espaço bidimensional."
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
                    TargetLessonNumber = 7,
                    MathFormulas =
                        "• Transformação MVP (Model -> View -> Projection):\n" +
                        "  v_{clip} = M_{proj} × M_{view} × M_{model} × v_{local}\n\n" +
                        "• Divisão Perspectiva (NDC [-1, 1]):\n" +
                        "  v_{ndc} = (x / w,  y / w,  z / w)\n\n" +
                        "• Descarte de Faces Ocultas (Back-face Culling):\n" +
                        "  N_{face} · V_{view} < 0 ⟹ Face voltada para frente (visível)\n\n" +
                        "• Modelo de Iluminação de Phong / Blinn-Phong:\n" +
                        "  I = I_a·k_a + I_d·k_d·(N · L) + I_s·k_s·(N · H)^α\n" +
                        "  Onde H = (L + V) / |L + V| é o half-vector de Blinn.",
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
                    ComplexityAndTips = "• Complexidade: O(Triângulos · Pixels/Triângulo). Paralelizável nativamente em GPUs.",
                    WhereToTest = "Aba 'Computação Gráfica 3D' e Aba 'Ray Tracing'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual é o propósito da 'Divisão Perspectiva' (dividir as coordenadas X, Y, Z pelo componente homogêneo W)?",
                        Options = new List<string>
                        {
                            "Produzir o efeito de escorço de perspectiva, fazendo com que objetos distantes (com maior valor de profundidade) pareçam menores na tela em NDC [-1, 1].",
                            "Eliminar os triângulos que estão de costas para a câmera (Back-face Culling).",
                            "Calcular a iluminação especular do modelo de Phong."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Na projeção perspectiva, a matriz 4x4 armazena a profundidade Z no componente W. Ao dividir X e Y por W, objetos com maior distância são projetados mais próximos do centro óptico da câmera."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão geral de gráficos 3D no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/3-d-graphics-overview",
                            Description = "Guia completo de geometria 3D, câmeras, luzes e materiais acelerados por hardware no WPF."
                        }
                    }
                },
                #endregion

                #region 8. Modelagem Hierárquica & Cinemática
                new StudyTopic
                {
                    Id = "hierarchical_modeling",
                    Category = "8. Modelagem Hierárquica & Cinemática",
                    Title = "Grafos de Cena (Scene Graph), Design Top-Down e Cinemática Direta (Robô Articulado)",
                    Summary = "Composição de objetos complexos articulados e propagação de transformações geométricas pai-filho.",
                    TargetLessonNumber = 8,
                    MathFormulas =
                        "• Propagação Matricial em Árvore (Scene Graph):\n" +
                        "  M_{global, filho} = M_{global, pai} × M_{local, filho}\n\n" +
                        "• Cinemática Direta do Braço Robótico (4 Níveis):\n" +
                        "  M_{garra} = T_{base} × R_y(θ_{base}) × T_{ombro} × R_z(θ_{ombro}) × T_{cotovelo} × R_z(θ_{cotovelo}) × R_x(θ_{pulso})",
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
                    WhereToTest = "Aba 'Computação Gráfica 3D' -> Seção 3: 'Modelagem Hierárquica'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Em um Grafo de Cena (Scene Graph), como a transformação de um nó pai afeta seus nós descendentes (filhos)?",
                        Options = new List<string>
                        {
                            "A matriz de transformação global do filho é obtida multiplicando a matriz acumulada do pai pela matriz local do filho, propagando o movimento automaticamente.",
                            "Os filhos não são afetados pelas rotações do pai, exigindo recálculo manual com trigonometria.",
                            "Apenas a translação é propagada; rotações são canceladas pelo Z-Buffer."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Na modelagem hierárquica, as transformações são acumuladas multiplicando matrizes ao longo dos ramos da árvore hierárquica."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Transform3DGroup (System.Windows.Media.Media3D)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.transform3dgroup",
                            Description = "Agrupamento e composição de transformações hierárquicas compostas no WPF."
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
                    TargetLessonNumber = 9,
                    MathFormulas =
                        "• Equação Paramétrica do Raio: r(t) = O + t · D,  t > 0\n\n" +
                        "• Interseção Raio-Esfera (|O + tD - C|² = R²):\n" +
                        "  a·t² + b·t + c = 0 ⟹ t = (-b ± √(b² - 4ac)) / (2a)\n\n" +
                        "• Reflexão Especular:\n" +
                        "  R = D - 2(D · N)N\n\n" +
                        "• Lei de Refração de Snell (Vidro/Água):\n" +
                        "  n1 · sin(θ1) = n2 · sin(θ2)",
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
                    ComplexityAndTips = "• Complexidade: O(Pixels · Objetos · Profundidade_Rebatimento · Luzes). Gera fotorrealismo espetacular com reflexões e vidros perfeitos.",
                    WhereToTest = "Aba 'Ray Tracing'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "No algoritmo de Ray Tracing clássico (Whitted), como é determinada a sombra de um ponto na superfície de um objeto?",
                        Options = new List<string>
                        {
                            "Lançando um 'Raio de Sombra' (Shadow Ray) do ponto de interseção até a fonte de luz; se outro objeto opaco interceptar esse raio antes da luz, o ponto está na sombra.",
                            "Consultando o valor no Z-Buffer para verificar se o ponto está atrás da câmera.",
                            "Calculando o produto vetorial entre a normal da superfície e o vetor da câmera."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! O teste de visibilidade direta entre o ponto iluminado e as fontes de luz gera sombras duras e precisas de forma analítica no Ray Tracing."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Estrutura Vector3 (System.Numerics)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.numerics.vector3",
                            Description = "Operações vetoriais de produto escalar, produto vetorial, reflexão e normalização."
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
                    TargetLessonNumber = 10,
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
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual componente de baixo nível do WPF é responsável por enviar comandos diretamente ao Direct3D na placa de vídeo?",
                        Options = new List<string>
                        {
                            "milcore (Media Integration Layer Core), executado em C++ nativo em uma thread de renderização dedicada separada da thread de interface (UI Thread).",
                            "Garbage Collector do .NET Framework.",
                            "O subsistema Windows GDI32 legado."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! O milcore é o motor gráfico não gerenciado em C++ que recebe as árvores de composição do WPF e compila chamadas nativas Direct3D para a GPU."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Níveis de Renderização de Gráficos (Graphics Tiers)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/graphics-rendering-tiers",
                            Description = "Documentação oficial dos níveis de aceleração por hardware e recursos de GPU no WPF."
                        }
                    }
                },
                #endregion

                #region 11. Guia de Estudos & Roteiro Integrado
                new StudyTopic
                {
                    Id = "academic_guide_integrated",
                    Category = "11. Guia de Estudos & Práticas",
                    Title = "[CONTEÚDO DE ESTUDO & TEORIA] Roteiro Integrado de Estudos e Práticas Aplicadas",
                    Summary = "Visão integrada conectando Processamento Digital de Imagens, Rasterização 2D, Computação Gráfica 3D, Modelagem Hierárquica e Ray Tracing.",
                    TargetLessonNumber = 11,
                    MathFormulas =
                        "• Estrutura Pedagógica Integrada:\n" +
                        "  - Processamento Digital de Imagens: Canais BGRA, Filtros Convolucionais, Canny, Histograma e Morfologia.\n" +
                        "  - Computação Gráfica 2D e 3D: Bresenham, Bézier, MVP, Pipeline em Software, Z-Buffer e Iluminação Difusa.\n" +
                        "  - Modelagem Hierárquica e Ray Tracing: Grafos de Cena, Cadeias Cinemáticas, Modelo de Phong e Reflexões Analíticas.",
                    CodeExplanation =
                        "1. Processamento Digital de Imagens: Explore os módulos em ImageProcessing/ e teste cada filtro na Aba PDI.\n" +
                        "2. Computação Gráfica 2D e 3D: Analise Graphics2D/Rasterizer2D.cs e Graphics3D/SoftwareRenderer3D.cs para entender a rasterização.\n" +
                        "3. Modelagem Hierárquica e Ray Tracing: Experimente a articulação de mecanismos em Graphics3D/HierarchicalModeling.cs e o traçado de raios em Graphics3D/Raytracer3D.cs.",
                    CodeSnippet =
@"// Roteiro de estudos interativo:
// Navegue pelas abas superiores do programa e compare os resultados
// visuais com as fórmulas matemáticas da documentação!",
                    ComplexityAndTips = "• Consulte as abas interativas para verificar os resultados de cada algoritmo em tempo real.",
                    WhereToTest = "Central de Estudos e Laboratório C#/WPF.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual dos tópicos a seguir faz parte do escopo fundamental de Computação Gráfica 2D e 3D?",
                        Options = new List<string>
                        {
                            "Rasterização de primitivas (Bresenham, Círculo de Ponto Médio), Curvas de Bézier e Pipeline 3D com matrizes MVP e Z-Buffer.",
                            "Compressão de vídeo MPEG e streaming RTP.",
                            "Compilação de shaders HLSL para WebGL."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! O módulo aborda a base matemática de conversão geométrica discreta 2D e o pipeline clássico 3D de visualização."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Roteiros de Aprendizagem do .NET no Microsoft Learn",
                            Url = "https://learn.microsoft.com/pt-br/training/dotnet/",
                            Description = "Cursos e trilhas de aprendizagem oficiais gratuitas da Microsoft."
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
                    TargetLessonNumber = 12,
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
                    WhereToTest = "Aba 'Laboratório C#/WPF' e Aba 'Estúdio de Projetos'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual API do .NET Compiler Platform (Roslyn) é utilizada para avaliar e executar trechos de código C# em tempo de execução sem gerar arquivos temporários no disco?",
                        Options = new List<string>
                        {
                            "Microsoft.CodeAnalysis.CSharp.Scripting (CSharpScript.EvaluateAsync / CreateDelegate).",
                            "System.Reflection.Emit clássico.",
                            "MSBuild.exe em processo filho externo."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! O pacote Roslyn Scripting compila e emite assemblies diretamente na memória em milissegundos para execução interativa."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão Geral dos Compiladores .NET (Roslyn APIs)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/roslyn-overview",
                            Description = "Guia oficial da plataforma de compiladores .NET e APIs de análise sintática e semântica."
                        }
                    }
                },
                #endregion

                #region 13. Revisão Avançada C#, XAML & Arquitetura WPF
                new StudyTopic
                {
                    Id = "wpf_xaml_deepdive",
                    Category = "13. Revisão C#, XAML & WPF",
                    Title = "Revisão Completa de C#, XAML, DependencyProperties e Renderização WPF",
                    Summary = "Tudo sobre como o WPF funciona: Árvore Visual e Lógica, XAML parsing, DependencyProperties, RoutedEvents e aceleração DirectX.",
                    TargetLessonNumber = 12,
                    MathFormulas =
                        "• Hierarquia de Classes Base do WPF:\n" +
                        "  Object -> DispatcherObject -> DependencyObject -> Visual -> UIElement -> FrameworkElement -> Control\n\n" +
                        "• Árvore Lógica vs. Árvore Visual:\n" +
                        "  - Logical Tree: Representa a estrutura de elementos declarada no XAML (ex: Button com Content).\n" +
                        "  - Visual Tree: Representa todos os nós visuais detalhados gerados por ControlTemplates (Border, ContentPresenter, TextBlock).\n\n" +
                        "• Mecanismo de DependencyProperty:\n" +
                        "  Valor Efetivo = Resolução de Precedência (Local > Style > Template > Herança > Valor Padrão)",
                    CodeExplanation =
                        "1. XAML (Extensible Application Markup Language) é um formato XML declarativo que o compilador do WPF traduz em código C# instanciando objetos e configurando propriedades.\n" +
                        "2. DependencyProperties permitem recursos avançados como Data Binding, Animações, Estilos automáticos e herança de propriedades entre controles pai e filho sem ocupar memória em campos de instância redundantes.\n" +
                        "3. RoutedEvents percorrem a árvore visual em três estratégias: Direct (apenas no elemento), Tunneling (do topo para o elemento, prefixo Preview) e Bubbling (do elemento subindo até a janela raiz).\n" +
                        "4. XamlReader.Parse(): Permite instanciar árvores de controles WPF dinamicamente a partir de strings XAML em tempo de execução sem recompilar o executável.",
                    CodeSnippet =
@"// Registro de uma DependencyProperty com callback de alteração:
public static readonly DependencyProperty CustomRadiusProperty =
    DependencyProperty.Register(
        name: ""CustomRadius"",
        propertyType: typeof(double),
        ownerType: typeof(MyCustomControl),
        typeMetadata: new PropertyMetadata(10.0, OnRadiusChanged));

private static void OnRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
{
    var ctrl = (MyCustomControl)d;
    ctrl.InvalidateVisual(); // Solicita redesenho ao subsistema DirectX
}",
                    ComplexityAndTips = "• Dica: Para criar interfaces fluidas, use Grid, Viewbox e Canvas de forma combinada e evite tamanhos fixos em pixels rígidos.",
                    WhereToTest = "Aba 'Laboratório C#/WPF' (aba XAML) e Aba 'Estúdio de Projetos'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual é a principal diferença entre a 'Árvore Lógica' (Logical Tree) e a 'Árvore Visual' (Visual Tree) no WPF?",
                        Options = new List<string>
                        {
                            "A Árvore Lógica contém apenas os elementos definidos na estrutura do XAML, enquanto a Árvore Visual expande todos os elementos gráficos internos (Borders, Brushes, ContentPresenters) criados pelos templates para renderização.",
                            "A Árvore Lógica roda na GPU e a Árvore Visual roda na CPU.",
                            "A Árvore Lógica só aceita classes C# e a Árvore Visual só aceita arquivos XML."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! A Árvore Lógica descreve a estrutura de dados declarada pelo desenvolvedor, enquanto a Árvore Visual contém cada nó de renderização gerado pelos ControlTemplates e estilos."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão geral de XAML (WPF .NET)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/fundamentals/xaml",
                            Description = "Guia completo de sintaxe XAML, elementos de objeto, propriedades de elemento e markup extensions."
                        },
                        new DocReference
                        {
                            Title = "Visão geral das propriedades de dependência",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/properties/dependency-properties-overview",
                            Description = "Como funcionam as Dependency Properties, herança de valores e data binding no WPF."
                        },
                        new DocReference
                        {
                            Title = "Árvores no WPF (Árvore Visual vs Lógica)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/fundamentals/trees-overview",
                            Description = "Compreensão detalhada de árvores lógicas e visuais no WPF."
                        }
                    }
                },
                #endregion

                #region 8. Módulos Avançados: Teoria e Atividades Práticas Aplicadas

                #region 2D: Teoria de Pipeline, Matrizes e Templates
                new StudyTopic
                {
                    Id = "cg2d_templates_and_pipeline",
                    Category = "5. Transformações e Álgebra Linear",
                    Title = "[CONTEÚDO DE ESTUDO & TEORIA] Pipeline Gráfico 2D, Matrizes Homogêneas 3x3 e Templates no WPF",
                    Summary = "Pipeline de renderização 2D, transformações afins em coordenadas homogêneas, rotação em torno de pivô arbitrário e reutilização de geometria com ControlTemplate.",
                    TargetLessonNumber = 13,
                    MathFormulas =
                        "• Matriz de Transformação Afim 2D em Coordenadas Homogêneas (3x3):\n" +
                        "  [ x' ]   [ s_x·cos(θ)  -s_y·sin(θ)  t_x ]   [ x ]\n" +
                        "  [ y' ] = [ s_x·sin(θ)   s_y·cos(θ)  t_y ] · [ y ]\n" +
                        "  [ 1  ]   [     0            0        1  ]   [ 1 ]\n\n" +
                        "• Rotação em Torno de Ponto Pivô (x0, y0):\n" +
                        "  M = T(x0, y0) · R(θ) · T(-x0, -y0)\n\n" +
                        "• Interpolação Temporal de Animação Contínua com AutoReverse:\n" +
                        "  x(t) = x_inicial + (x_final - x_inicial) · f_easing(t / T)",
                    CodeExplanation =
                        "1. Coordenadas homogêneas permitem representar rotação, escala e translação através de uma única multiplicação matricial 3x3 encadeada.\n" +
                        "2. No WPF, ControlTemplates declarados dentro de Canvas.Resources permitem instanciar geometrias complexas de forma modular em qualquer posição da tela com RenderTransform.\n" +
                        "3. A propriedade RenderTransformOrigin=\"0.5,0.5\" define o centro geométrico relativo para rotação sem necessidade de translações manuais de offset.\n" +
                        "4. DoubleAnimation aplicada ao Storyboard gerencia o loop temporal suave com suporte nativo a AutoReverse=\"True\" e RepeatBehavior=\"Forever\".",
                    CodeSnippet =
@"<!-- Definição de Template Reutilizável com Rotação Pivotada no WPF -->
<Canvas.Resources>
    <ControlTemplate x:Key=""PonteiroTemplate"">
        <Polygon Points=""0,0 -4,-18 0,-50 4,-18"" Fill=""#38BDF8""/>
    </ControlTemplate>
</Canvas.Resources>

<!-- Instanciação com Transformação Afim -->
<Control Template=""{StaticResource PonteiroTemplate}"">
    <Control.RenderTransform>
        <RotateTransform Angle=""45"" CenterX=""0"" CenterY=""0""/>
    </Control.RenderTransform>
</Control>",
                    ComplexityAndTips = "• Dica: Construa componentes primitivos com referência na origem (0,0) para que rotações e escalas funcionem de maneira previsível através de TransformGroups.",
                    WhereToTest = "Aba 'Laboratório Interativo' (Lição 13) e Aba 'Estúdio de Projetos' (Veículo Articulado 2D).",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual é a sequência matemática correta de transformações matriciais para rotacionar um polígono 2D em torno de um ponto arbitrário P(x0, y0)?",
                        Options = new List<string>
                        {
                            "Transladar o polígono pelo vetor (-x0, -y0) para a origem, aplicar a matriz de rotação R(θ) e transladar de volta por (+x0, +y0).",
                            "Aplicar a matriz de rotação R(θ) diretamente e somar as coordenadas (x0, y0) ao resultado.",
                            "Multiplicar a escala por (x0, y0) e depois aplicar a translação inversa."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Para rotacionar em torno de um ponto qualquer P, primeiro movemos o ponto para a origem T(-P), rotacionamos R(θ) e restauramos a posição original T(+P)."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão geral de transformações no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview",
                            Description = "Classes RotateTransform, TranslateTransform, ScaleTransform e MatrixTransform no WPF."
                        },
                        new DocReference
                        {
                            Title = "Visão geral da animação de gráficos no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview",
                            Description = "DoubleAnimation, Storyboards e propriedades de repetição temporal."
                        }
                    }
                },
                #endregion

                #region 2D: Atividade Prática de Sistema Articulado
                new StudyTopic
                {
                    Id = "cg2d_activity_articulated_system",
                    Category = "4. Rasterização e Geometria 2D",
                    Title = "[ATIVIDADE PRÁTICA APLICADA] Construção de Sistema Mecânico 2D com Eixos Múltiplos e Animação",
                    Summary = "Roteiro prático para projetar um veículo 2D com eixos múltiplos, templates modulares de ponteiros/raios e sincronização de animações bidirecionais.",
                    TargetLessonNumber = 13,
                    MathFormulas =
                        "• Distribuição Angular dos Raios da Roda (N raios):\n" +
                        "  θ_k = k · (360° / N), para k = 0, 1, ..., N-1\n\n" +
                        "• Sincronização Cinemática entre Translação Linear (v) e Velocidade Angular (ω):\n" +
                        "  v = ω · R_roda  ⇒  ω = v / R_roda  (rad/s)\n\n" +
                        "• Período de Rotação Completa:\n" +
                        "  T_rot = (2 · π · R_roda) / v",
                    CodeExplanation =
                        "1. Passo 1 (Template do Raio): Construa um ControlTemplate para o ponteiro/raio com vértice na origem (0,0).\n" +
                        "2. Passo 2 (Montagem da Roda): Crie um Canvas contendo o aro circular e instancie pelo menos 4 ponteiros com ângulos uniformes (0°, 90°, 180°, 270°).\n" +
                        "3. Passo 3 (Template da Roda): Envolva a roda em seu próprio ControlTemplate reutilizável.\n" +
                        "4. Passo 4 (Giro Animado): Adicione uma DoubleAnimation na propriedade RotateTransform.Angle do template da roda.\n" +
                        "5. Passo 5 (Chassi e Instâncias): No Canvas principal, desenhe a carroceria e instancie 4 rodas (1 dianteira e 3 no conjunto traseiro).\n" +
                        "6. Passo 6 (Translação com AutoReverse): Aplique uma TranslateTransform3D/2D no veículo completo com AutoReverse=\"True\" para inverter a direção e rotação.",
                    CodeSnippet =
@"<!-- Estrutura de Atividade Prática 2D -->
<Canvas x:Name=""CenarioCanvas"" Width=""800"" Height=""400"">
    <!-- Veículo Completo com Animação de Deslocamento -->
    <Canvas x:Name=""Veiculo"">
        <!-- Carroceria / Chassi -->
        <Path Data=""M 40,80 L 180,80 L 220,130 L 320,130 L 320,200 L 40,200 Z"" Fill=""#1E293B"" Stroke=""#38BDF8"" StrokeThickness=""2""/>
        <!-- 4 Instâncias da Roda Modular -->
        <Control Canvas.Left=""70"" Canvas.Top=""200"" Template=""{StaticResource RodaTemplate}""/>
        <Control Canvas.Left=""190"" Canvas.Top=""200"" Template=""{StaticResource RodaTemplate}""/>
        <Control Canvas.Left=""235"" Canvas.Top=""200"" Template=""{StaticResource RodaTemplate}""/>
        <Control Canvas.Left=""280"" Canvas.Top=""200"" Template=""{StaticResource RodaTemplate}""/>
    </Canvas>
</Canvas>",
                    ComplexityAndTips = "• Dica de Implementação: Usar a propriedade AutoReverse=\"True\" no Storyboard inverte suavemente o trajeto e o sentido do giro sem descontinuidades na animação.",
                    WhereToTest = "Aba 'Estúdio de Projetos' -> Template 'Veículo Articulado 2D com Eixo Triplo'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Ao aplicar a propriedade AutoReverse=\"True\" na animação de rotação e translação de um veículo, qual comportamento é esperado?",
                        Options = new List<string>
                        {
                            "O veículo atinge o final do percurso e retorna ao ponto inicial pelo caminho inverso, invertendo automaticamente o sentido de giro das rodas.",
                            "A animação é pausada permanentemente no último quadro.",
                            "As rodas continuam girando para frente enquanto o chassi anda para trás."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! A propriedade AutoReverse faz com que a linha do tempo da animação seja reproduzida em sentido inverso ao término de cada ciclo, mantendo a coerência física."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Timeline.AutoReverse Property",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.animation.timeline.autoreverse",
                            Description = "Documentação oficial da propriedade AutoReverse na engine de animações do WPF."
                        }
                    }
                },
                #endregion

                #region 3D: Teoria de Malhas, Câmeras e Iluminação
                new StudyTopic
                {
                    Id = "cg3d_mesh_cameras_lambert",
                    Category = Category3D,
                    Title = "[CONTEÚDO DE ESTUDO & TEORIA] Malhas Triangulares 3D, Câmeras Perspectivas e Lei de Lambert",
                    Summary = "Estrutura de malhas MeshGeometry3D, ordenação anti-horária de vértices, modelo de iluminação difusa de Lambert, sombreamento de Gouraud e mapeamento UV com TileMode.",
                    TargetLessonNumber = 14,
                    MathFormulas =
                        "• Vetor Normal da Face Triangular (V0, V1, V2 com ordem CCW):\n" +
                        "  Vetor U = V1 - V0,   Vetor V = V2 - V0\n" +
                        "  Normal N = (U × V) / ||U × V||\n\n" +
                        "• Reflexão Difusa da Lei de Lambert:\n" +
                        "  I_difusa = I_luz · k_d · max(0, N · L)\n" +
                        "  onde N é o vetor normal unitário e L é a direção da luz incidente.\n\n" +
                        "• Sombreamento de Gouraud (Normal no Vértice v_k compartilhado por M faces):\n" +
                        "  N_vertice = (∑ N_face_i) / ||∑ N_face_i||",
                    CodeExplanation =
                        "1. Uma malha triangular no WPF é definida por MeshGeometry3D através de Positions (vértices 3D) e TriangleIndices (trios de índices).\n" +
                        "2. A ordem dos vértices no sentido anti-horário (CCW) determina a face frontal. Triângulos visualizados por trás são descartados por Back-Face Culling.\n" +
                        "3. A PerspectiveCamera define a posição no espaço (Position), a direção de mira (LookDirection), a orientação vertical (UpDirection) e o campo de visão (FieldOfView).\n" +
                        "4. DirectionalLight simula fontes infinitamente distantes (como o Sol), onde todos os raios de luz incidem paralelamente com a mesma intensidade.\n" +
                        "5. ImageBrush com ViewportUnits=\"Absolute\" e TileMode=\"Tile\" permite repetir texturas de pedra/terreno em pisos e paredes sem distorção.",
                    CodeSnippet =
@"<!-- Especificação de Malha 3D e Iluminação Difusa no WPF -->
<GeometryModel3D>
    <GeometryModel3D.Geometry>
        <MeshGeometry3D Positions=""-1,0,-1  1,0,-1  1,0,1  -1,0,1""
                        TriangleIndices=""0,1,2  0,2,3""
                        TextureCoordinates=""0,0  4,0  4,4  0,4""/>
    </GeometryModel3D.Geometry>
    <GeometryModel3D.Material>
        <DiffuseMaterial>
            <DiffuseMaterial.Brush>
                <ImageBrush ImageSource=""Assets/granite.png""
                            TileMode=""Tile"" Viewport=""0,0,0.25,0.25"" ViewportUnits=""Relative""/>
            </DiffuseMaterial.Brush>
        </DiffuseMaterial>
    </GeometryModel3D.Material>
</GeometryModel3D>",
                    ComplexityAndTips = "• Dica: Para obter sombreamento suave em superfícies curvas, compartilhe os mesmos índices de vértices. Para faces facetadas (flat shading), duplique os vértices de cada triângulo.",
                    WhereToTest = "Aba 'Computação Gráfica 3D' e 'Laboratório Interativo' (Lição 14).",
                    Quiz = new StudyQuiz
                    {
                        Question = "Por que o modelo de iluminação de Lambert utiliza o produto escalar entre o vetor normal da superfície (N) e o vetor da luz (L)?",
                        Options = new List<string>
                        {
                            "Porque o produto escalar N · L calcula o cosseno do ângulo de incidência, determinando quanta energia luminosa por unidade de área é projetada sobre a superfície.",
                            "Porque o produto escalar inverte a cor do pixel quando a luz atinge a superfície perpendicularmente.",
                            "Porque as placas de vídeo não conseguem calcular sombras sem antes normalizar as coordenadas da câmera."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Pela Lei de Lambert, a intensidade refletida por uma superfície difusa é proporcional ao cosseno do ângulo entre a normal da superfície e a direção da luz incidente."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão geral de gráficos 3D no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/3-d-graphics-overview",
                            Description = "Guia completo de Viewport3D, MeshGeometry3D, PerspectiveCamera e Materials no WPF."
                        },
                        new DocReference
                        {
                            Title = "Classe DirectionalLight (System.Windows.Media.Media3D)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.directionallight",
                            Description = "Documentação oficial da fonte de luz direcional com vetor Direction e Color."
                        }
                    }
                },
                #endregion

                #region 3D: Atividade Prática de Cena Arquitetônica
                new StudyTopic
                {
                    Id = "cg3d_activity_architectural_scene",
                    Category = Category3D,
                    Title = "[ATIVIDADE PRÁTICA APLICADA] Cena Arquitetônica 3D com Sistema de Iluminação Solar Duplo e Câmera Orbital",
                    Summary = "Guia de implementação de uma estrutura tridimensional com múltiplos cômodos, piso texturizado com repetição, câmera orbital vertical de 180° e duas luzes direcionais a 30°.",
                    TargetLessonNumber = 14,
                    MathFormulas =
                        "• Trajetória Orbital Vertical da Câmera (Arco de 180° em torno de Y):\n" +
                        "  θ(t) = π · (t / T_arco), para t ∈ [0, T_arco]\n" +
                        "  X_cam = R · cos(θ(t)),   Y_cam = R · sin(θ(t)) + Y_offset,   Z_cam = Z_fixo\n\n" +
                        "• Vetores de Iluminação Direcional a 30° do Solo em Rotação Horizontal Oposta (360°):\n" +
                        "  Luz 1: D1 = (cos(ω·t) · cos(30°), -sin(30°), sin(ω·t) · cos(30°))\n" +
                        "  Luz 2: D2 = (-cos(ω·t) · cos(30°), -sin(30°), -sin(ω·t) · cos(30°))",
                    CodeExplanation =
                        "1. Passo 1 (Geometria da Estrutura): Crie pelo menos 3 blocos retangulares para os cômodos e um prisma triangular para o telhado usando GeometryModel3D com cores distintas.\n" +
                        "2. Passo 2 (Piso com Textura em Mosaico): Construa um plano de chão amplo e aplique um ImageBrush com TileMode=\"Tile\" e Viewport dimensionado para repetir a textura de granito.\n" +
                        "3. Passo 3 (Câmera Orbital Vertical): Configure uma PerspectiveCamera animada por Point3DAnimation ou RotateTransform3D para realizar um arco de 180° por cima da cena em 5 segundos com AutoReverse=\"True\".\n" +
                        "4. Passo 4 (Sistema Solar Duplo): Instancie duas DirectionalLights com inclinação fixa de 30° em relação ao plano do chão, aplicando Vector3DAnimation para girá-las 360° em sentidos opostos durante 10 segundos.",
                    CodeSnippet =
@"<!-- Cena 3D Arquitetônica com Câmera e Iluminação Dupla -->
<Viewport3D>
    <Viewport3D.Camera>
        <PerspectiveCamera Position=""0,8,14"" LookDirection=""0,-0.5,-1"" UpDirection=""0,1,0"" FieldOfView=""60""/>
    </Viewport3D.Camera>
    <ModelVisual3D>
        <ModelVisual3D.Content>
            <Model3DGroup>
                <!-- Luz Direcional 1 (Sol Leste) -->
                <DirectionalLight Color=""#FFF0D0"" Direction=""0.866, -0.5, 0""/>
                <!-- Luz Direcional 2 (Sol Oeste Oposto) -->
                <DirectionalLight Color=""#E0F0FF"" Direction=""-0.866, -0.5, 0""/>
                <AmbientLight Color=""#303038""/>
                <!-- Modelos 3D da Casa e Chão -->
            </Model3DGroup>
        </ModelVisual3D.Content>
    </ModelVisual3D>
</Viewport3D>",
                    ComplexityAndTips = "• Dica: Para garantir que as faces não desapareçam durante a rotação da câmera, certifique-se de que todos os triângulos externos foram indexados no sentido anti-horário (CCW).",
                    WhereToTest = "Aba 'Estúdio de Projetos' -> Template 'Cena Arquitetônica 3D com Iluminação Solar Dupla'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Como é configurada uma iluminação direcional que forma 30° de inclinação em relação ao plano horizontal (chão XZ)?",
                        Options = new List<string>
                        {
                            "Definindo a componente vertical Y do vetor como -sin(30°) = -0.5 e o raio horizontal como cos(30°) ≈ 0.866.",
                            "Definindo a posição da luz em (0, 30, 0) com PointLight.",
                            "Alterando a propriedade FieldOfView da câmera para 30 graus."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! O vetor de direção da luz direcional D = (cos(30°), -sin(30°), 0) estabelece o ângulo de 30° descendente em relação ao plano horizontal XZ."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe TileBrush.TileMode Property",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.tilebrush.tilemode",
                            Description = "Como configurar repetição de texturas com None, Tile, FlipX, FlipY e FlipXY."
                        }
                    }
                },
                #endregion

                #region Hierarquia: Teoria de Modelagem Hierárquica e Grafos de Cena
                new StudyTopic
                {
                    Id = "cg3d_hierarchical_modeling",
                    Category = Category3D,
                    Title = "[CONTEÚDO DE ESTUDO & TEORIA] Modelagem Hierárquica, Design Top-Down e Grafos de Cena 3D",
                    Summary = "Design Top-Down e Construção Bottom-Up de modelos complexos articulados, separação entre componentes primitivos e agrupadores, transformações de instância e transformações de junta.",
                    TargetLessonNumber = 15,
                    MathFormulas =
                        "• Composição Hierárquica da Matriz de Transformação Global de um Nó Filho (Grafo de Cena):\n" +
                        "  M_global_filho = M_global_pai · M_instancia · M_junta(t)\n\n" +
                        "• Cadeia Cinemática de um Membro Articulado (ex: Tronco → Coxa → Canela → Pé):\n" +
                        "  M_pe = M_tronco · M_junta_quadril(t) · T_coxa · M_junta_joelho(t) · T_canela · M_junta_tornozelo(t)\n\n" +
                        "• Transformação de Instância vs Transformação de Junta:\n" +
                        "  - Instância: Posicionamento estático e orientação relativa ao componente pai.\n" +
                        "  - Junta: Rotação dinâmica parametrizada pelo tempo em torno do ponto de articulação.",
                    CodeExplanation =
                        "1. Top-Down Design: Estratégia de análise onde o objeto complexo é decomposto em subcomponentes hierárquicos identificando as juntas onde ocorrerá movimento.\n" +
                        "2. Bottom-Up Construction: Construção prática que inicia pelas partes mais simples (componentes primitivos como pé, canela, coxa) e as agrega em Model3DGroups com transformações de instância.\n" +
                        "3. Componente Primitivo: Geometria básica criada a partir de uma malha (MeshGeometry3D) com material próprio na origem.\n" +
                        "4. Componente Agrupador: Model3DGroup que contém subcomponentes primitivos e outros agrupadores filhos.\n" +
                        "5. Transformação de Junta: RotateTransform3D aplicada no ponto pivô da articulação com animações periódicas defasadas (senos/cossenos) para gerar locomoção realista.",
                    CodeSnippet =
@"<!-- Grafo de Cena Hierárquico no WPF com Animação de Junta -->
<Model3DGroup x:Name=""PernaCompleta"">
    <!-- Coxa (Nó Pai) -->
    <GeometryModel3D Geometry=""{StaticResource CoxaMesh}"" Material=""{StaticResource CoxaMat}""/>
    
    <!-- Canela e Pé (Nó Filho Articulado no Joelho) -->
    <Model3DGroup>
        <Model3DGroup.Transform>
            <Transform3DGroup>
                <!-- Transformação de Instância (Offset até o joelho) -->
                <TranslateTransform3D OffsetY=""-1.5""/>
                <!-- Transformação de Junta (Articulação do Joelho) -->
                <RotateTransform3D CenterY=""0"">
                    <RotateTransform3D.Rotation>
                        <AxisAngleRotation3D Axis=""1,0,0"" Angle=""25""/>
                    </RotateTransform3D.Rotation>
                </RotateTransform3D>
            </Transform3DGroup>
        </Model3DGroup.Transform>
        <!-- Geometria da Canela e Pé -->
        <GeometryModel3D Geometry=""{StaticResource CanelaMesh}"" Material=""{StaticResource CanelaMat}""/>
    </Model3DGroup>
</Model3DGroup>",
                    ComplexityAndTips = "• Vantagem: Alterar a posição ou rotação do nó raiz propaga-se automaticamente para todos os nós filhos sem recalcular vértices manualmente.",
                    WhereToTest = "Aba 'Laboratório Interativo' (Lição 15) e 'Estúdio de Projetos' (Modelo Hierárquico de Quadrúpede).",
                    Quiz = new StudyQuiz
                    {
                        Question = "Qual é a diferença fundamental entre uma 'Transformação de Instância' e uma 'Transformação de Junta' em modelagem hierárquica?",
                        Options = new List<string>
                        {
                            "A Transformação de Instância posiciona e orienta o subcomponente estaticamente em relação ao seu pai, enquanto a Transformação de Junta aplica a rotação dinâmica da articulação.",
                            "A Transformação de Instância é calculada pela CPU e a de Junta é calculada pelo monitor.",
                            "A Transformação de Instância altera a cor do material e a de Junta altera a textura."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! A transformação de instância define onde o subcomponente fica montado na estrutura, enquanto a transformação de junta permite a rotação e movimento articulado da junta mecânica."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Model3DGroup (System.Windows.Media.Media3D)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.model3dgroup",
                            Description = "Agrupamento de nós 3D em grafo de cena com transformações hierárquicas no WPF."
                        }
                    }
                },
                #endregion

                #region Hierarquia: Atividade Prática de Quadrúpede Articulado
                new StudyTopic
                {
                    Id = "cg3d_activity_quadruped_caravan",
                    Category = Category3D,
                    Title = "[ATIVIDADE PRÁTICA APLICADA] Modelagem Hierárquica de Quadrúpede Articulado e Animação de Marcha",
                    Summary = "Roteiro de construção de um modelo 3D articulado completo com 14 componentes primitivos coloridos, 9 juntas animadas para ciclo de marcha e instanciação de múltiplos modelos em fila.",
                    TargetLessonNumber = 15,
                    MathFormulas =
                        "• Equação Harmônica da Marcha Quadrúpede (Defasagem Angular das 4 Patas):\n" +
                        "  θ_dianteira_esq(t) = A_passo · sin(ω · t)\n" +
                        "  θ_dianteira_dir(t) = A_passo · sin(ω · t + π)\n" +
                        "  θ_traseira_esq(t)  = A_passo · sin(ω · t + π/2)\n" +
                        "  θ_traseira_dir(t)  = A_passo · sin(ω · t + 3π/2)\n\n" +
                        "• Translação Linear da Caravana ao Longo do Plano do Terreno:\n" +
                        "  X_k(t) = (X_inicial_k + v_marcha · t) mod L_cenario",
                    CodeExplanation =
                        "1. Passo 1 (14 Primitivas): Construa 14 malhas geométricas retangulares (tronco, pescoço, cabeça, corcova, 4 coxas, 4 canelas, 4 pés/patas) com cores distintas para rápida identificação visual.\n" +
                        "2. Passo 2 (Agrupador Raiz): Crie o Model3DGroup raiz do quadrúpede e estruture a árvore hierárquica conectando pescoço à cabeça e tronco às 4 pernas.\n" +
                        "3. Passo 3 (9 Juntas Animadas): Adicione transformações de rotação nas 9 articulações (pescoço/cabeça e quadril/joelho das 4 patas) com rotações senoidais defasadas.\n" +
                        "4. Passo 4 (Cenário e Múltiplas Instâncias): Instancie 4 quadrúpedes em fila indiana sobre um plano de terreno texturizado com iluminação direcional e ambiente.",
                    CodeSnippet =
@"<!-- Instanciação de Múltiplos Modelos Hierárquicos na Cena -->
<Viewport3D>
    <ModelVisual3D>
        <ModelVisual3D.Content>
            <Model3DGroup>
                <!-- Terreno Texturizado -->
                <GeometryModel3D Geometry=""{StaticResource TerrenoMesh}"" Material=""{StaticResource TerrenoMat}""/>
                
                <!-- Quadrúpede 1 -->
                <Model3DGroup Transform=""{StaticResource PosicaoQuadrupede1}"">
                    <StaticResource ResourceKey=""ModeloQuadrupedeCompleto""/>
                </Model3DGroup>
                
                <!-- Quadrúpede 2 (Seguidor na Fila) -->
                <Model3DGroup Transform=""{StaticResource PosicaoQuadrupede2}"">
                    <StaticResource ResourceKey=""ModeloQuadrupedeCompleto""/>
                </Model3DGroup>
            </Model3DGroup>
        </ModelVisual3D.Content>
    </ModelVisual3D>
</Viewport3D>",
                    ComplexityAndTips = "• Dica de Desempenho: Reutilize as definições de MeshGeometry3D para todas as instâncias da caravana, alterando apenas os nós raiz de transformação TranslateTransform3D.",
                    WhereToTest = "Aba 'Estúdio de Projetos' -> Template 'Modelo Hierárquico 3D de Quadrúpede'.",
                    Quiz = new StudyQuiz
                    {
                        Question = "Para gerar uma marcha quadrúpede natural em uma animação hierárquica, como devem ser configuradas as oscilações das quatro patas?",
                        Options = new List<string>
                        {
                            "Com defasagens angulares periódicas (ex.: 0, π, π/2, 3π/2) entre patas dianteiras e traseiras opostas, simulando a transferência alternada de peso.",
                            "Movendo todas as quatro patas simultaneamente no mesmo ângulo e fase.",
                            "Mantendo as patas estáticas e rotacionando apenas a câmera."
                        },
                        CorrectOptionIndex = 0,
                        Explanation = "Correto! Na locomoção quadrúpede (trote ou caminhada), as patas movem-se com defasagem de fase harmônica para garantir estabilidade mecânica contínua."
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Hierarquia e Transformações 3D no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/3-d-graphics-overview#3-d-transformations",
                            Description = "Guia de composição matricial com Transform3DGroup e hierarquias de Model3DGroup."
                        }
                    }
                }
                #endregion

                #endregion
            };
        }
    }
}
