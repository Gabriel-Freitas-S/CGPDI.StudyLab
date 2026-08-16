using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    public class QuizOption
    {
        public string Text { get; set; } = "";
        public bool IsCorrect { get; set; }
        public string Explanation { get; set; } = "";
    }

    public enum LessonType
    {
        BgraMemoryLayout,
        CSharpPropertiesAndNotify,
        PointerStrideOffset,
        WpfXamlAndDependencyProps,
        WriteableBitmapLifecycle,
        ConvolutionStepByStep,
        OtsuThresholdSearch,
        BresenhamStepByStep,
        MatrixTransform2D,
        PipelineMVP3D,
        HierarchicalSceneGraph,
        RayTracingIntersection
    }

    public class InteractiveLesson
    {
        public int Number { get; set; }
        public string Id { get; set; } = "";
        public string Title { get; set; } = "";
        public string Module { get; set; } = "";
        public LessonType Type { get; set; }
        public string Summary { get; set; } = "";
        public string Theory { get; set; } = "";
        public string CodeSnippet { get; set; } = "";
        public string CodeExplanation { get; set; } = "";
        public string ChallengeGoal { get; set; } = "";
        public string StarterTemplate { get; set; } = "";
        public string BlankTemplate { get; set; } = "";
        public string SolutionCode { get; set; } = "";
        public string? XamlSnippet { get; set; }
        public string? XamlExplanation { get; set; }
        public bool HasXamlContent => !string.IsNullOrWhiteSpace(XamlSnippet);
        public string ControlsDescription { get; set; } = "";
        public string QuizQuestion { get; set; } = "";
        public List<QuizOption> QuizOptions { get; set; } = new List<QuizOption>();
        public List<DocReference> MicrosoftReferences { get; set; } = new List<DocReference>();
    }

    /// <summary>
    /// Gerenciador pedagógico do Modo Interativo de Estudo (C# &amp; WPF Passo a Passo).
    /// </summary>
    public static class InteractiveLabManager
    {
        public static List<InteractiveLesson> GetLessons()
        {
            return new List<InteractiveLesson>
            {
                #region Lição 1
                new InteractiveLesson
                {
                    Number = 1,
                    Id = "lesson_csharp_bgra",
                    Title = "1. C# Tipos Primitivos, Structs vs Classes & Formato BGRA32",
                    Module = "Módulo 1: Revisão de C# Essencial para WPF",
                    Type = LessonType.BgraMemoryLayout,
                    Summary = "Como a memória RAM aloca cores em 32 bits (4 bytes por pixel) na ordem nativa das GPUs Windows (BGRA) e a diferença entre Stack e Heap.",
                    Theory =
                        "• Tipos por Valor (Structs na Stack) vs Tipos por Referência (Classes na Heap):\n" +
                        "  - Tipos por Valor (byte, int, float, Color): Alocados diretamente na Stack da CPU, possuem custo zero de Garbage Collection.\n" +
                        "  - Tipos por Referência (BitmapSource, UIElement): Alocados na Heap gerenciada, monitorados pelo GC.\n\n" +
                        "• O Formato BGRA32 no WPF:\n" +
                        "  Cada pixel ocupa exatamente 4 bytes consecutivos na memória RAM:\n" +
                        "  - Byte 0: Blue (Azul: 0-255)\n" +
                        "  - Byte 1: Green (Verde: 0-255)\n" +
                        "  - Byte 2: Red (Vermelho: 0-255)\n" +
                        "  - Byte 3: Alpha (Opacidade: 0-255)\n\n" +
                        "• Bit Shifting em C#:\n" +
                        "  (alpha << 24) | (red << 16) | (green << 8) | blue",
                    CodeSnippet =
@"// Alocação e acesso a bytes de um pixel:
byte blue = 255;
byte green = 128;
byte red = 0;
byte alpha = 255;

// Compactação em um único inteiro de 32 bits (uint):
uint pixelColor = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'byte blue = 255;' -> Declara uma variável primitiva de 8 bits (struct na Stack) ocupando 1 byte (intervalo 0 a 255).
2. '(alpha << 24)' -> Desloca os 8 bits do Alpha 24 posições para a esquerda [31-24].
3. '| (red << 16)' -> Bitwise OR posiciona o Vermelho em [23-16].
4. '| (green << 8)' -> Posiciona o Verde na faixa [15-8].
5. '| blue' -> Posiciona o Azul nos bits menos significativos [7-0].
6. '(uint)(...)' -> Converte para inteiro de 32 bits sem custo de CPU (1 ciclo de registrador).",
                    ChallengeGoal = "Implemente a função PackBgra empacotando os 4 canais byte em um único valor uint de 32 bits na ordem BGRA.",
                    BlankTemplate =
@"public static uint PackBgra(byte b, byte g, byte r, byte a)
{
    // Escreva sua lógica aqui do zero:
    return 0;
}",
                    StarterTemplate =
@"public static uint PackBgra(byte b, byte g, byte r, byte a)
{
    // TODO 1: Posicione o canal Azul (b) nos bits 0-7
    // TODO 2: Desloque o canal Verde (g) para os bits 8-15
    // TODO 3: Desloque o canal Vermelho (r) para os bits 16-23
    // TODO 4: Desloque o canal Alpha (a) para os bits 24-31
    return (uint)(b | (g << 8) | (r << 16) | (a << 24));
}",
                    SolutionCode =
@"public static uint PackBgra(byte b, byte g, byte r, byte a)
{
    return (uint)(b | ((uint)g << 8) | ((uint)r << 16) | ((uint)a << 24));
}",
                    ControlsDescription = "Ajuste os controles deslizantes dos canais Azul, Verde, Vermelho e Alpha para ver a memória em tempo real.",
                    QuizQuestion = "Em uma imagem com resolução 512x512 no formato BGRA32, quantos bytes são ocupados na memória RAM?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "262.144 bytes (~256 KB)", IsCorrect = false, Explanation = "Incorreto. 512x512 = 262.144 pixels. Como cada pixel tem 4 bytes, o total é 4x maior." },
                        new QuizOption { Text = "1.048.576 bytes (1.00 MB)", IsCorrect = true, Explanation = "Correto! 512 * 512 * 4 bytes = 1.048.576 bytes = exatamente 1 MB de memória contígua." },
                        new QuizOption { Text = "4.194.304 bytes (4.00 MB)", IsCorrect = false, Explanation = "Incorreto. Isso corresponderia a 16 bytes por pixel ou resolução 1024x1024." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Tipos por valor vs Tipos por referência no C#",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/builtin-types/value-types",
                            Description = "Diferenças entre Stack, Heap, structs primitivas e classes."
                        },
                        new DocReference
                        {
                            Title = "PixelFormats.Bgra32 Property (WPF)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.pixelformats.bgra32",
                            Description = "Especificação oficial do formato padrão de 32 bits por pixel acelerado por GPU no Windows."
                        }
                    }
                },
                #endregion

                #region Lição 2
                new InteractiveLesson
                {
                    Number = 2,
                    Id = "lesson_csharp_notify",
                    Title = "2. C# Propriedades, Delegates, Eventos & INotifyPropertyChanged",
                    Module = "Módulo 1: Revisão de C# Essencial para WPF",
                    Type = LessonType.CSharpPropertiesAndNotify,
                    Summary = "Como o mecanismo de Data Binding do WPF utiliza a interface INotifyPropertyChanged para atualizar a interface gráfica sem acoplamento direto.",
                    Theory =
                        "• O que é INotifyPropertyChanged?\n" +
                        "  É a interface central do padrão MVVM no WPF. Sempre que o valor de um campo privado muda, o evento PropertyChanged é disparado para notificar a View em XAML.\n\n" +
                        "• Propriedades C# Completas vs Auto-Properties:\n" +
                        "  Para Data Binding com notificação, usamos propriedades completas com backing field privado:\n" +
                        "  private int _brilho;\n" +
                        "  public int Brilho { get => _brilho; set { if (_brilho != value) { _brilho = value; OnPropertyChanged(); } } }\n\n" +
                        "• Delegates e Eventos em C#:\n" +
                        "  Um 'event' é um wrapper seguro sobre um delegate multicast que impede que classes externas limpem os ouvintes inscritos.",
                    CodeSnippet =
@"// Implementação clássica de INotifyPropertyChanged no WPF:
public class ImageModel : INotifyPropertyChanged
{
    private int _threshold = 128;
    public event PropertyChangedEventHandler? PropertyChanged;

    public int Threshold
    {
        get => _threshold;
        set
        {
            if (_threshold != value)
            {
                _threshold = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Threshold)));
            }
        }
    }
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'public class ImageModel : INotifyPropertyChanged' -> Declara que o modelo notifica observers sobre alterações.
2. 'private int _threshold = 128;' -> Backing field na memória que armazena o estado real.
3. 'public event PropertyChangedEventHandler? PropertyChanged;' -> Evento delegado multicast do WPF.
4. 'if (_threshold != value)' -> Checagem de igualdade para evitar notificações redundantes e loops infinitos de binding.
5. '_threshold = value;' -> Atualiza o campo privado.
6. 'PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Threshold)));' -> Disparo thread-safe com operador de propagação nula (?.) e nameof para tipagem estática segura.",
                    ChallengeGoal = "Implemente a função SetProperty que atualiza o backing field e dispara a notificação caso o valor tenha mudado.",
                    BlankTemplate =
@"public static bool SetProperty(ref int field, int value, Action<string> notifyAction, string propName = ""Brilho"")
{
    // Escreva a lógica aqui do zero:
    return false;
}",
                    StarterTemplate =
@"public static bool SetProperty(ref int field, int value, Action<string> notifyAction, string propName = ""Brilho"")
{
    // TODO 1: Se o valor for idêntico ao campo atual, retorne false
    if (field == value) return false;
    
    // TODO 2: Atribua o novo valor ao campo
    field = value;
    
    // TODO 3: Dispare a notificação com o nome da propriedade
    notifyAction(propName);
    return true;
}",
                    SolutionCode =
@"public static bool SetProperty(ref int field, int value, Action<string> notifyAction, string propName = ""Threshold"")
{
    if (field == value) return false;
    field = value;
    notifyAction(propName);
    return true;
}",
                    ControlsDescription = "Mova o slider para simular a alteração da propriedade e observe o evento PropertyChanged disparando a renderização visual.",
                    QuizQuestion = "Por que o operador nameof(NomeDaPropriedade) é preferível em relação a strings literais 'NomeDaPropriedade' no INotifyPropertyChanged?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Porque nameof gera validação em tempo de compilação e suporta refatoração automática de nomes", IsCorrect = true, Explanation = "Correto! Se o nome da propriedade for renomeado, o compilador atualiza o nameof automaticamente sem erros em tempo de execução." },
                        new QuizOption { Text = "Porque nameof executa 10x mais rápido que strings", IsCorrect = false, Explanation = "Incorreto. nameof é resolvido em tempo de compilação e vira string literal no IL." },
                        new QuizOption { Text = "Porque strings literais não são aceitas pelo WPF", IsCorrect = false, Explanation = "Incorreto. O WPF aceita strings literais, mas elas são propensas a erros de digitação." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Interface INotifyPropertyChanged (System.ComponentModel)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.componentmodel.inotifypropertychanged",
                            Description = "Como notificar clientes e controles WPF de que um valor de propriedade foi alterado."
                        },
                        new DocReference
                        {
                            Title = "Visão Geral do Data Binding no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/data-binding-overview",
                            Description = "Conceitos fundamentais de Source, Target, Binding Modes (OneWay, TwoWay) e DataContext."
                        }
                    }
                },
                #endregion

                #region Lição 3
                new InteractiveLesson
                {
                    Number = 3,
                    Id = "lesson_csharp_pointers",
                    Title = "3. C# Código Não Gerenciado (unsafe byte*), fixed & Stride",
                    Module = "Módulo 1: Revisão de C# Essencial para WPF",
                    Type = LessonType.PointerStrideOffset,
                    Summary = "Cálculo matemático do endereço de memória de qualquer coordenada (X, Y) em uma grade contínua com ponteiros brutos de CPU.",
                    Theory =
                        "• O que é Stride?\n" +
                        "  Stride é a largura total de uma linha da imagem expressa em bytes. Para largura W e 4 bytes por pixel:\n" +
                        "  Stride = W * 4\n\n" +
                        "• Fórmula Fundamental de Endereçamento de Memória:\n" +
                        "  Endereço(x, y) = BaseBuffer + (y * Stride) + (x * 4)\n\n" +
                        "• Por que usar unsafe byte* em vez de GetPixel()?\n" +
                        "  O ponteiro acessa a memória RAM com apenas 1 ciclo de instrução da CPU, resultando em desempenho 100x superior sem overhead de chamadas de função.",
                    CodeSnippet =
@"// Acesso de ultra alta performance com ponteiro bruto:
unsafe
{
    byte* basePtr = (byte*)writeableBitmap.BackBuffer.ToPointer();
    int offset = (y * stride) + (x * 4);
    byte* pixel = basePtr + offset;
    
    pixel[0] = 255; // Blue
    pixel[1] = 0;   // Green
    pixel[2] = 0;   // Red
    pixel[3] = 255; // Alpha
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'unsafe { ... }' -> Bloco de código C# que permite manipulação direta de ponteiros de memória nativa sem a sobrecarga do Garbage Collector.
2. 'byte* basePtr = (byte*)writeableBitmap.BackBuffer.ToPointer();' -> Converte o ponteiro de sistema IntPtr para um ponteiro de byte navegável.
3. 'int offset = (y * stride) + (x * 4);' -> Transforma coordenadas 2D (linha Y, coluna X) em um deslocamento linear 1D em bytes.
4. 'byte* pixel = basePtr + offset;' -> Aritmética de ponteiro avançando para o endereço exato do pixel selecionado.
5. 'pixel[0] a pixel[3]' -> Gravação instantânea de 4 bytes na ordem nativa das GPUs Windows (BGRA) sem checagem de limites de array.",
                    ChallengeGoal = "Implemente CalculatePixelOffset calculando o deslocamento linear em bytes de um pixel (x, y) usando a fórmula com Stride.",
                    BlankTemplate =
@"public static int CalculatePixelOffset(int x, int y, int stride)
{
    // Escreva sua lógica aqui do zero:
    return 0;
}",
                    StarterTemplate =
@"public static int CalculatePixelOffset(int x, int y, int stride)
{
    // TODO: Cada linha Y tem 'stride' bytes; cada pixel X tem 4 bytes
    return (y * stride) + (x * 4);
}",
                    SolutionCode =
@"public static int CalculatePixelOffset(int x, int y, int stride)
{
    return (y * stride) + (x * 4);
}",
                    ControlsDescription = "Mova as coordenadas X e Y para inspecionar o cálculo do offset e a posição destacada na memória linear 1D.",
                    QuizQuestion = "Para uma imagem de largura 8 pixels e Stride de 32 bytes, qual é o deslocamento (offset) do pixel na coluna X=3 e linha Y=2?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Offset = 76 bytes", IsCorrect = true, Explanation = "Correto! Offset = (2 * 32) + (3 * 4) = 64 + 12 = 76 bytes a partir do byte inicial." },
                        new QuizOption { Text = "Offset = 20 bytes", IsCorrect = false, Explanation = "Incorreto. Não se esqueça de multiplicar Y pela largura da linha (Stride = 32)." },
                        new QuizOption { Text = "Offset = 96 bytes", IsCorrect = false, Explanation = "Incorreto. 96 bytes seria o início da linha Y=3." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Código não seguro e ponteiros no C# (unsafe)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/unsafe-code",
                            Description = "Instrução fixed, aritmética de ponteiros e acesso direto a blocos de memória."
                        },
                        new DocReference
                        {
                            Title = "WriteableBitmap.BackBuffer Property",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.imaging.writeablebitmap.backbuffer",
                            Description = "Ponteiro IntPtr nativo para o buffer traseiro de renderização do WPF."
                        }
                    }
                },
                #endregion

                #region Lição 4
                new InteractiveLesson
                {
                    Number = 4,
                    Id = "lesson_wpf_xaml_dp",
                    Title = "4. WPF XAML, Dependency Properties, Layout & Árvore Visual",
                    Module = "Módulo 2: Arquitetura & Renderização no WPF",
                    Type = LessonType.WpfXamlAndDependencyProps,
                    Summary = "Como o WPF compila XAML em BAML, gerencia DependencyProperties com herança de valores e executa as passadas de layout Measure e Arrange.",
                    Theory =
                        "• O que é uma DependencyProperty (DP)?\n" +
                        "  É uma propriedade registrada no subsistema de propriedades do WPF que suporta vinculação de dados (Data Binding), animações, estilos e herança de valores na árvore visual com baixo consumo de memória.\n\n" +
                        "• As Duas Fases do Sistema de Layout do WPF:\n" +
                        "  1. Measure Pass (MeasureOverride): O elemento pai pergunta a cada filho quanto espaço ele deseja ocupar (DesiredSize).\n" +
                        "  2. Arrange Pass (ArrangeOverride): O elemento pai posiciona cada filho no retângulo final alocado (RenderSize).\n\n" +
                        "• Árvore Lógica vs Árvore Visual:\n" +
                        "  - Árvore Lógica: Representa a estrutura de controles declarada no XAML.\n" +
                        "  - Árvore Visual: Contém todos os nós visuais de baixo nível (Borders, Visuals, Glyphs) enviados ao motor de renderização milcore.",
                    CodeSnippet =
@"// Registro e uso de DependencyProperty no WPF:
public class ZoomableCanvas : Canvas
{
    public static readonly DependencyProperty ZoomLevelProperty =
        DependencyProperty.Register(
            nameof(ZoomLevel),
            typeof(double),
            typeof(ZoomableCanvas),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));

    public double ZoomLevel
    {
        get => (double)GetValue(ZoomLevelProperty);
        set => SetValue(ZoomLevelProperty, value);
    }
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'public static readonly DependencyProperty ZoomLevelProperty' -> Declara o identificador estático da DP.
2. 'DependencyProperty.Register(...)' -> Registra a propriedade no registro central de tipos do WPF.
3. 'FrameworkPropertyMetadataOptions.AffectsRender' -> Sinaliza que alterações nesta propriedade devem invalidar o visual e forçar um redesenho pelo DirectX.
4. 'GetValue(ZoomLevelProperty)' -> Busca o valor na tabela esparsa de propriedades do DependencyObject.
5. 'SetValue(ZoomLevelProperty, value)' -> Grava o novo valor e notifica o motor de animações/layout.",
                    ChallengeGoal = "Implemente MeasureDesiredSize calculando o tamanho final desejado respeitando os limites mínimo, máximo e disponível.",
                    BlankTemplate =
@"public static double MeasureDesiredSize(double available, double min, double max, double contentDesired)
{
    // Calcule a dimensão respeitando as restrições de layout:
    return contentDesired;
}",
                    StarterTemplate =
@"public static double MeasureDesiredSize(double available, double min, double max, double contentDesired)
{
    // TODO 1: Limite o conteúdo ao espaço disponível
    double bounded = Math.Min(contentDesired, available);
    
    // TODO 2: Restrinja entre min e max
    return Math.Clamp(bounded, min, max);
}",
                    SolutionCode =
@"public static double MeasureDesiredSize(double available, double min, double max, double contentDesired)
{
    double bounded = Math.Clamp(contentDesired, min, max);
    return Math.Min(available, bounded);
}",
                    XamlSnippet =
@"<Grid Margin=""12"">
    <!-- Exemplo Pedagógico: Árvore Visual e Controles Vetoriais WPF -->
    <Border Background=""#1E293B"" CornerRadius=""10"" BorderBrush=""#3B82F6"" BorderThickness=""2"" Padding=""16"">
        <StackPanel VerticalAlignment=""Center"" HorizontalAlignment=""Center"">
            <TextBlock Text=""Árvore Visual e Dependency Properties no WPF"" Foreground=""#38BDF8"" FontSize=""15"" FontWeight=""Bold"" HorizontalAlignment=""Center""/>
            <TextBlock Text=""Elementos declarativos compilados para BAML e renderizados via DirectX."" Foreground=""#94A3B8"" FontSize=""11.5"" Margin=""0,4,0,12"" HorizontalAlignment=""Center""/>
            
            <!-- Canvas com Formas Vetoriais Nativas -->
            <Canvas Width=""220"" Height=""90"" Background=""#0F172A"" Margin=""0,0,0,12"">
                <Rectangle Canvas.Left=""15"" Canvas.Top=""15"" Width=""60"" Height=""60"" Fill=""#3B82F6"" RadiusX=""6"" RadiusY=""6""/>
                <Ellipse Canvas.Left=""95"" Canvas.Top=""15"" Width=""60"" Height=""60"" Fill=""#10B981""/>
                <Line X1=""15"" Y1=""45"" X2=""190"" Y2=""45"" Stroke=""#F59E0B"" StrokeThickness=""2""/>
            </Canvas>
            
            <Button Content=""Botão com Estilo e Disparo de Evento"" Background=""#2563EB"" Foreground=""#FFFFFF"" Padding=""12,6"" FontWeight=""Bold""/>
        </StackPanel>
    </Border>
</Grid>",
                    XamlExplanation = "Demonstração de composição de árvore visual (Border -> StackPanel -> Canvas -> Shapes), onde propriedades como Fill, Stroke e CornerRadius são Dependency Properties cacheadas na memória esparsa do WPF.",
                    ControlsDescription = "Ajuste o controle deslizante de escala de layout para observar a árvore visual e a restrição de tamanho calculada.",
                    QuizQuestion = "Qual é a principal vantagem das Dependency Properties em relação às propriedades C# normais com campos privados?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Economia massiva de memória (armazenamento esparso) e suporte nativo a estilos, animações e DataBinding", IsCorrect = true, Explanation = "Correto! Controles WPF têm centenas de propriedades, mas uma DP só consome memória se tiver um valor diferente do padrão." },
                        new QuizOption { Text = "Elas funcionam sem o runtime .NET", IsCorrect = false, Explanation = "Incorreto. Elas são parte do framework .NET/WPF." },
                        new QuizOption { Text = "Elas substituem o Garbage Collector", IsCorrect = false, Explanation = "Incorreto. DPs são gerenciadas pelo CLR." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Visão geral das propriedades de dependência (WPF)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/properties/dependency-properties-overview",
                            Description = "Como o sistema de propriedades do WPF calcula valores efetivos e gerencia herança."
                        },
                        new DocReference
                        {
                            Title = "Sistema de Layout do WPF (Measure & Arrange)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/layout",
                            Description = "As fases de medição e organização de elementos na árvore visual."
                        }
                    }
                },
                #endregion

                #region Lição 5
                new InteractiveLesson
                {
                    Number = 5,
                    Id = "lesson_wpf_lifecycle",
                    Title = "5. WPF Threading, Dispatcher & Ciclo do WriteableBitmap",
                    Module = "Módulo 2: Arquitetura & Renderização no WPF",
                    Type = LessonType.WriteableBitmapLifecycle,
                    Summary = "Como o WPF sincroniza a CPU com a placa de vídeo através do ciclo Lock -> Edit -> AddDirtyRect -> Unlock e do Dispatcher.",
                    Theory =
                        "• O Ciclo de Vida em 4 Etapas:\n" +
                        "  1. Lock(): Bloqueia o buffer traseiro na RAM para que o Garbage Collector (GC) não o mova de endereço.\n" +
                        "  2. Edição de Pixels: A CPU escreve diretamente na memória RAM (em paralelo com Parallel.For).\n" +
                        "  3. AddDirtyRect(Int32Rect): Informa ao subsistema milcore/DirectX exatamente qual região retangular foi alterada.\n" +
                        "  4. Unlock(): Libera o buffer e aciona o redesenho com aceleração de hardware pela GPU.\n\n" +
                        "• O Dispatcher e Threads no WPF:\n" +
                        "  Apenas a UI Thread pode alterar elementos visuais. Cálculos pesados rodam em background tasks e usam Dispatcher.InvokeAsync para atualizar a tela sem congelar o software.",
                    CodeSnippet =
@"// Padrão de ciclo de vida seguro no WPF:
bitmap.Lock();
try
{
    // 1. Escreve pixels diretamente no BackBuffer
    DirectDraw(bitmap.BackBuffer, bitmap.Stride);
    
    // 2. Notifica a GPU sobre a região alterada
    bitmap.AddDirtyRect(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight));
}
finally
{
    // 3. Sempre libera o lock no bloco finally
    bitmap.Unlock();
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'bitmap.Lock();' -> Bloqueia o BackBuffer do WriteableBitmap, garantindo exclusividade de acesso pela CPU.
2. 'try { ... } finally { bitmap.Unlock(); }' -> Padrão defensivo em C#: o bloco 'finally' assegura que o Unlock() sempre será invocado.
3. 'DirectDraw(...)' -> Escreve milhões de pixels em paralelo usando instruções SIMD e ponteiros.
4. 'bitmap.AddDirtyRect(new Int32Rect(0, 0, W, H));' -> Sinaliza para a camada milcore em C++ a área retangular que sofreu mutação.
5. 'bitmap.Unlock();' -> Libera o lock e enfileira a atualização de textura no DirectX a 60+ FPS.",
                    ChallengeGoal = "Implemente GetLifecycleSequence retornando a string com a ordem das etapas: 'Lock -> Modificacao -> AddDirtyRect -> Unlock'.",
                    BlankTemplate =
@"public static string GetLifecycleSequence()
{
    // Retorne a sequência oficial:
    return """";
}",
                    StarterTemplate =
@"public static string GetLifecycleSequence()
{
    // TODO: A ordem deve conter Lock, AddDirtyRect e Unlock
    return ""Lock -> Modificacao -> AddDirtyRect -> Unlock"";
}",
                    SolutionCode =
@"public static string GetLifecycleSequence()
{
    return ""Lock -> Modificacao -> AddDirtyRect -> Unlock"";
}",
                    XamlSnippet =
@"<Grid Margin=""12"">
    <!-- Exemplo Pedagógico: Hospedagem de WriteableBitmap em Image WPF -->
    <Border Background=""#0F172A"" BorderBrush=""#1E293B"" BorderThickness=""1"" CornerRadius=""8"" Padding=""14"">
        <StackPanel HorizontalAlignment=""Center"" VerticalAlignment=""Center"">
            <TextBlock Text=""Renderização de Bitmap com NearestNeighbor Scaling"" Foreground=""#38BDF8"" FontSize=""14"" FontWeight=""Bold"" Margin=""0,0,0,6"" HorizontalAlignment=""Center""/>
            <TextBlock Text=""RenderOptions.BitmapScalingMode garante pixels nítidos sem borrão bilinear."" Foreground=""#94A3B8"" FontSize=""11.5"" Margin=""0,0,0,10"" HorizontalAlignment=""Center""/>
            <Border BorderBrush=""#3B82F6"" BorderThickness=""2"" CornerRadius=""4"" Background=""#000000"" Padding=""4"">
                <!-- Controle Image conectado ao buffer de vídeo -->
                <Image Width=""220"" Height=""120"" RenderOptions.BitmapScalingMode=""NearestNeighbor""/>
            </Border>
        </StackPanel>
    </Border>
</Grid>",
                    XamlExplanation = "Mostra como o controle <Image> do WPF hospeda um WriteableBitmap com modo de interpolação NearestNeighbor, essencial para visualização nítida de pixels individuais em Computação Gráfica.",
                    ControlsDescription = "Clique no botão 'Simular Próximo Passo' para acompanhar o ciclo de vida do buffer e o envio à GPU.",
                    QuizQuestion = "O que acontece se um algoritmo esquecer de chamar o método AddDirtyRect() antes do Unlock()?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "O programa trava com AccessViolationException", IsCorrect = false, Explanation = "Incorreto. A gravação na RAM ocorre normalmente, não há erro de memória." },
                        new QuizOption { Text = "Os pixels são alterados na RAM, mas a tela não é redesenhada pelo DirectX", IsCorrect = true, Explanation = "Correto! Sem o AddDirtyRect, o WPF supõe que a textura na GPU ainda é válida e não envia os novos dados." },
                        new QuizOption { Text = "O Garbage Collector descarrega o Bitmap da memória", IsCorrect = false, Explanation = "Incorreto. O GC não é acionado por esse motivo." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe WriteableBitmap (WPF)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.imaging.writeablebitmap",
                            Description = "Guia completo de métodos de sincronização e controle de dirty rects."
                        },
                        new DocReference
                        {
                            Title = "Modelo de Threading do WPF e Dispatcher",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/threading-model",
                            Description = "Como a UI Thread e o Dispatcher coordenam a renderização assíncrona."
                        }
                    }
                },
                #endregion

                #region Lição 6
                new InteractiveLesson
                {
                    Number = 6,
                    Id = "lesson_pdi_convolution",
                    Title = "6. Convolução Espacial 2D Passo a Passo",
                    Module = "Módulo 3: Processamento Digital de Imagens (PDI)",
                    Type = LessonType.ConvolutionStepByStep,
                    Summary = "Como uma matriz 3x3 (Kernel) desliza sobre a imagem calculando médias ponderadas de pixels vizinhos.",
                    Theory =
                        "• O que é um Kernel de Convolução?\n" +
                        "  É uma matriz pequena (ex: 3x3) contendo pesos multiplicadores.\n\n" +
                        "• Equação Discreta:\n" +
                        "  g(x, y) = (Σ f(x - u, y - v) * K(u, v)) / Divisor + Bias\n\n" +
                        "• Exemplo - Box Blur (Média 3x3):\n" +
                        "  Todos os 9 pesos são 1, com divisor = 9. O pixel central torna-se a média aritmética de seus 8 vizinhos.",
                    CodeSnippet =
@"// Convolução discreta em C#:
double sum = 0;
for (int ky = -1; ky <= 1; ky++) {
    for (int kx = -1; kx <= 1; kx++) {
        byte neighbor = src[y + ky, x + kx];
        double weight = kernel[ky + 1, kx + 1];
        sum += neighbor * weight;
    }
}
byte result = (byte)Math.Clamp(sum / divisor + bias, 0, 255);",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'double sum = 0;' -> Acumulador de precisão de 64 bits para evitar estouro de valores intermediários.
2. 'for (int ky = -1; ky <= 1; ky++)' -> Laço aninhado vertical percorrendo linha acima, central e abaixo.
3. 'for (int kx = -1; kx <= 1; kx++)' -> Laço horizontal percorrendo coluna à esquerda, central e à direita.
4. 'byte neighbor = src[y + ky, x + kx];' -> Lê a intensidade do pixel vizinho.
5. 'sum += neighbor * weight;' -> Multiplica pelo peso da matriz de convolução.
6. 'Math.Clamp(sum / divisor + bias, 0, 255)' -> Normaliza e restringe para o intervalo de byte [0, 255].",
                    ChallengeGoal = "Implemente ApplyBoxBlur3x3 calculando a média aritmética dos 9 elementos de um grid 3x3.",
                    BlankTemplate =
@"public static int ApplyBoxBlur3x3(int[] grid9)
{
    // Calcule a média dos 9 valores:
    return 0;
}",
                    StarterTemplate =
@"public static int ApplyBoxBlur3x3(int[] grid9)
{
    // TODO: Somar os 9 valores e dividir por 9
    int sum = 0;
    for (int i = 0; i < 9; i++) sum += grid9[i];
    return sum / 9;
}",
                    SolutionCode =
@"public static int ApplyBoxBlur3x3(int[] grid9)
{
    int sum = 0;
    for (int i = 0; i < 9; i++) sum += grid9[i];
    return sum / 9;
}",
                    ControlsDescription = "Clique em 'Avançar Pixel' para ver a máscara 3x3 deslizando e calculando o pixel de destino.",
                    QuizQuestion = "Para aplicar um filtro de Nitidez (Sharpen) com kernel central = 5 e quatro vizinhos = -1, qual deve ser o divisor de normalização para preservar o brilho original?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Divisor = 9", IsCorrect = false, Explanation = "Incorreto. A soma dos pesos é 5 + (-1) + (-1) + (-1) + (-1) = 1." },
                        new QuizOption { Text = "Divisor = 1", IsCorrect = true, Explanation = "Correto! Como a soma dos pesos é 5 - 4 = 1, o divisor é 1 e o brilho médio da imagem permanece inalterado." },
                        new QuizOption { Text = "Divisor = 0", IsCorrect = false, Explanation = "Incorreto. Divisão por zero causaria erro de execução." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Parallel (System.Threading.Tasks)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.threading.tasks.parallel",
                            Description = "Paralelização de convoluções espaciais em múltiplos núcleos de CPU."
                        }
                    }
                },
                #endregion

                #region Lição 7
                new InteractiveLesson
                {
                    Number = 7,
                    Id = "lesson_pdi_otsu",
                    Title = "7. Limiarização Automática de Otsu Passo a Passo",
                    Module = "Módulo 3: Processamento Digital de Imagens (PDI)",
                    Type = LessonType.OtsuThresholdSearch,
                    Summary = "Como o algoritmo de Otsu analisa o histograma para encontrar automaticamente o limiar de corte ideal.",
                    Theory =
                        "• O Problema da Binarização:\n" +
                        "  Transformar uma imagem em tons de cinza em Preto (0) e Branco (255) separando o objeto do fundo.\n\n" +
                        "• O Critério de Otsu (1979):\n" +
                        "  Testa todos os limiares T de 0 a 255 e calcula a Variância Inter-Classes (σ²_B):\n" +
                        "  σ²_B(t) = ω0(t) * ω1(t) * [μ0(t) - μ1(t)]²\n\n" +
                        "• Onde:\n" +
                        "  - ω0, ω1: Proporção de pixels no fundo e no objeto.\n" +
                        "  - μ0, μ1: Nível médio de cinza do fundo e do objeto.\n" +
                        "  - O limiar ótimo T* é o ponto onde a variância é MÁXIMA.",
                    CodeSnippet =
@"// Algoritmo de Otsu em O(256):
double maxVariance = 0;
int bestThreshold = 0;

for (int t = 0; t < 256; t++) {
    double w0 = weightBackground[t];
    double w1 = weightForeground[t];
    double meanDiff = meanBackground[t] - meanForeground[t];
    double varianceBetween = w0 * w1 * meanDiff * meanDiff;
    
    if (varianceBetween > maxVariance) {
        maxVariance = varianceBetween;
        bestThreshold = t;
    }
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'double maxVariance = 0; int bestThreshold = 0;' -> Inicializa variáveis de rastreamento para o limiar que produz a maior separação estatística.
2. 'for (int t = 0; t < 256; t++)' -> Itera linearmente sobre todos os 256 níveis de cinza possíveis.
3. 'double w0 = weightBackground[t]; double w1 = weightForeground[t];' -> Proporção cumulativa de pixels das duas classes.
4. 'double meanDiff = meanBackground[t] - meanForeground[t];' -> Diferença entre os tons médios de cinza de cada classe.
5. 'double varianceBetween = w0 * w1 * meanDiff * meanDiff;' -> Variância inter-classes de Otsu.
6. 'if (varianceBetween > maxVariance)' -> Atualiza o limiar ótimo T* no instante do pico máximo.",
                    ChallengeGoal = "Implemente CalculateOtsuThreshold calculando o limiar que divide um conjunto de valores de pixels.",
                    BlankTemplate =
@"public static int CalculateOtsuThreshold(int[] pixelValues)
{
    // Calcule o limiar ótimo:
    return 128;
}",
                    StarterTemplate =
@"public static int CalculateOtsuThreshold(int[] pixelValues)
{
    // TODO: Calcular a média global dos valores
    int sum = 0;
    for (int i = 0; i < pixelValues.Length; i++) sum += pixelValues[i];
    return sum / pixelValues.Length;
}",
                    SolutionCode =
@"public static int CalculateOtsuThreshold(int[] pixelValues)
{
    if (pixelValues.Length == 0) return 128;
    int sum = 0;
    for (int i = 0; i < pixelValues.Length; i++) sum += pixelValues[i];
    return sum / pixelValues.Length;
}",
                    ControlsDescription = "Mova o slider do limiar T para observar a curva da variância inter-classes e o pico máximo detectado.",
                    QuizQuestion = "Qual é a complexidade de tempo do algoritmo de Otsu após a geração do histograma de 256 posições?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "O(W * H * 256) — Lento", IsCorrect = false, Explanation = "Incorreto. O histograma é calculado uma única vez em O(W*H), e a busca pelo limiar testa apenas 256 valores." },
                        new QuizOption { Text = "O(256) = O(1) — Extremamente rápido", IsCorrect = true, Explanation = "Correto! O laço de busca testa exatamente 256 passos fixos, executando em menos de 0.01 ms." },
                        new QuizOption { Text = "O(log N)", IsCorrect = false, Explanation = "Incorreto. A busca é linear sobre os 256 tons possíveis." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Manipulação de Arrays em C#",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/programming-guide/arrays/",
                            Description = "Alocação de buffers de contadores para histogramas estatísticos."
                        }
                    }
                },
                #endregion

                #region Lição 8
                new InteractiveLesson
                {
                    Number = 8,
                    Id = "lesson_cg2d_bresenham",
                    Title = "8. Reta de Bresenham (Aritmética 100% Inteira)",
                    Module = "Módulo 4: Computação Gráfica 2D (Rasterização)",
                    Type = LessonType.BresenhamStepByStep,
                    Summary = "Como as placas de vídeo desenham linhas retas na grade discreta de pixels sem nenhuma divisão.",
                    Theory =
                        "• O Desafio da Rasterização:\n" +
                        "  A equação contínua da reta y = m*x + b exige números de ponto flutuante (float) e divisões lentas.\n\n" +
                        "• A Sacada de Jack Bresenham (1965):\n" +
                        "  Multiplicou a equação por 2*Δx para eliminar as frações. Criou uma variável de decisão de erro 'e':\n" +
                        "  e_inicial = 2*Δy - Δx\n\n" +
                        "• A cada passo em X:\n" +
                        "  - Se e < 0: mantém o mesmo Y; e = e + 2*Δy\n" +
                        "  - Se e >= 0: sobe Y em +1; e = e + 2*(Δy - Δx)",
                    CodeSnippet =
@"// Reta de Bresenham com inteiros puros:
int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
int err = (dx > dy ? dx : -dy) / 2;

while (true) {
    SetPixel(x0, y0, color);
    if (x0 == x1 && y0 == y1) break;
    int e2 = err;
    if (e2 > -dx) { err -= dy; x0 += sx; }
    if (e2 < dy) { err += dx; y0 += sy; }
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);' -> Variação absoluta horizontal e vertical da reta.
2. 'int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;' -> Vetor de passo direcional unitário (+1 ou -1).
3. 'int err = (dx > dy ? dx : -dy) / 2;' -> Variável de decisão de Bresenham para eliminar qualquer divisão por float.
4. 'SetPixel(x0, y0, color);' -> Plota o pixel rasterizado diretamente na tela.
5. 'if (e2 > -dx) { err -= dy; x0 += sx; }' -> Avança no eixo X e desconta a inclinação acumulada.
6. 'if (e2 < dy) { err += dx; y0 += sy; }' -> Quando o erro cruza o limiar, ajusta o eixo Y.",
                    ChallengeGoal = "Implemente CountBresenhamPoints calculando o total de pixels desenhados ao rasterizar a reta.",
                    BlankTemplate =
@"public static int CountBresenhamPoints(int x0, int y0, int x1, int y1)
{
    // Retorne a quantidade de pontos da reta:
    return 0;
}",
                    StarterTemplate =
@"public static int CountBresenhamPoints(int x0, int y0, int x1, int y1)
{
    // TODO: Em retas com dx >= dy, o total de pontos é dx + 1
    int dx = Math.Abs(x1 - x0);
    return dx + 1;
}",
                    SolutionCode =
@"public static int CountBresenhamPoints(int x0, int y0, int x1, int y1)
{
    int dx = Math.Abs(x1 - x0);
    int dy = Math.Abs(y1 - y0);
    return Math.Max(dx, dy) + 1;
}",
                    ControlsDescription = "Clique em '▶️ Próximo Pixel' para plotar a reta pixel a pixel e acompanhar o valor da variável de decisão.",
                    QuizQuestion = "Por que o algoritmo de Bresenham foi um dos mais importantes da história da computação gráfica?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Porque permitiu desenhar retas perfeitas usando apenas adições, subtrações e inteiros", IsCorrect = true, Explanation = "Correto! Em 1965 os processadores não possuíam unidade de ponto flutuante (FPU), tornando o Bresenham viável e ultrarrápido." },
                        new QuizOption { Text = "Porque ele calcula sombras em 3D", IsCorrect = false, Explanation = "Incorreto. Bresenham é um algoritmo 2D de rasterização." },
                        new QuizOption { Text = "Porque ele utiliza inteligência artificial", IsCorrect = false, Explanation = "Incorreto. É um algoritmo matemático determinístico discreto." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Operadores aritméticos e tipos inteiros em C#",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/csharp/language-reference/operators/arithmetic-operators",
                            Description = "Aritmética inteira rápida sem conversões de tipo."
                        }
                    }
                },
                #endregion

                #region Lição 9
                new InteractiveLesson
                {
                    Number = 9,
                    Id = "lesson_cg2d_matrix",
                    Title = "9. Álgebra Linear 2D & Coordenadas Homogêneas (3x3)",
                    Module = "Módulo 4: Computação Gráfica 2D (Rasterização)",
                    Type = LessonType.MatrixTransform2D,
                    Summary = "Como matrizes 3x3 unificam translação, rotação e escala através de coordenadas homogêneas [x, y, 1].",
                    Theory =
                        "• O que são Coordenadas Homogêneas?\n" +
                        "  Um ponto 2D (x, y) é representado como um vetor 3D [x, y, 1]^T.\n\n" +
                        "• Por que matrizes 3x3?\n" +
                        "  Matrizes 2x2 conseguem fazer Rotação e Escala, mas NÃO conseguem fazer Translação (deslocamento). Com matrizes 3x3, a translação torna-se uma simples multiplicação matricial!\n\n" +
                        "• Matriz de Translação:\n" +
                        "  [[1, 0, Tx], [0, 1, Ty], [0, 0, 1]]\n\n" +
                        "• Composição Afim:\n" +
                        "  M_total = M_translacao * M_rotacao * M_escala",
                    CodeSnippet =
@"// Multiplicação de matriz 3x3 por ponto homogêneo [x, y, 1]:
double newX = m.M11 * x + m.M12 * y + m.M13 * 1.0;
double newY = m.M21 * x + m.M22 * y + m.M23 * 1.0;
// newX e newY são as novas coordenadas transformadas!",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'newX = m.M11 * x + m.M12 * y + m.M13 * 1.0;' -> Produto escalar da 1ª linha da matriz pelo vetor coluna [x, y, 1]. 'm.M13' adiciona a translação Tx.
2. 'newY = m.M21 * x + m.M22 * y + m.M23 * 1.0;' -> Produto escalar da 2ª linha da matriz pelo vetor coluna [x, y, 1]. 'm.M23' adiciona a translação Ty.
3. '1.0' -> A coordenada homogênea W = 1 permite transformar operações afins em multiplicações puras.",
                    ChallengeGoal = "Implemente TransformX e TransformY multiplicando o ponto (x, y) pela matriz de translação afim (tx, ty).",
                    BlankTemplate =
@"public static double TransformX(double x, double y, double tx, double ty)
{
    return x;
}
public static double TransformY(double x, double y, double tx, double ty)
{
    return y;
}",
                    StarterTemplate =
@"public static double TransformX(double x, double y, double tx, double ty)
{
    // TODO: X' = 1*x + 0*y + tx*1
    return x + tx;
}
public static double TransformY(double x, double y, double tx, double ty)
{
    // TODO: Y' = 0*x + 1*y + ty*1
    return y + ty;
}",
                    SolutionCode =
@"public static double TransformX(double x, double y, double tx, double ty)
{
    return x + tx;
}

public static double TransformY(double x, double y, double tx, double ty)
{
    return y + ty;
}",
                    ControlsDescription = "Altere os sliders de Translação (Tx, Ty), Rotação (Graus) e Escala para inspecionar a matriz 3x3 e o desenho resultante.",
                    QuizQuestion = "Se multiplicarmos a matriz de Translação T pela matriz de Rotação R, o resultado T * R é igual a R * T?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Sim, a multiplicação matricial é comutativa", IsCorrect = false, Explanation = "Incorreto. A ordem das transformações altera completamente a posição final do objeto." },
                        new QuizOption { Text = "Não, a multiplicação matricial NÃO é comutativa (A * B != B * A)", IsCorrect = true, Explanation = "Correto! Rotacionar e depois transladar gera um resultado visual completamente diferente de transladar e depois rotacionar." },
                        new QuizOption { Text = "Apenas se a escala for igual a 1", IsCorrect = false, Explanation = "Incorreto. Mesmo com escala 1, a ordem afeta a rotação orbital vs rotação sobre o próprio centro." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Estrutura Matrix (System.Windows.Media)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.matrix",
                            Description = "Representação nativa de matrizes de transformação 3x3 no WPF."
                        }
                    }
                },
                #endregion

                #region Lição 10
                new InteractiveLesson
                {
                    Number = 10,
                    Id = "lesson_cg3d_mvp",
                    Title = "10. O Pipeline 3D & A Divisão Perspectiva (1/Z)",
                    Module = "Módulo 5: Computação Gráfica 3D",
                    Type = LessonType.PipelineMVP3D,
                    Summary = "A jornada do vértice tridimensional (X, Y, Z) até o pixel bidimensional na tela (x, y).",
                    Theory =
                        "• O Pipeline MVP em 5 Etapas:\n" +
                        "  1. Model Matrix (M_model): Posiciona o objeto no mundo.\n" +
                        "  2. View Matrix (M_view): Move a cena para a perspectiva dos olhos da câmera (LookAt).\n" +
                        "  3. Projection Matrix (M_proj): Aplica o cone de visão (Frustum).\n" +
                        "  4. Divisão Perspectiva: Divide as coordenadas por W (onde W = Z da câmera). Isso faz objetos distantes ficarem menores!\n" +
                        "  5. Viewport Transform: Converte de NDC [-1, 1] para os pixels reais da tela (ex: 512x512).",
                    CodeSnippet =
@"// Projeção em perspectiva analítica:
// Ponto na câmera: (Xc, Yc, Zc)
double x_ndc = Xc / (Zc * tan(fov / 2));
double y_ndc = Yc / (Zc * tan(fov / 2));

// Mapeamento para pixels de tela:
int screenX = (int)((x_ndc + 1.0) * 0.5 * width);
int screenY = (int)((1.0 - y_ndc) * 0.5 * height);",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'Xc, Yc, Zc' -> Coordenadas do vértice transformadas para o espaço de visão da câmera (View Space).
2. 'tan(fov / 2)' -> Ajuste de abertura da lente da câmera (Campo de Visão).
3. 'Xc / Zc' -> DIVISÃO PERSPECTIVA: A divisão por Z comprime os pontos mais profundos.
4. 'x_ndc, y_ndc' -> Coordenadas no espaço normalizado NDC entre [-1.0 e +1.0].
5. '(x_ndc + 1.0) * 0.5 * width' -> Viewport Transform: Mapeia o intervalo NDC de [-1, 1] para [0, largura da tela em pixels].",
                    ChallengeGoal = "Implemente ProjectPerspectiveX projetando a coordenada 3D X na tela através da divisão perspectiva (X / Z) * fov.",
                    BlankTemplate =
@"public static double ProjectPerspectiveX(double x, double z, double fov)
{
    // Divisão perspectiva por Z:
    return 0.0;
}",
                    StarterTemplate =
@"public static double ProjectPerspectiveX(double x, double z, double fov)
{
    // TODO: Dividir X pela profundidade Z e multiplicar pela distância focal
    return (x / z) * fov;
}",
                    SolutionCode =
@"public static double ProjectPerspectiveX(double x, double z, double fov)
{
    return (x / z) * fov;
}",
                    ControlsDescription = "Ajuste a distância Z da câmera e o Campo de Visão (FOV) para observar a projeção dos vértices 3D.",
                    QuizQuestion = "Por que a divisão perspectiva divide as coordenadas X e Y exatamente pela coordenada Z (profundidade)?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Porque objetos com maior Z (mais distantes) devem parecer menores (proporção 1/Z)", IsCorrect = true, Explanation = "Correto! A semelhança de triângulos na óptica geométrica dita que o tamanho aparente decai com 1/distância." },
                        new QuizOption { Text = "Para evitar estouro de memória na GPU", IsCorrect = false, Explanation = "Incorreto. É um princípio da geometria projetiva, não uma restrição de memória." },
                        new QuizOption { Text = "Para calcular as cores dos pixels", IsCorrect = false, Explanation = "Incorreto. A cor é calculada pelos shaders de iluminação." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "System.Numerics.Matrix4x4 Struct",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.numerics.matrix4x4",
                            Description = "Matriz de transformação 4x4 otimizada com aceleração por hardware SIMD no .NET."
                        },
                        new DocReference
                        {
                            Title = "Visão geral de gráficos 3D no WPF",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/3-d-graphics-overview",
                            Description = "Pipeline 3D oficial do WPF com PerspectiveCamera e Viewport3D."
                        }
                    }
                },
                #endregion

                #region Lição 11
                new InteractiveLesson
                {
                    Number = 11,
                    Id = "lesson_cg3d_hierarchy",
                    Title = "11. Modelagem Hierárquica & Cinemática Direta",
                    Module = "Módulo 5: Computação Gráfica 3D",
                    Type = LessonType.HierarchicalSceneGraph,
                    Summary = "Grafos de cena (Scene Graph) e como transformações geométricas acumulam em cadeia pai-filho.",
                    Theory =
                        "• O que é um Grafo de Cena?\n" +
                        "  É uma árvore onde cada nó possui uma matriz de transformação local em relação ao seu pai.\n\n" +
                        "• Propagação em Cadeia:\n" +
                        "  M_global(Filho) = M_global(Pai) * M_local(Filho)\n\n" +
                        "• Exemplo do Robô Articulado (4 Níveis):\n" +
                        "  - Base (gira em Y) -> Ombro (gira em Z) -> Cotovelo (gira em Z) -> Garra (gira em X)\n" +
                        "  - Ao girar a Base, todas as juntas filhas giram juntas automaticamente sem recalcular posições absolutas!",
                    CodeSnippet =
@"// Propagação recursiva em Grafo de Cena:
public void UpdateGlobalMatrix(Matrix4x4 parentMatrix)
{
    GlobalMatrix = LocalMatrix * parentMatrix;
    foreach (var child in Children)
    {
        child.UpdateGlobalMatrix(GlobalMatrix); // Propaga para os filhos
    }
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'GlobalMatrix = LocalMatrix * parentMatrix;' -> Multiplicação matricial pai-filho. O nó herda a translação e rotação acumulada.
2. 'foreach (var child in Children)' -> Itera sobre a lista de juntas filhas conectadas nesta articulação.
3. 'child.UpdateGlobalMatrix(GlobalMatrix);' -> Chamada recursiva em profundidade (Depth-First Search) propagando as matrizes com complexidade O(N).",
                    ChallengeGoal = "Implemente CalculateEndEffectorX calculando a posição X da garra do robô através da cinemática direta dos 2 elos.",
                    BlankTemplate =
@"public static double CalculateEndEffectorX(double l1, double l2, double theta1Deg, double theta2Deg)
{
    // Cinemática direta do braço:
    return 0.0;
}",
                    StarterTemplate =
@"public static double CalculateEndEffectorX(double l1, double l2, double theta1Deg, double theta2Deg)
{
    // TODO: Converter graus para radianos e somar projeções dos 2 elos
    double rad1 = theta1Deg * Math.PI / 180.0;
    double rad2 = (theta1Deg + theta2Deg) * Math.PI / 180.0;
    return l1 * Math.Cos(rad1) + l2 * Math.Cos(rad2);
}",
                    SolutionCode =
@"public static double CalculateEndEffectorX(double l1, double l2, double theta1Deg, double theta2Deg)
{
    double rad1 = theta1Deg * Math.PI / 180.0;
    double rad2 = (theta1Deg + theta2Deg) * Math.PI / 180.0;
    return l1 * Math.Cos(rad1) + l2 * Math.Cos(rad2);
}",
                    ControlsDescription = "Mova os sliders de rotação da Base, Ombro e Cotovelo para ver o acúmulo de transformações nas juntas.",
                    QuizQuestion = "Em um sistema solar hierárquico (Sol -> Terra -> Lua), se o Sol se mover 100 unidades para a direita, o que acontece com a Lua?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "A Lua permanece parada na posição original", IsCorrect = false, Explanation = "Incorreto. A Lua é neta do Sol e herda todas as suas transformações globais." },
                        new QuizOption { Text = "A Lua move-se 100 unidades para a direita acompanhando a Terra e o Sol", IsCorrect = true, Explanation = "Correto! Por herança matricial em árvore, a translação do Sol é propagada automaticamente para a Terra e para a Lua." },
                        new QuizOption { Text = "A órbita da Lua é deformada", IsCorrect = false, Explanation = "Incorreto. O raio orbital local permanece intacto." }
                    },
                    MicrosoftReferences = new List<DocReference>
                    {
                        new DocReference
                        {
                            Title = "Classe Transform3DGroup (WPF)",
                            Url = "https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.media3d.transform3dgroup",
                            Description = "Agrupamento de transformações hierárquicas compostas no WPF."
                        }
                    }
                },
                #endregion

                #region Lição 12
                new InteractiveLesson
                {
                    Number = 12,
                    Id = "lesson_cg3d_raytracing",
                    Title = "12. Ray Tracing & Interseção Analítica Raio-Esfera",
                    Module = "Módulo 6: Renderização Realística",
                    Type = LessonType.RayTracingIntersection,
                    Summary = "Como resolver analiticamente a equação quadrática de interseção raio-esfera para gerar reflexões fotorrealistas.",
                    Theory =
                        "• A Equação Paramétrica do Raio:\n" +
                        "  r(t) = Origem + t * Direcao, com t > 0\n\n" +
                        "• Equação Implícita da Esfera:\n" +
                        "  |P - Centro|² = Raio²\n\n" +
                        "• Substituindo o raio na esfera:\n" +
                        "  a*t² + b*t + c = 0\n" +
                        "  Discriminante: Δ = b² - 4ac\n\n" +
                        "• Análise do Discriminante Δ:\n" +
                        "  - Se Δ < 0: O raio errou a esfera (sem interseção).\n" +
                        "  - Se Δ = 0: O raio é tangente (1 ponto de toque).\n" +
                        "  - Se Δ > 0: O raio atravessa a esfera (2 pontos; pegamos o menor t > 0, que é a superfície mais próxima).",
                    CodeSnippet =
@"// Interseção Raio-Esfera analítica:
Vec3 oc = ray.Origin - sphere.Center;
double b = 2.0 * Vec3.Dot(oc, ray.Direction);
double c = Vec3.Dot(oc, oc) - sphere.Radius * sphere.Radius;
double delta = b * b - 4.0 * c;

if (delta >= 0) {
    double t = (-b - Math.Sqrt(delta)) / 2.0; // Ponto de impacto mais próximo
    Vec3 hitPoint = ray.Origin + ray.Direction * t;
    Vec3 normal = (hitPoint - sphere.Center).Normalized();
}",
                    CodeExplanation =
@"🔍 Explicação Linha a Linha:
1. 'Vec3 oc = ray.Origin - sphere.Center;' -> Vetor que vai do centro da esfera até a origem do raio.
2. 'double b = 2.0 * Vec3.Dot(oc, ray.Direction);' -> Coeficiente linear derivado do produto escalar.
3. 'double delta = b * b - 4.0 * c;' -> Discriminante quadrático Δ.
4. 'double t = (-b - Math.Sqrt(delta)) / 2.0;' -> Fórmula quadrática (Bhaskara) selecionando a menor raiz positiva.
5. 'Vec3 hitPoint = ray.Origin + ray.Direction * t;' -> Calcula a coordenada 3D exata do ponto de colisão.
6. 'Vec3 normal = (hitPoint - sphere.Center).Normalized();' -> Vetor unitário perpendicular à superfície da esfera para reflexão Phong.",
                    ChallengeGoal = "Implemente IntersectRaySphere calculando o discriminante delta e retornando a menor raiz positiva t.",
                    BlankTemplate =
@"public static double IntersectRaySphere(double ox, double oy, double oz, double dx, double dy, double dz, double radius)
{
    // Interseção raio-esfera do zero:
    return -1.0;
}",
                    StarterTemplate =
@"public static double IntersectRaySphere(double ox, double oy, double oz, double dx, double dy, double dz, double radius)
{
    // TODO 1: Coeficientes a, b, c da equação quadrática
    double a = dx * dx + dy * dy + dz * dz;
    double b = 2.0 * (ox * dx + oy * dy + oz * dz);
    double c = (ox * ox + oy * oy + oz * oz) - radius * radius;
    double delta = b * b - 4.0 * a * c;

    // TODO 2: Se delta < 0, o raio não toca a esfera
    if (delta < 0) return -1.0;

    // TODO 3: Menor raiz t positiva
    double t = (-b - Math.Sqrt(delta)) / (2.0 * a);
    return t > 0 ? t : -1.0;
}",
                    SolutionCode =
@"public static double IntersectRaySphere(double ox, double oy, double oz, double dx, double dy, double dz, double radius)
{
    double a = dx * dx + dy * dy + dz * dz;
    double b = 2.0 * (ox * dx + oy * dy + oz * dz);
    double c = (ox * ox + oy * oy + oz * oz) - radius * radius;
    double delta = b * b - 4.0 * a * c;

    if (delta < 0) return -1.0;

    double t = (-b - Math.Sqrt(delta)) / (2.0 * a);
    return t > 0 ? t : -1.0;
}",
                    ControlsDescription = "Mova o slider da posição Y do raio para inspecionar o teste do discriminante Delta e os raios traçados.",
                    QuizQuestion = "Por que em um Ray Tracer tradicional disparamos os raios a partir da câmera em direção à cena, em vez de disparar da lâmpada?",
                    QuizOptions = new List<QuizOption>
                    {
                        new QuizOption { Text = "Porque apenas os raios que atingem a lente da câmera contribuem para os pixels da imagem final", IsCorrect = true, Explanation = "Correto! Uma lâmpada emite bilhões de raios que nunca chegam aos olhos. O traçado reverso calcula exclusivamente os raios visíveis." },
                        new QuizOption { Text = "Porque as lâmpadas não emitem luz vetorial", IsCorrect = false, Explanation = "Incorreto. A física da luz é bidirecional." },
                        new QuizOption { Text = "Para evitar reflexões no vidro", IsCorrect = false, Explanation = "Incorreto. O Ray Tracer calcula reflexões e refrações recursivas com precisão física." }
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
                }
                #endregion
            };
        }

        #region Métodos Geradores de Simulação Visual Didática

        /// <summary>
        /// Desenha a simulação visual passo a passo no DirectBitmap para cada lição.
        /// </summary>
        public static void RenderSimulation(DirectBitmap bmp, InteractiveLesson lesson, double p1, double p2, double p3, double p4, int stepIndex, StringBuilder logOut)
        {
            bmp.Lock();
            bmp.Clear(Color.FromRgb(14, 14, 20));

            switch (lesson.Type)
            {
                case LessonType.BgraMemoryLayout:
                    RenderBgraSimulation(bmp, p1, p2, p3, p4, logOut);
                    break;
                case LessonType.CSharpPropertiesAndNotify:
                    RenderPropertiesAndNotifySimulation(bmp, p1, logOut);
                    break;
                case LessonType.PointerStrideOffset:
                    RenderPointerStrideSimulation(bmp, (int)p1, (int)p2, logOut);
                    break;
                case LessonType.WpfXamlAndDependencyProps:
                    RenderXamlAndLayoutSimulation(bmp, p1, p2, logOut);
                    break;
                case LessonType.WriteableBitmapLifecycle:
                    int lifeStep = (int)Math.Clamp(p1 - 1, 0, 3);
                    if (stepIndex > 0) lifeStep = (stepIndex % 4);
                    RenderLifecycleSimulation(bmp, lifeStep, logOut);
                    break;
                case LessonType.ConvolutionStepByStep:
                    int convKx = (int)Math.Clamp(p1 - 1, 0, 3);
                    int convKy = (int)Math.Clamp(p2 - 1, 0, 3);
                    if (stepIndex > 0) { convKx = (stepIndex % 4); convKy = (stepIndex / 4) % 4; }
                    RenderConvolutionSimulation(bmp, convKx, convKy, (int)p3, logOut);
                    break;
                case LessonType.OtsuThresholdSearch:
                    RenderOtsuSimulation(bmp, p1, logOut);
                    break;
                case LessonType.BresenhamStepByStep:
                    RenderBresenhamSimulation(bmp, stepIndex, (int)p1, (int)p2, logOut);
                    break;
                case LessonType.MatrixTransform2D:
                    RenderMatrix2DSimulation(bmp, p1, 0, p2, p3, logOut);
                    break;
                case LessonType.PipelineMVP3D:
                    RenderPipeline3DSimulation(bmp, p1, p2, p3, logOut);
                    break;
                case LessonType.HierarchicalSceneGraph:
                    RenderHierarchySimulation(bmp, p1, p2, p3, logOut);
                    break;
                case LessonType.RayTracingIntersection:
                    RenderRayTracingSimulation(bmp, p1, logOut);
                    break;
            }

            bmp.Unlock(true);
        }

        private static void RenderBgraSimulation(DirectBitmap bmp, double blue, double green, double red, double alpha, StringBuilder log)
        {
            byte b = (byte)Math.Clamp(blue, 0, 255);
            byte g = (byte)Math.Clamp(green, 0, 255);
            byte r = (byte)Math.Clamp(red, 0, 255);
            byte a = (byte)Math.Clamp(alpha, 0, 255);

            Color color = Color.FromArgb(a, r, g, b);

            // Preenche painel visual central com a cor resultante
            for (int y = 50; y < 220; y++)
            {
                for (int x = 50; x < 250; x++)
                {
                    bmp.SetPixel(x, y, color);
                }
            }

            // Desenha grade representativa dos 4 bytes na memória RAM
            DrawMemoryCell(bmp, 280, 70, 45, 120, Color.FromRgb(40, 100, 220), $"Byte 0 (B)\n{b} (0x{b:X2})");
            DrawMemoryCell(bmp, 335, 70, 45, 120, Color.FromRgb(40, 200, 80), $"Byte 1 (G)\n{g} (0x{g:X2})");
            DrawMemoryCell(bmp, 390, 70, 45, 120, Color.FromRgb(220, 50, 50), $"Byte 2 (R)\n{r} (0x{r:X2})");
            DrawMemoryCell(bmp, 445, 70, 45, 120, Color.FromRgb(180, 180, 200), $"Byte 3 (A)\n{a} (0x{a:X2})");

            log.AppendLine($"[Memória RAM] Formato BGRA32:");
            log.AppendLine($"• Byte 0 (Azul): {b} | Hex: 0x{b:X2} | Binário: {Convert.ToString(b, 2).PadLeft(8, '0')}");
            log.AppendLine($"• Byte 1 (Verde): {g} | Hex: 0x{g:X2} | Binário: {Convert.ToString(g, 2).PadLeft(8, '0')}");
            log.AppendLine($"• Byte 2 (Vermelho): {r} | Hex: 0x{r:X2} | Binário: {Convert.ToString(r, 2).PadLeft(8, '0')}");
            log.AppendLine($"• Byte 3 (Alpha): {a} | Hex: 0x{a:X2} | Binário: {Convert.ToString(a, 2).PadLeft(8, '0')}");
            log.AppendLine($"• Inteiro Compactado (uint 32-bit): 0x{a:X2}{r:X2}{g:X2}{b:X2}");
        }

        private static void RenderPropertiesAndNotifySimulation(DirectBitmap bmp, double propValue, StringBuilder log)
        {
            int val = (int)Math.Clamp(propValue, 0, 255);
            byte intensity = (byte)val;

            // Desenha caixa do Backing Field (Memória Privada)
            for (int y = 50; y < 140; y++)
                for (int x = 40; x < 220; x++)
                    bmp.SetPixel(x, y, Color.FromRgb(20, 30, 48));

            // Desenha caixa da View XAML (Data Binding)
            for (int y = 50; y < 140; y++)
                for (int x = 290; x < 470; x++)
                    bmp.SetPixel(x, y, Color.FromRgb(intensity, (byte)(intensity * 0.7), (byte)(255 - intensity)));

            // Desenha flecha de sincronização PropertyChanged no centro
            for (int x = 230; x < 280; x++)
                for (int y = 92; y < 98; y++)
                    bmp.SetPixel(x, y, Color.FromRgb(56, 189, 248));

            // Retângulo do valor
            for (int y = 160; y < 230; y++)
                for (int x = 120; x < 390; x++)
                    bmp.SetPixel(x, y, Color.FromRgb(15, 23, 42));

            log.AppendLine("[C# & WPF Data Binding: INotifyPropertyChanged]");
            log.AppendLine($"• Backing Field (_threshold): {val}");
            log.AppendLine($"• Propriedade Pública (Threshold): {val}");
            log.AppendLine($"• Evento Disparado: PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(\"Threshold\"))");
            log.AppendLine($"• Resposta da View XAML: Elemento atualizado reativamente na tela via Data Binding!");
        }

        private static void RenderXamlAndLayoutSimulation(DirectBitmap bmp, double desiredScale, double maxScale, StringBuilder log)
        {
            double scale = Math.Clamp(desiredScale, 10, 200);
            int boxWidth = (int)(scale * 1.8);
            int boxHeight = (int)(scale * 0.9);

            // Container Pai (Canvas / Grid: 400x200)
            for (int y = 40; y < 220; y++)
            {
                for (int x = 40; x < 472; x++)
                {
                    bool isBorder = (x == 40 || x == 471 || y == 40 || y == 219);
                    bmp.SetPixel(x, y, isBorder ? Color.FromRgb(59, 130, 246) : Color.FromRgb(10, 15, 26));
                }
            }

            // Elemento Filho (Measure & Arrange)
            int startX = 60;
            int startY = 60;
            int endX = Math.Min(startX + boxWidth, 450);
            int endY = Math.Min(startY + boxHeight, 200);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    bmp.SetPixel(x, y, Color.FromRgb(16, 185, 129));
                }
            }

            log.AppendLine("[WPF Sistema de Layout: Measure & Arrange]");
            log.AppendLine($"• Espaço Disponível do Pai: 432 x 180 px");
            log.AppendLine($"• Tamanho Desejado pelo Filho (MeasureOverride): {boxWidth} x {boxHeight} px");
            log.AppendLine($"• Retângulo Final Alocado (ArrangeOverride): ({startX}, {startY}, {endX - startX}, {endY - startY})");
            log.AppendLine($"• Árvore Visual: Window -> Grid -> Border -> DirectBitmap Canvas");
        }

        private static void RenderPointerStrideSimulation(DirectBitmap bmp, int targetX, int targetY, StringBuilder log)
        {
            int gridCols = 8;
            int gridRows = 8;
            int cellSize = 36;
            int startX = 40;
            int startY = 60;
            int stride = gridCols * 4; // 32 bytes

            targetX = Math.Clamp(targetX, 0, gridCols - 1);
            targetY = Math.Clamp(targetY, 0, gridRows - 1);

            int offset = (targetY * stride) + (targetX * 4);

            // Desenha grade 2D de pixels
            for (int r = 0; r < gridRows; r++)
            {
                for (int c = 0; c < gridCols; c++)
                {
                    bool isTarget = (c == targetX && r == targetY);
                    Color cellColor = isTarget ? Color.FromRgb(255, 180, 0) : Color.FromRgb(30, 35, 50);

                    int px = startX + c * (cellSize + 4);
                    int py = startY + r * (cellSize + 4);

                    for (int dy = 0; dy < cellSize; dy++)
                    {
                        for (int dx = 0; dx < cellSize; dx++)
                        {
                            bmp.SetPixel(px + dx, py + dy, cellColor);
                        }
                    }
                }
            }

            // Desenha fita de memória contínua linear 1D na base
            int memX = 40;
            int memY = 400;
            for (int i = 0; i < 32; i++)
            {
                bool isTargetByte = (i >= (offset % 32) && i < (offset % 32) + 4 && (offset / 32) == targetY);
                Color mCol = isTargetByte ? Color.FromRgb(255, 180, 0) : Color.FromRgb(45, 50, 70);
                for (int dy = 0; dy < 30; dy++)
                {
                    for (int dx = 0; dx < 11; dx++)
                    {
                        bmp.SetPixel(memX + i * 13 + dx, memY + dy, mCol);
                    }
                }
            }

            log.AppendLine($"[Aritmética de Ponteiros]:");
            log.AppendLine($"• Dimensões da Grade: {gridCols} colunas x {gridRows} linhas");
            log.AppendLine($"• Stride (Largura em Bytes): {gridCols} * 4 bytes = {stride} bytes por linha");
            log.AppendLine($"• Coordenada Selecionada: X = {targetX}, Y = {targetY}");
            log.AppendLine($"• Cálculo: Offset = (Y * Stride) + (X * 4)");
            log.AppendLine($"• Offset = ({targetY} * {stride}) + ({targetX} * 4) = {targetY * stride} + {targetX * 4} = {offset} bytes");
            log.AppendLine($"• Endereço Absoluto: BasePointer + 0x{offset:X4}");
        }

        private static void RenderLifecycleSimulation(DirectBitmap bmp, int step, StringBuilder log)
        {
            step = Math.Clamp(step, 0, 3);
            string[] stepsNames = {
                "1. Lock(): Bloqueia buffer traseiro na RAM (Fixa ponteiro)",
                "2. Edição de Pixels: CPU escreve diretamente na memória",
                "3. AddDirtyRect(): Notifica milcore sobre região alterada",
                "4. Unlock(): Libera lock e GPU renderiza via DirectX"
            };

            for (int i = 0; i < 4; i++)
            {
                Color boxColor = (i == step) ? Color.FromRgb(40, 180, 240) : Color.FromRgb(30, 32, 45);
                int py = 60 + i * 90;

                for (int y = 0; y < 65; y++)
                {
                    for (int x = 50; x < 460; x++)
                    {
                        bmp.SetPixel(x, py + y, boxColor);
                    }
                }
            }

            log.AppendLine($"[Ciclo de Vida do WriteableBitmap - Passo {step + 1} de 4]:");
            log.AppendLine($"• Etapa Atual: {stepsNames[step]}");
            if (step == 0)
            {
                log.AppendLine("  -> O WPF bloqueia o BackBuffer na memória RAM.");
                log.AppendLine("  -> O Garbage Collector não moverá esse bloco durante a escrita.");
            }
            else if (step == 1)
            {
                log.AppendLine("  -> Ponteiros diretos (unsafe byte*) escrevem valores BGRA32.");
                log.AppendLine("  -> Utilização de Parallel.For para distribuir as linhas entre os núcleos de CPU.");
            }
            else if (step == 2)
            {
                log.AppendLine("  -> Chamada: bitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height)).");
                log.AppendLine("  -> O WPF marca a área para sincronização de textura na GPU.");
            }
            else
            {
                log.AppendLine("  -> Chamada: bitmap.Unlock().");
                log.AppendLine("  -> O Direct3D/DirectX recebe a textura atualizada e exibe na tela a 60+ FPS!");
            }
        }

        private static void RenderConvolutionSimulation(DirectBitmap bmp, int kx, int ky, int divisor, StringBuilder log)
        {
            if (divisor <= 0) divisor = 9;
            kx = Math.Clamp(kx, 0, 3);
            ky = Math.Clamp(ky, 0, 3);

            // Desenha grade de entrada (6x6)
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    bool inKernel = (x >= kx && x < kx + 3 && y >= ky && y < ky + 3);
                    Color c = inKernel ? Color.FromRgb(240, 140, 40) : Color.FromRgb(40, 45, 60);

                    for (int dy = 0; dy < 35; dy++)
                    {
                        for (int dx = 0; dx < 35; dx++)
                        {
                            bmp.SetPixel(40 + x * 40 + dx, 80 + y * 40 + dy, c);
                        }
                    }
                }
            }

            // Desenha pixel resultante com intensidade calculada
            int pixelIntensity = Math.Clamp(200 / divisor, 20, 255);
            Color resColor = Color.FromRgb((byte)pixelIntensity, 180, 240);

            for (int dy = 0; dy < 60; dy++)
            {
                for (int dx = 0; dx < 60; dx++)
                {
                    bmp.SetPixel(340 + dx, 150 + dy, resColor);
                }
            }

            log.AppendLine($"[Convolução Espacial 2D - Posição ({kx + 1}, {ky + 1})]:");
            log.AppendLine($"• Posição Central do Kernel: X = {kx + 1}, Y = {ky + 1}");
            log.AppendLine($"• Divisor de Normalização: {divisor}");
            log.AppendLine($"• Pixel de Saída g({kx + 1}, {ky + 1}) gravado no buffer de destino.");
        }

        private static void RenderOtsuSimulation(DirectBitmap bmp, double currentT, StringBuilder log)
        {
            int threshold = (int)Math.Clamp(currentT, 0, 255);
            int bestT = 118; // Ponto ótimo de teste

            // Desenha histograma simulado
            for (int x = 0; x < 256; x++)
            {
                // Dois picos gaussianos (Fundo escuro e Objeto claro)
                double h1 = 120 * Math.Exp(-Math.Pow(x - 60, 2) / 400.0);
                double h2 = 140 * Math.Exp(-Math.Pow(x - 180, 2) / 600.0);
                int height = (int)(h1 + h2);

                Color col = (x < threshold) ? Color.FromRgb(60, 120, 220) : Color.FromRgb(220, 120, 60);
                if (x == threshold) col = Color.FromRgb(255, 255, 255);

                for (int y = 0; y < height; y++)
                {
                    bmp.SetPixel(80 + x, 320 - y, col);
                }
            }

            // Curva de variância
            for (int x = 10; x < 245; x++)
            {
                double varB = 80 * Math.Sin((x - 10) * Math.PI / 235.0);
                int vy = 450 - (int)varB;
                bmp.SetPixel(80 + x, vy, Color.FromRgb(40, 220, 120));
                bmp.SetPixel(80 + x, vy + 1, Color.FromRgb(40, 220, 120));
            }

            log.AppendLine($"[Algoritmo de Otsu]:");
            log.AppendLine($"• Limiar de Teste Atual T: {threshold}");
            log.AppendLine($"• Limiar Ótimo Detectado T*: {bestT}");
            log.AppendLine($"• Variância Inter-Classes σ²_B: {Math.Sin((threshold - 10) * Math.PI / 235.0):F4}");
            log.AppendLine(threshold == bestT ? "PICO MÁXIMO ENCONTRADO! Separação ótima entre objeto e fundo." : "-> Continue deslizando para encontrar o ponto de máximo.");
        }

        private static void RenderBresenhamSimulation(DirectBitmap bmp, int step, int x1, int y1, StringBuilder log)
        {
            int gridScale = 25;
            int startX = 30;
            int startY = 30;

            int x0 = 1; int y0 = 1;
            x1 = Math.Clamp(x1, 3, 15);
            y1 = Math.Clamp(y1, 2, 12);

            int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;

            int curX = x0, curY = y0;
            int currentStep = 0;
            bool drawAll = (step >= 20 || step == 0);

            while (true)
            {
                if (drawAll || currentStep <= step)
                {
                    // Plota pixel na grade
                    for (int gy = 0; gy < gridScale - 2; gy++)
                    {
                        for (int gx = 0; gx < gridScale - 2; gx++)
                        {
                            bmp.SetPixel(startX + curX * gridScale + gx, startY + curY * gridScale + gy, Color.FromRgb(40, 200, 240));
                        }
                    }
                }

                if (curX == x1 && curY == y1) break;
                if (!drawAll && currentStep == step) break;

                int e2 = err;
                if (e2 > -dx) { err -= dy; curX += sx; }
                if (e2 < dy) { err += dx; curY += sy; }
                currentStep++;
            }

            log.AppendLine($"[Reta de Bresenham]:");
            log.AppendLine($"• Ponto Inicial: ({x0}, {y0}) -> Ponto Final: ({x1}, {y1})");
            log.AppendLine($"• ΔX = {dx}, ΔY = {dy}");
            log.AppendLine($"• Pixel Atual Plotado: ({curX}, {curY})");
            log.AppendLine($"• Variável de Decisão 'err': {err}");
        }

        private static void RenderMatrix2DSimulation(DirectBitmap bmp, double tx, double ty, double angleDeg, double scale, StringBuilder log)
        {
            if (scale <= 0.05) scale = 1.0;
            double rad = angleDeg * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            // Vértices do triângulo original
            double[,] pts = { { 0, -60 }, { 50, 50 }, { -50, 50 } };

            int centerX = 256;
            int centerY = 256;

            // Desenha eixos cartesianos
            for (int x = 20; x < 492; x++) bmp.SetPixel(x, centerY, Color.FromRgb(40, 45, 60));
            for (int y = 20; y < 492; y++) bmp.SetPixel(centerX, y, Color.FromRgb(40, 45, 60));

            // Transforma vértices
            int[] transX = new int[3];
            int[] transY = new int[3];

            for (int i = 0; i < 3; i++)
            {
                double px = pts[i, 0] * scale;
                double py = pts[i, 1] * scale;

                double rx = px * cos - py * sin + tx;
                double ry = px * sin + py * cos + ty;

                transX[i] = (int)(centerX + rx);
                transY[i] = (int)(centerY + ry);
            }

            // Desenha linhas do triângulo
            DrawLine(bmp, transX[0], transY[0], transX[1], transY[1], Color.FromRgb(240, 180, 40));
            DrawLine(bmp, transX[1], transY[1], transX[2], transY[2], Color.FromRgb(240, 180, 40));
            DrawLine(bmp, transX[2], transY[2], transX[0], transY[0], Color.FromRgb(240, 180, 40));

            log.AppendLine($"[Matriz de Transformação Homogênea 3x3]:");
            log.AppendLine($"┌ {cos * scale,6:F2}  {-sin * scale,6:F2}  {tx,6:F1} ┐");
            log.AppendLine($"│ {sin * scale,6:F2}   {cos * scale,6:F2}  {ty,6:F1} │");
            log.AppendLine($"└   0.00    0.00    1.00 ┘");
            log.AppendLine($"• Translação: ({tx:F1}, {ty:F1}) | Rotação: {angleDeg:F1}° | Escala: {scale:F2}x");
        }

        private static void RenderPipeline3DSimulation(DirectBitmap bmp, double rotYDeg, double zDist, double fovDeg, StringBuilder log)
        {
            if (zDist < 1.0) zDist = 3.5;
            if (fovDeg < 30.0) fovDeg = 60.0;
            zDist = Math.Clamp(zDist, 1.0, 10.0);
            fovDeg = Math.Clamp(fovDeg, 30.0, 120.0);

            double rotRad = rotYDeg * Math.PI / 180.0;
            double cosR = Math.Cos(rotRad);
            double sinR = Math.Sin(rotRad);

            int centerX = 256;
            int centerY = 256;

            // Cubo 3D (8 vértices)
            double[,] vertices = {
                {-1, -1, -1}, {1, -1, -1}, {1, 1, -1}, {-1, 1, -1},
                {-1, -1,  1}, {1, -1,  1}, {1, 1,  1}, {-1, 1,  1}
            };

            int[] px = new int[8];
            int[] py = new int[8];

            double fovFactor = 1.0 / Math.Tan((fovDeg * Math.PI / 180.0) / 2.0);

            for (int i = 0; i < 8; i++)
            {
                double vx = vertices[i, 0];
                double vy = vertices[i, 1];
                double vz = vertices[i, 2];

                // Rotação Y
                double rx = vx * cosR + vz * sinR;
                double ry = vy;
                double rz = -vx * sinR + vz * cosR + zDist;

                if (rz < 0.1) rz = 0.1;

                // Divisão Perspectiva por Z
                double x_ndc = (rx * fovFactor) / rz;
                double y_ndc = (ry * fovFactor) / rz;

                px[i] = (int)(centerX + x_ndc * 180);
                py[i] = (int)(centerY - y_ndc * 180);
            }

            // Conecta arestas do cubo
            int[,] edges = {
                {0,1},{1,2},{2,3},{3,0},
                {4,5},{5,6},{6,7},{7,4},
                {0,4},{1,5},{2,6},{3,7}
            };

            for (int e = 0; e < 12; e++)
            {
                int v0 = edges[e, 0];
                int v1 = edges[e, 1];
                DrawLine(bmp, px[v0], py[v0], px[v1], py[v1], Color.FromRgb(40, 180, 240));
            }

            log.AppendLine($"[Pipeline MVP 3D & Projeção Perspectiva]:");
            log.AppendLine($"• Rotação Y: {rotYDeg:F0}°");
            log.AppendLine($"• Distância Z da Câmera: {zDist:F2}");
            log.AppendLine($"• Campo de Visão (FOV): {fovDeg:F1}°");
            log.AppendLine($"• Divisão Perspectiva: X_screen = (X * fovFactor) / Z");
        }

        private static void RenderHierarchySimulation(DirectBitmap bmp, double baseAngle, double shoulderAngle, double elbowAngle, StringBuilder log)
        {
            int rootX = 100;
            int rootY = 380;

            double bRad = baseAngle * Math.PI / 180.0;
            double sRad = shoulderAngle * Math.PI / 180.0;
            double eRad = elbowAngle * Math.PI / 180.0;

            int armLen1 = 120;
            int armLen2 = 100;

            // Ombro
            int shoulderX = rootX + (int)(30 * Math.Cos(bRad));
            int shoulderY = rootY - 40;

            // Cotovelo (herda ombro)
            int elbowX = shoulderX + (int)(armLen1 * Math.Cos(sRad));
            int elbowY = shoulderY - (int)(armLen1 * Math.Sin(sRad));

            // Garra (herda cotovelo + ombro)
            int wristX = elbowX + (int)(armLen2 * Math.Cos(sRad + eRad));
            int wristY = elbowY - (int)(armLen2 * Math.Sin(sRad + eRad));

            // Desenha articulações
            DrawLine(bmp, rootX, rootY, shoulderX, shoulderY, Color.FromRgb(150, 150, 170));
            DrawLine(bmp, shoulderX, shoulderY, elbowX, elbowY, Color.FromRgb(40, 180, 240));
            DrawLine(bmp, elbowX, elbowY, wristX, wristY, Color.FromRgb(240, 180, 40));

            // Juntas em círculos
            DrawCircle(bmp, rootX, rootY, 12, Color.FromRgb(200, 200, 220));
            DrawCircle(bmp, shoulderX, shoulderY, 10, Color.FromRgb(40, 180, 240));
            DrawCircle(bmp, elbowX, elbowY, 8, Color.FromRgb(240, 180, 40));
            DrawCircle(bmp, wristX, wristY, 6, Color.FromRgb(220, 60, 60));

            log.AppendLine($"[Modelagem Hierárquica & Grafo de Cena]:");
            log.AppendLine($"• 1. Base Fixa: ({rootX}, {rootY})");
            log.AppendLine($"• 2. Ombro: ({shoulderX}, {shoulderY}) [Ângulo: {shoulderAngle:F1}°]");
            log.AppendLine($"• 3. Cotovelo (Filho do Ombro): ({elbowX}, {elbowY}) [Ângulo Relativo: {elbowAngle:F1}°]");
            log.AppendLine($"• 4. Garra (Neta do Ombro): ({wristX}, {wristY}) [Ângulo Global Acumulado: {shoulderAngle + elbowAngle:F1}°]");
        }

        private static void RenderRayTracingSimulation(DirectBitmap bmp, double rayYOffset, StringBuilder log)
        {
            int cx = 300;
            int cy = 250;
            int radius = 80;

            // Desenha Esfera
            DrawCircle(bmp, cx, cy, radius, Color.FromRgb(40, 120, 220));

            // Raio Primário
            int rayOriginX = 40;
            int rayOriginY = 250 + (int)rayYOffset;

            int rayTargetX = 480;
            int rayTargetY = rayOriginY;

            // Teste de Discriminante Raio-Esfera
            double ocY = rayOriginY - cy;
            double r2 = radius * radius;
            double disc = r2 - (ocY * ocY);

            if (disc >= 0)
            {
                double dx = Math.Sqrt(disc);
                int hitX = (int)(cx - dx);
                int hitY = rayOriginY;

                // Raio até o impacto
                DrawLine(bmp, rayOriginX, rayOriginY, hitX, hitY, Color.FromRgb(255, 220, 0));

                // Ponto de Impacto
                DrawCircle(bmp, hitX, hitY, 5, Color.FromRgb(255, 60, 60));

                // Vetor Normal
                int nx = hitX + (int)((hitX - cx) * 0.5);
                int ny = hitY + (int)((hitY - cy) * 0.5);
                DrawLine(bmp, hitX, hitY, nx, ny, Color.FromRgb(60, 240, 120));

                // Raio Refletido
                DrawLine(bmp, hitX, hitY, hitX - 100, hitY + (int)(ocY * 1.5), Color.FromRgb(200, 100, 240));

                log.AppendLine($"[Ray Tracer - Interseção Raio-Esfera]:");
                log.AppendLine($"• Posição do Raio Y = {rayOriginY}");
                log.AppendLine($"• Discriminante Δ = {disc:F1} (Δ >= 0 -> IMPACTO CONFIRMADO)");
                log.AppendLine($"• Ponto de Impacto: ({hitX}, {hitY})");
                log.AppendLine($"• Vetor Normal N calculada para Iluminação Phong & Reflexão Especular.");
            }
            else
            {
                // Errou a esfera
                DrawLine(bmp, rayOriginX, rayOriginY, rayTargetX, rayTargetY, Color.FromRgb(100, 100, 120));
                log.AppendLine($"[Ray Tracer - Interseção Raio-Esfera]:");
                log.AppendLine($"• Posição do Raio Y = {rayOriginY}");
                log.AppendLine($"• Discriminante Δ = {disc:F1} (Δ < 0 -> O RAIO PASSOU NO VAZIO)");
            }
        }

        #endregion

        #region Utilitários de Desenho 2D Básico

        public static void DrawMemoryCell(DirectBitmap bmp, int x, int y, int w, int h, Color color, string label)
        {
            for (int dy = 0; dy < h; dy++)
            {
                for (int dx = 0; dx < w; dx++)
                {
                    bool isBorder = (dx == 0 || dx == w - 1 || dy == 0 || dy == h - 1);
                    bmp.SetPixel(x + dx, y + dy, isBorder ? Color.FromRgb(200, 200, 220) : color);
                }
            }
        }

        public static void DrawLine(DirectBitmap bmp, int x0, int y0, int x1, int y1, Color color)
        {
            int dx = Math.Abs(x1 - x0), dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1, sy = y0 < y1 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;

            while (true)
            {
                bmp.SetPixel(x0, y0, color);
                if (x0 == x1 && y0 == y1) break;
                int e2 = err;
                if (e2 > -dx) { err -= dy; x0 += sx; }
                if (e2 < dy) { err += dx; y0 += sy; }
            }
        }

        public static void DrawCircle(DirectBitmap bmp, int xc, int yc, int r, Color color)
        {
            for (int y = -r; y <= r; y++)
            {
                for (int x = -r; x <= r; x++)
                {
                    if (x * x + y * y <= r * r)
                    {
                        bmp.SetPixel(xc + x, yc + y, color);
                    }
                }
            }
        }

        #endregion
    }
}
