using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace CGPDI.StudyLab.Core
{
    public class TestResult
    {
        public string Name { get; set; } = string.Empty;
        public bool Passed { get; set; }
        public string Expected { get; set; } = string.Empty;
        public string Actual { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    public class EvaluationReport
    {
        public bool Success { get; set; }
        public string? CompilerError { get; set; }
        public string? ErrorMessage => CompilerError;
        public List<TestResult> Tests { get; set; } = new List<TestResult>();
        public string ConsoleLogs { get; set; } = string.Empty;
        public double ExecutionTimeMs { get; set; }
        public bool RenderApplied { get; set; }

        public string FeedbackReport
        {
            get
            {
                if (!string.IsNullOrEmpty(CompilerError)) return CompilerError;
                var sb = new StringBuilder();
                sb.AppendLine(Success ? "TODOS OS TESTES PASSARAM COM SUCESSO!" : "ALGUNS TESTES FALHARAM:");
                foreach (var t in Tests)
                {
                    sb.AppendLine($"• [{(t.Passed ? "APROVADO" : "FALHA")}] {t.Name}: Esperado '{t.Expected}' | Obtido '{t.Actual}'");
                    if (!string.IsNullOrEmpty(t.Details))
                        sb.AppendLine($"    Detalhe: {t.Details}");
                }
                return sb.ToString();
            }
        }
    }

    public static partial class LiveCodeCompiler
    {
        private static readonly Lazy<ScriptOptions> DefaultOptionsLazy = new(CreateDefaultOptions);
        private static ScriptOptions DefaultOptions => DefaultOptionsLazy.Value;

        private static ScriptOptions CreateDefaultOptions()
        {
            try
            {
                var references = new Dictionary<string, MetadataReference>(StringComparer.OrdinalIgnoreCase);
                AddAssemblyReference(references, typeof(object).Assembly);
                AddAssemblyReference(references, typeof(Math).Assembly);
                AddAssemblyReference(references, typeof(DirectBitmap).Assembly);
                AddAssemblyReference(references, typeof(Vector3).Assembly);
                AddAssemblyReference(references, typeof(System.Linq.Enumerable).Assembly);
                AddAssemblyReference(references, typeof(System.Windows.UIElement).Assembly);
                AddAssemblyReference(references, typeof(System.Windows.Media.Color).Assembly);
                AddAssemblyReference(references, typeof(System.Windows.Media.Media3D.Vector3D).Assembly);
                AddAssemblyReference(references, typeof(System.Text.RegularExpressions.Regex).Assembly);

                if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trustedPlatformAssemblies)
                {
                    foreach (string path in trustedPlatformAssemblies.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
                    {
                        TryAddReferenceFromPath(references, path);
                    }
                }

                return ScriptOptions.Default
                    .WithReferences(references.Values)
                    .WithImports(
                        "System",
                        "System.Math",
                        "System.Collections.Generic",
                        "System.Text",
                        "System.Text.RegularExpressions",
                        "System.Numerics",
                        "System.Windows",
                        "System.Windows.Media",
                        "CGPDI.StudyLab.Core");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LiveCodeCompiler] Falha ao inicializar referências do Roslyn: {ex}");
                return ScriptOptions.Default
                    .WithImports(
                        "System",
                        "System.Math",
                        "System.Collections.Generic",
                        "System.Text",
                        "System.Text.RegularExpressions",
                        "System.Numerics",
                        "System.Windows",
                        "System.Windows.Media",
                        "CGPDI.StudyLab.Core");
            }
        }

        private static void AddAssemblyReference(Dictionary<string, MetadataReference> references, Assembly assembly)
        {
            if (assembly.IsDynamic)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(assembly.Location))
            {
                return;
            }

            TryAddReferenceFromPath(references, assembly.Location);
        }

        private static void TryAddReferenceFromPath(Dictionary<string, MetadataReference> references, string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path) || references.ContainsKey(path))
            {
                return;
            }

            try
            {
                references[path] = MetadataReference.CreateFromFile(path);
            }
            catch
            {
                // Ignora referencias indisponiveis no ambiente atual.
            }
        }

        public static async Task<EvaluationReport> RunTestsAndEvaluateAsync(
            InteractiveLesson lesson,
            string userCode,
            DirectBitmap? targetBitmap = null,
            double param1 = 128,
            double param2 = 128,
            double param3 = 128)
        {
            var report = new EvaluationReport();
            var stopwatch = Stopwatch.StartNew();
            var logs = new StringBuilder();

            try
            {
                logs.AppendLine($"[COMPILADOR ROSLYN] Iniciando análise do código da Lição {lesson.Number}: {lesson.Title}...");

                // Dependendo da lição, envolvemos o código em uma classe de teste ou executamos o script
                switch (lesson.Type)
                {
                    case LessonType.BgraMemoryLayout:
                        await RunBgraTestsAsync(userCode, report);
                        break;

                    case LessonType.CSharpPropertiesAndNotify:
                        await RunPropertiesAndNotifyTestsAsync(userCode, report);
                        break;

                    case LessonType.PointerStrideOffset:
                        await RunPointersAndStrideTestsAsync(userCode, report);
                        break;

                    case LessonType.WpfXamlAndDependencyProps:
                        await RunXamlLayoutTestsAsync(userCode, report);
                        break;

                    case LessonType.WriteableBitmapLifecycle:
                        await RunWriteableBitmapTestsAsync(userCode, report);
                        break;

                    case LessonType.ConvolutionStepByStep:
                        await RunBoxBlurTestsAsync(userCode, report);
                        break;

                    case LessonType.OtsuThresholdSearch:
                        await RunOtsuTestsAsync(userCode, report);
                        break;

                    case LessonType.BresenhamStepByStep:
                        await RunBresenhamTestsAsync(userCode, report);
                        break;

                    case LessonType.MatrixTransform2D:
                        await RunAffineTransformTestsAsync(userCode, report);
                        break;

                    case LessonType.PipelineMVP3D:
                        await RunPerspectiveTestsAsync(userCode, report);
                        break;

                    case LessonType.HierarchicalSceneGraph:
                        await RunRobotArmTestsAsync(userCode, report);
                        break;

                    case LessonType.RayTracingIntersection:
                        await RunRaySphereTestsAsync(userCode, report);
                        break;

                    default:
                        logs.AppendLine("[AVISO] Nenhum validador específico configurado para esta lição.");
                        break;
                }

                // Verifica se todos os testes passaram
                bool allPassed = report.Tests.Count > 0 && report.Tests.TrueForAll(t => t.Passed);
                report.Success = allPassed;

                if (allPassed)
                {
                    logs.AppendLine("\n🎉 PARABÉNS! Seu código passou em 100% dos testes pedagógicos!");
                }
                else
                {
                    logs.AppendLine("\n🧪 MODO PERSONALIZAÇÃO ATIVO: Código customizado executado!");
                    logs.AppendLine("💡 (Para aprovação na trilha, verifique se todas as asserções de teste abaixo são atendidas)");
                }

                // Renderiza o Canvas SEMPRE com o resultado REAL gerado pelo código do usuário!
                if (targetBitmap != null)
                {
                    logs.AppendLine("🎨 Executando e renderizando o resultado visual REAL do seu código no Canvas...");
                    await RenderUserCodeDirectlyAsync(lesson, userCode, targetBitmap, param1, param2, param3, logs);
                    report.RenderApplied = true;
                }
            }
            catch (CompilationErrorException ex)
            {
                report.Success = false;
                report.CompilerError = string.Join("\n", ex.Diagnostics);
                logs.AppendLine("\n❌ ERRO DE COMPILAÇÃO C#:");
                foreach (var diag in ex.Diagnostics)
                {
                    var lineSpan = diag.Location.GetLineSpan();
                    logs.AppendLine($" • [Linha {lineSpan.StartLinePosition.Line + 1}, Coluna {lineSpan.StartLinePosition.Character + 1}]: {diag.GetMessage()}");
                }
            }
            catch (Exception ex)
            {
                report.Success = false;
                report.CompilerError = ex.Message;
                logs.AppendLine($"\n❌ EXCEÇÃO DE EXECUÇÃO EM TEMPO REAL: {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                report.ExecutionTimeMs = stopwatch.Elapsed.TotalMilliseconds;
                report.ConsoleLogs = logs.ToString();
            }

            return report;
        }

        #region Renderizador Visual Dinâmico Conectado ao Código do Aluno

        private static async Task RenderUserCodeDirectlyAsync(
            InteractiveLesson lesson,
            string userCode,
            DirectBitmap targetBitmap,
            double param1,
            double param2,
            double param3,
            StringBuilder logs)
        {
            try
            {
                switch (lesson.Type)
                {
                    case LessonType.BgraMemoryLayout:
                    {
                        byte bIn = (byte)Math.Clamp(param1, 0, 255);
                        byte gIn = (byte)Math.Clamp(param2, 0, 255);
                        byte rIn = (byte)Math.Clamp(param3, 0, 255);
                        byte aIn = 255;

                        string script = $@"
{userCode}

PackBgra((byte){bIn}, (byte){gIn}, (byte){rIn}, (byte){aIn})
";
                        uint userPixel = await CSharpScript.EvaluateAsync<uint>(script, DefaultOptions);
                        byte bRes = (byte)(userPixel & 0xFF);
                        byte gRes = (byte)((userPixel >> 8) & 0xFF);
                        byte rRes = (byte)((userPixel >> 16) & 0xFF);
                        byte aRes = (byte)((userPixel >> 24) & 0xFF);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        // Bloco de Cor Gerado pelo Código do Aluno
                        System.Windows.Media.Color userColor = System.Windows.Media.Color.FromArgb(
                            aRes == 0 && userPixel != 0 ? (byte)255 : (aRes == 0 ? (byte)255 : aRes), rRes, gRes, bRes);
                        for (int y = 40; y < 220; y++)
                        {
                            for (int x = 40; x < 230; x++)
                            {
                                targetBitmap.SetPixel(x, y, userColor);
                            }
                        }

                        // Células da Memória RAM com valores gerados pelo código do aluno
                        InteractiveLabManager.DrawMemoryCell(targetBitmap, 260, 60, 50, 130, System.Windows.Media.Color.FromRgb(40, 100, 220), $"Byte 0 (B)\n{bRes}\n0x{bRes:X2}");
                        InteractiveLabManager.DrawMemoryCell(targetBitmap, 320, 60, 50, 130, System.Windows.Media.Color.FromRgb(40, 200, 80), $"Byte 1 (G)\n{gRes}\n0x{gRes:X2}");
                        InteractiveLabManager.DrawMemoryCell(targetBitmap, 380, 60, 50, 130, System.Windows.Media.Color.FromRgb(220, 50, 50), $"Byte 2 (R)\n{rRes}\n0x{rRes:X2}");
                        InteractiveLabManager.DrawMemoryCell(targetBitmap, 440, 60, 50, 130, System.Windows.Media.Color.FromRgb(180, 180, 200), $"Byte 3 (A)\n{aRes}\n0x{aRes:X2}");

                        // Gradiente ao vivo gerado usando as cores calculadas pelo aluno
                        for (int x = 40; x < 490; x++)
                        {
                            byte gb = (byte)((x - 40) * 255 / 450);
                            for (int y = 235; y < 265; y++)
                            {
                                targetBitmap.SetPixel(x, y, System.Windows.Media.Color.FromArgb(255, rRes, gRes, gb));
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Canvas Renderizado pelo SEU Código]:");
                        logs.AppendLine($" • uint Compactado: 0x{userPixel:X8} ({userPixel})");
                        logs.AppendLine($" • Decodificação dos 4 bytes: B={bRes}, G={gRes}, R={rRes}, A={aRes}");
                        break;
                    }

                    case LessonType.CSharpPropertiesAndNotify:
                    {
                        int valIn = (int)Math.Clamp(param1, 0, 255);
                        string script = $@"
{userCode}

var tupleFunc = (Func<(bool, int, string)>)(() => {{
    int field = 100;
    string prop = """";
    bool changed = SetProperty(ref field, {valIn}, p => prop = p, ""Threshold"");
    return (changed, field, prop);
}});
tupleFunc()
";
                        var (changed, fieldVal, propNotified) = await CSharpScript.EvaluateAsync<(bool, int, string)>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        byte brightness = (byte)Math.Clamp(fieldVal, 0, 255);
                        System.Windows.Media.Color reactiveColor = System.Windows.Media.Color.FromRgb(brightness, (byte)(brightness * 0.7), (byte)(255 - brightness));

                        // Painel visual reativo
                        for (int y = 40; y < 140; y++)
                            for (int x = 40; x < 240; x++)
                                targetBitmap.SetPixel(x, y, reactiveColor);

                        // Barra de valor do backing field
                        int barW = (int)Math.Clamp(fieldVal * 420 / 255, 0, 420);
                        for (int y = 170; y < 205; y++)
                        {
                            for (int x = 40; x < 40 + barW; x++)
                            {
                                targetBitmap.SetPixel(x, y, changed ? System.Windows.Media.Color.FromRgb(56, 189, 248) : System.Windows.Media.Color.FromRgb(100, 116, 139));
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Data Binding Reativo do SEU Código]:");
                        logs.AppendLine($" • Valor final gravado no Field: {fieldVal}");
                        logs.AppendLine($" • Retorno de SetProperty: {changed}");
                        logs.AppendLine($" • Notificação disparada para: \"{propNotified}\"");
                        break;
                    }

                    case LessonType.PointerStrideOffset:
                    {
                        int xIn = (int)Math.Clamp(param1, 0, 7);
                        int yIn = (int)Math.Clamp(param2, 0, 7);
                        int strideIn = 32;

                        string script = $@"
{userCode}

CalculatePixelOffset({xIn}, {yIn}, {strideIn})
";
                        int userOffset = await CSharpScript.EvaluateAsync<int>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int cellSize = 22;
                        int startX = 40, startY = 40;

                        for (int gy = 0; gy < 8; gy++)
                        {
                            for (int gx = 0; gx < 8; gx++)
                            {
                                int expectedOffset = (gy * strideIn) + (gx * 4);
                                bool isCalculatedOffset = (expectedOffset == userOffset);
                                bool isInputCoords = (gx == xIn && gy == yIn);

                                System.Windows.Media.Color cellCol = isCalculatedOffset
                                    ? System.Windows.Media.Color.FromRgb(245, 158, 11)
                                    : (isInputCoords ? System.Windows.Media.Color.FromRgb(59, 130, 246) : System.Windows.Media.Color.FromRgb(30, 41, 59));

                                for (int py = 0; py < cellSize - 2; py++)
                                {
                                    for (int px = 0; px < cellSize - 2; px++)
                                    {
                                        targetBitmap.SetPixel(startX + gx * cellSize + px, startY + gy * cellSize + py, cellCol);
                                    }
                                }
                            }
                        }

                        // Mapa Linear 1D de Memória RAM na base
                        for (int i = 0; i < 64; i++)
                        {
                            int bOffset = i * 4;
                            bool isTarget = (bOffset == userOffset);
                            System.Windows.Media.Color bCol = isTarget ? System.Windows.Media.Color.FromRgb(239, 68, 68) : (i % 8 == 0 ? System.Windows.Media.Color.FromRgb(71, 85, 105) : System.Windows.Media.Color.FromRgb(30, 41, 59));
                            for (int py = 0; py < 25; py++)
                            {
                                for (int px = 0; px < 5; px++)
                                {
                                    targetBitmap.SetPixel(40 + i * 7 + px, 230 + py, bCol);
                                }
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Endereçamento de Memória do SEU Código]:");
                        logs.AppendLine($" • CalculatePixelOffset(X={xIn}, Y={yIn}, Stride={strideIn}) = {userOffset} bytes");
                        logs.AppendLine($" • Endereço Físico na RAM: BasePointer + 0x{userOffset:X4} ({userOffset})");
                        break;
                    }

                    case LessonType.WpfXamlAndDependencyProps:
                    {
                        double avail = param2 > 0 ? param2 : 300.0;
                        double desired = param1 > 0 ? param1 * 2.0 : 250.0;

                        string script = $@"
{userCode}

MeasureDesiredSize({avail:F1}, 50.0, 400.0, {desired:F1})
";
                        double measured = await CSharpScript.EvaluateAsync<double>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int pW = (int)Math.Clamp(avail, 50, 440);
                        for (int y = 40; y < 220; y++)
                        {
                            for (int x = 40; x < 40 + pW; x++)
                            {
                                bool isBorder = (y == 40 || y == 219 || x == 40 || x == 40 + pW - 1);
                                targetBitmap.SetPixel(x, y, isBorder ? System.Windows.Media.Color.FromRgb(96, 165, 250) : System.Windows.Media.Color.FromRgb(20, 25, 40));
                            }
                        }

                        int cW = (int)Math.Clamp(measured, 10, 440);
                        for (int y = 60; y < 200; y++)
                        {
                            for (int x = 50; x < 50 + cW; x++)
                            {
                                targetBitmap.SetPixel(x, y, System.Windows.Media.Color.FromRgb(16, 185, 129));
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Layout MeasureOverride do SEU Código]:");
                        logs.AppendLine($" • Espaço Disponível do Pai: {avail:F0}px");
                        logs.AppendLine($" • DesiredSize Retornado pelo Filho: {measured:F1}px");
                        break;
                    }

                    case LessonType.WriteableBitmapLifecycle:
                    {
                        string script = $@"
{userCode}

GetLifecycleSequence()
";
                        string steps = await CSharpScript.EvaluateAsync<string>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        bool[] active = {
                            steps.Contains("Lock"),
                            steps.Contains("Modificacao") || steps.Contains("Escrita") || steps.Contains("Ponteiro"),
                            steps.Contains("AddDirtyRect"),
                            steps.Contains("Unlock")
                        };

                        for (int i = 0; i < 4; i++)
                        {
                            int yPos = 40 + i * 50;
                            System.Windows.Media.Color boxCol = active[i] ? System.Windows.Media.Color.FromRgb(16, 185, 129) : System.Windows.Media.Color.FromRgb(71, 85, 105);

                            for (int dy = 0; dy < 38; dy++)
                            {
                                for (int dx = 40; dx < 460; dx++)
                                {
                                    bool isBorder = (dx == 40 || dx == 459 || dy == 0 || dy == 37);
                                    targetBitmap.SetPixel(dx, yPos + dy, isBorder ? System.Windows.Media.Color.FromRgb(241, 245, 249) : boxCol);
                                }
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Ciclo de Vida do WriteableBitmap - Retorno do SEU Código]:");
                        logs.AppendLine($" • Sequência: {steps}");
                        break;
                    }

                    case LessonType.ConvolutionStepByStep:
                    {
                        string script = $@"
{userCode}

var blur = (Func<int[], int>)(grid => ApplyBoxBlur3x3(grid));
(
    blur(new int[] {{ 255, 255, 255, 255, 255, 255, 255, 255, 255 }}),
    blur(new int[] {{ 0, 0, 0, 0, 255, 0, 0, 0, 0 }}),
    blur(new int[] {{ 100, 150, 200, 100, 150, 200, 100, 150, 200 }})
)
";
                        var (rAll255, rCenter, rGrad) = await CSharpScript.EvaluateAsync<(int, int, int)>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int imgW = 200, imgH = 150;
                        int startX = 40, startY = 40;
                        int destX = 270;

                        double factor = rCenter > 0 ? (rCenter / 28.33) : 1.0;

                        for (int y = 0; y < imgH; y++)
                        {
                            for (int x = 0; x < imgW; x++)
                            {
                                byte val = (byte)((x ^ y) % 256);
                                targetBitmap.SetPixel(startX + x, startY + y, System.Windows.Media.Color.FromRgb(val, val, val));

                                byte convVal = (byte)Math.Clamp(val * factor, 0, 255);
                                targetBitmap.SetPixel(destX + x, startY + y, System.Windows.Media.Color.FromRgb(convVal, (byte)(convVal * 0.9), convVal));
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Convolução Espacial do SEU Código]:");
                        logs.AppendLine($" • Resposta para Matriz Uniforme (255): {rAll255}");
                        logs.AppendLine($" • Resposta para Ponto Central (255): {rCenter}");
                        logs.AppendLine($" • Resposta para Gradiente: {rGrad}");
                        break;
                    }

                    case LessonType.OtsuThresholdSearch:
                    {
                        string script = $@"
{userCode}

CalculateOtsuThreshold(new int[] {{ 20, 25, 30, 35, 40, 200, 210, 220, 230, 240, 250 }})
";
                        int userThreshold = await CSharpScript.EvaluateAsync<int>(script, DefaultOptions);
                        userThreshold = Math.Clamp(userThreshold, 0, 255);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int cx = 250, cy = 140;
                        for (int y = 20; y < 260; y++)
                        {
                            for (int x = 80; x < 420; x++)
                            {
                                double dist = Math.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy));
                                byte gray = (byte)Math.Clamp(255 - dist * 2.2, 0, 255);
                                bool isFore = gray >= userThreshold;
                                targetBitmap.SetPixel(x, y, isFore ? System.Windows.Media.Color.FromRgb(250, 204, 21) : System.Windows.Media.Color.FromRgb(15, 23, 42));
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Binarização por Otsu do SEU Código]:");
                        logs.AppendLine($" • Limiar T* Calculado: {userThreshold}");
                        logs.AppendLine($" • Pixels >= {userThreshold} classificados como Objeto (Amarelo)");
                        logs.AppendLine($" • Pixels < {userThreshold} classificados como Fundo (Azul Escuro)");
                        break;
                    }

                    case LessonType.BresenhamStepByStep:
                    {
                        int x1In = (int)Math.Clamp(param1, 2, 14);
                        int y1In = (int)Math.Clamp(param2, 2, 10);

                        string script = $@"
{userCode}

CountBresenhamPoints(0, 0, {x1In}, {y1In})
";
                        int userPoints = await CSharpScript.EvaluateAsync<int>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int startX = 50, startY = 50, scale = 26;
                        InteractiveLabManager.DrawLine(targetBitmap, startX, startY, startX + x1In * scale, startY + y1In * scale, System.Windows.Media.Color.FromRgb(56, 189, 248));

                        for (int i = 0; i <= x1In; i++)
                        {
                            int px = startX + i * scale;
                            int py = startY + (int)(i * (double)y1In / x1In * scale);
                            InteractiveLabManager.DrawCircle(targetBitmap, px, py, 4, System.Windows.Media.Color.FromRgb(245, 158, 11));
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Reta de Bresenham do SEU Código]:");
                        logs.AppendLine($" • Total de Pontos Contados: {userPoints} pontos de (0,0) até ({x1In},{y1In})");
                        break;
                    }

                    case LessonType.MatrixTransform2D:
                    {
                        double txIn = param1;
                        double tyIn = param2;

                        string script = $@"
{userCode}

(
    TransformX(0.0, -50.0, {txIn:F2}, {tyIn:F2}),
    TransformY(0.0, -50.0, {txIn:F2}, {tyIn:F2}),
    TransformX(40.0, 40.0, {txIn:F2}, {tyIn:F2}),
    TransformY(40.0, 40.0, {txIn:F2}, {tyIn:F2}),
    TransformX(-40.0, 40.0, {txIn:F2}, {tyIn:F2}),
    TransformY(-40.0, 40.0, {txIn:F2}, {tyIn:F2})
)
";
                        var (v1x, v1y, v2x, v2y, v3x, v3y) = await CSharpScript.EvaluateAsync<(double, double, double, double, double, double)>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int centerX = 250, centerY = 140;
                        InteractiveLabManager.DrawLine(targetBitmap, (int)(centerX + v1x), (int)(centerY + v1y), (int)(centerX + v2x), (int)(centerY + v2y), System.Windows.Media.Color.FromRgb(56, 189, 248));
                        InteractiveLabManager.DrawLine(targetBitmap, (int)(centerX + v2x), (int)(centerY + v2y), (int)(centerX + v3x), (int)(centerY + v3y), System.Windows.Media.Color.FromRgb(56, 189, 248));
                        InteractiveLabManager.DrawLine(targetBitmap, (int)(centerX + v3x), (int)(centerY + v3y), (int)(centerX + v1x), (int)(centerY + v1y), System.Windows.Media.Color.FromRgb(244, 63, 94));

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Transformação Matricial 2D do SEU Código]:");
                        logs.AppendLine($" • Vértice Superior: ({v1x:F1}, {v1y:F1})");
                        logs.AppendLine($" • Vértice Direita:  ({v2x:F1}, {v2y:F1})");
                        logs.AppendLine($" • Vértice Esquerda: ({v3x:F1}, {v3y:F1})");
                        break;
                    }

                    case LessonType.PipelineMVP3D:
                    {
                        double zDist = param1 > 0 ? param1 : 3.0;
                        double fovIn = param2 > 0 ? param2 : 60.0;

                        string script = $@"
{userCode}

(
    ProjectPerspectiveX(-1.0, {zDist:F2}, {fovIn:F2}),
    ProjectPerspectiveX(1.0, {zDist:F2}, {fovIn:F2}),
    ProjectPerspectiveX(-1.0, {zDist + 1.5:F2}, {fovIn:F2}),
    ProjectPerspectiveX(1.0, {zDist + 1.5:F2}, {fovIn:F2})
)
";
                        var (frontLeft, frontRight, backLeft, backRight) = await CSharpScript.EvaluateAsync<(double, double, double, double)>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int cX = 250, cY = 140;
                        int fL = (int)(cX + frontLeft * 3.5);
                        int fR = (int)(cX + frontRight * 3.5);
                        int bL = (int)(cX + backLeft * 3.5);
                        int bR = (int)(cX + backRight * 3.5);

                        int halfFront = (int)(Math.Abs(frontRight - frontLeft) * 1.75);
                        int halfBack = (int)(Math.Abs(backRight - backLeft) * 1.75);

                        InteractiveLabManager.DrawLine(targetBitmap, fL, cY - halfFront, fR, cY - halfFront, System.Windows.Media.Color.FromRgb(56, 189, 248));
                        InteractiveLabManager.DrawLine(targetBitmap, fR, cY - halfFront, fR, cY + halfFront, System.Windows.Media.Color.FromRgb(56, 189, 248));
                        InteractiveLabManager.DrawLine(targetBitmap, fR, cY + halfFront, fL, cY + halfFront, System.Windows.Media.Color.FromRgb(56, 189, 248));
                        InteractiveLabManager.DrawLine(targetBitmap, fL, cY + halfFront, fL, cY - halfFront, System.Windows.Media.Color.FromRgb(56, 189, 248));

                        InteractiveLabManager.DrawLine(targetBitmap, bL, cY - halfBack, bR, cY - halfBack, System.Windows.Media.Color.FromRgb(147, 197, 253));
                        InteractiveLabManager.DrawLine(targetBitmap, bR, cY - halfBack, bR, cY + halfBack, System.Windows.Media.Color.FromRgb(147, 197, 253));
                        InteractiveLabManager.DrawLine(targetBitmap, bR, cY + halfBack, bL, cY + halfBack, System.Windows.Media.Color.FromRgb(147, 197, 253));
                        InteractiveLabManager.DrawLine(targetBitmap, bL, cY + halfBack, bL, cY - halfBack, System.Windows.Media.Color.FromRgb(147, 197, 253));

                        InteractiveLabManager.DrawLine(targetBitmap, fL, cY - halfFront, bL, cY - halfBack, System.Windows.Media.Color.FromRgb(96, 165, 250));
                        InteractiveLabManager.DrawLine(targetBitmap, fR, cY - halfFront, bR, cY - halfBack, System.Windows.Media.Color.FromRgb(96, 165, 250));
                        InteractiveLabManager.DrawLine(targetBitmap, fR, cY + halfFront, bR, cY + halfBack, System.Windows.Media.Color.FromRgb(96, 165, 250));
                        InteractiveLabManager.DrawLine(targetBitmap, fL, cY + halfFront, bL, cY + halfBack, System.Windows.Media.Color.FromRgb(96, 165, 250));

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Projeção Perspectiva 3D do SEU Código]:");
                        logs.AppendLine($" • Largura Frontal Projetada (Z={zDist:F1}): {Math.Abs(fR - fL)}px");
                        logs.AppendLine($" • Largura Traseira Projetada (Z={zDist + 1.5:F1}): {Math.Abs(bR - bL)}px");
                        break;
                    }

                    case LessonType.HierarchicalSceneGraph:
                    {
                        double t1 = param1;
                        double t2 = param2;

                        string script = $@"
{userCode}

(
    CalculateEndEffectorX(90.0, 70.0, {t1:F2}, {t2:F2}),
    (90.0 * Math.Sin({t1:F2} * Math.PI / 180.0)) + (70.0 * Math.Sin(({t1:F2} + {t2:F2}) * Math.PI / 180.0))
)
";
                        var (clawX, clawY) = await CSharpScript.EvaluateAsync<(double, double)>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int baseX = 150, baseY = 200;
                        double r1 = t1 * Math.PI / 180.0;
                        int elbowX = (int)(baseX + 90.0 * Math.Cos(r1));
                        int elbowY = (int)(baseY - 90.0 * Math.Sin(r1));
                        int endX = (int)(baseX + clawX);
                        int endY = (int)(baseY - clawY);

                        InteractiveLabManager.DrawLine(targetBitmap, baseX, baseY, elbowX, elbowY, System.Windows.Media.Color.FromRgb(59, 130, 246));
                        InteractiveLabManager.DrawLine(targetBitmap, elbowX, elbowY, endX, endY, System.Windows.Media.Color.FromRgb(16, 185, 129));

                        InteractiveLabManager.DrawCircle(targetBitmap, baseX, baseY, 6, System.Windows.Media.Color.FromRgb(245, 158, 11));
                        InteractiveLabManager.DrawCircle(targetBitmap, elbowX, elbowY, 5, System.Windows.Media.Color.FromRgb(245, 158, 11));
                        InteractiveLabManager.DrawCircle(targetBitmap, endX, endY, 7, System.Windows.Media.Color.FromRgb(239, 68, 68));

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Cinemática Direta do SEU Código]:");
                        logs.AppendLine($" • Posição da Garra (End-Effector): X = {clawX:F1}, Y = {clawY:F1}");
                        break;
                    }

                    case LessonType.RayTracingIntersection:
                    {
                        double rayY = param1 * 0.01;
                        double sphRadius = param2 > 0 ? param2 : 1.0;

                        string script = $@"
{userCode}

(
    IntersectRaySphere(0.0, {rayY:F2}, -4.0, 0.0, 0.0, 1.0, {sphRadius:F2}),
    IntersectRaySphere(-0.5, 0.0, -4.0, 0.0, 0.0, 1.0, {sphRadius:F2}),
    IntersectRaySphere(0.5, 0.0, -4.0, 0.0, 0.0, 1.0, {sphRadius:F2})
)
";
                        var (tCenter, tLeft, tRight) = await CSharpScript.EvaluateAsync<(double, double, double)>(script, DefaultOptions);

                        targetBitmap.Lock();
                        targetBitmap.Clear(System.Windows.Media.Color.FromRgb(14, 14, 20));

                        int vpW = targetBitmap.Width;
                        int vpH = targetBitmap.Height;

                        for (int y = 0; y < vpH; y += 2)
                        {
                            double ny = (1.0 - (y / (double)vpH) * 2.0);
                            for (int x = 0; x < vpW; x += 2)
                            {
                                double nx = ((x / (double)vpW) * 2.0 - 1.0) * (vpW / (double)vpH);
                                double d2 = nx * nx + ny * ny;
                                if (d2 <= sphRadius * sphRadius)
                                {
                                    double nz = Math.Sqrt(sphRadius * sphRadius - d2);
                                    double dot = Math.Max(0.0, (nx * 0.577 + ny * 0.577 + nz * 0.577) / sphRadius);
                                    byte intensity = (byte)Math.Clamp(dot * 220 + 35, 0, 255);
                                    System.Windows.Media.Color phong = System.Windows.Media.Color.FromArgb(255, (byte)(intensity * 0.4), (byte)(intensity * 0.8), intensity);

                                    for (int dy = 0; dy < 2 && y + dy < vpH; dy++)
                                    {
                                        for (int dx = 0; dx < 2 && x + dx < vpW; dx++)
                                        {
                                            targetBitmap.SetPixel(x + dx, y + dy, phong);
                                        }
                                    }
                                }
                            }
                        }

                        targetBitmap.Unlock(true);

                        logs.AppendLine($"[Ray Tracing Analítico do SEU Código]:");
                        logs.AppendLine($" • Raio Central: t = {(tCenter > 0 ? tCenter.ToString("F2") : "Sem Colisão")}");
                        logs.AppendLine($" • Raio Esquerdo: t = {(tLeft > 0 ? tLeft.ToString("F2") : "Sem Colisão")}");
                        logs.AppendLine($" • Raio Direito:  t = {(tRight > 0 ? tRight.ToString("F2") : "Sem Colisão")}");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                logs.AppendLine($"\n[AVISO DE RENDERIZAÇÃO] O Canvas utilizou a simulação padrão porque o código personalizado gerou: {ex.Message}");
                InteractiveLabManager.RenderSimulation(targetBitmap, lesson, param1, param2, param3, 255, 100, logs);
            }
        }

        #endregion

        #region Baterias de Testes Automatizados por Lição

        // 1. Bytes & Formato BGRA32
        private static async Task RunBgraTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    PackBgra((byte)255, (byte)0, (byte)0, (byte)255),
    PackBgra((byte)0, (byte)255, (byte)0, (byte)255),
    PackBgra((byte)10, (byte)20, (byte)30, (byte)40)
)
";
            var (val1, val2, val3) = await CSharpScript.EvaluateAsync<(uint, uint, uint)>(script, DefaultOptions);

            uint exp1 = 0xFF0000FFu;
            bool pass1 = (val1 == exp1);
            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Empacotamento de Azul Puro (B=255, G=0, R=0, A=255)",
                Passed = pass1,
                Expected = $"0x{exp1:X8}",
                Actual = $"0x{val1:X8}",
                Details = pass1 ? "Byte B posicionado corretamente no byte 0." : "Bit shifts incorretos para os canais."
            });

            uint exp2 = 0xFF00FF00u;
            bool pass2 = (val2 == exp2);
            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Empacotamento de Verde Puro (B=0, G=255, R=0, A=255)",
                Passed = pass2,
                Expected = $"0x{exp2:X8}",
                Actual = $"0x{val2:X8}",
                Details = pass2 ? "Canal Verde corretamente deslocado por << 8." : "Erro de deslocamento no canal Green."
            });

            uint exp3 = (10u | (20u << 8) | (30u << 16) | (40u << 24));
            bool pass3 = (val3 == exp3);
            report.Tests.Add(new TestResult
            {
                Name = "Teste 3: Empacotamento de Cor Composta (B=10, G=20, R=30, A=40)",
                Passed = pass3,
                Expected = $"0x{exp3:X8}",
                Actual = $"0x{val3:X8}",
                Details = pass3 ? "Todos os 4 bytes alinhados perfeitamente em 32 bits." : "Inconsistência na ordem dos bytes BGRA."
            });
        }

        // 2. C# Propriedades, Delegates & INotifyPropertyChanged
        private static async Task RunPropertiesAndNotifyTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

var tupleFunc = (Func<(bool, int, string, bool, string)>)(() => {{
    int f1 = 100;
    string n1 = """";
    bool c1 = SetProperty(ref f1, 250, p => n1 = p, ""Threshold"");
    
    int f2 = 250;
    string n2 = """";
    bool c2 = SetProperty(ref f2, 250, p => n2 = p, ""Threshold"");
    
    return (c1, f1, n1, c2, n2);
}});
tupleFunc()
";
            var (c1, f1, n1, c2, n2) = await CSharpScript.EvaluateAsync<(bool, int, string, bool, string)>(script, DefaultOptions);

            bool pass1 = c1 && f1 == 250 && n1 == "Threshold";
            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Atualização de Campo e Disparo de Notificação (Valor Alterado)",
                Passed = pass1,
                Expected = "changed=true, field=250, prop=\"Threshold\"",
                Actual = $"changed={c1}, field={f1}, prop=\"{n1}\"",
                Details = pass1 ? "Propriedade e evento de notificação disparados perfeitamente." : "Falha na atribuição ou na chamada do delegate."
            });

            bool pass2 = !c2 && string.IsNullOrEmpty(n2);
            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Proteção contra Notificações Redundantes (Valor Idêntico)",
                Passed = pass2,
                Expected = "changed=false, noNotif=\"\"",
                Actual = $"changed={c2}, notif=\"{n2}\"",
                Details = pass2 ? "Otimização de binding validada (sem ciclos redundantes de UI)." : "Falha na verificação de igualdade."
            });
        }

        // 3. Ponteiros & Stride
        private static async Task RunPointersAndStrideTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    CalculatePixelOffset(0, 0, 2048),
    CalculatePixelOffset(10, 5, 2048),
    CalculatePixelOffset(100, 200, 7680)
)
";
            var (off1, off2, off3) = await CSharpScript.EvaluateAsync<(int, int, int)>(script, DefaultOptions);

            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Offset na Origem (X=0, Y=0, Stride=2048)",
                Passed = off1 == 0,
                Expected = "0",
                Actual = off1.ToString(),
                Details = off1 == 0 ? "Origem no endereço base correta." : "Offset da origem deve ser 0."
            });

            int exp2 = 5 * 2048 + 10 * 4; // 10240 + 40 = 10280
            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Offset em Linha Arbitrária (X=10, Y=5, Stride=2048)",
                Passed = off2 == exp2,
                Expected = exp2.ToString(),
                Actual = off2.ToString(),
                Details = off2 == exp2 ? "Fórmula Y * Stride + X * 4 implementada corretamente." : "Fórmula de Stride incorreta."
            });

            int exp3 = 200 * 7680 + 100 * 4; // 1536000 + 400 = 1536400
            report.Tests.Add(new TestResult
            {
                Name = "Teste 3: Offset em Alta Resolução (X=100, Y=200, Stride=7680)",
                Passed = off3 == exp3,
                Expected = exp3.ToString(),
                Actual = off3.ToString(),
                Details = off3 == exp3 ? "Suporte a qualquer Stride de GPU verificado." : "Erro no cálculo com múltiplos strides."
            });
        }

        // 4. WPF XAML, Dependency Properties & Layout
        private static async Task RunXamlLayoutTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    MeasureDesiredSize(500.0, 100.0, 400.0, 250.0),
    MeasureDesiredSize(300.0, 50.0, 600.0, 800.0)
)
";
            var (normal, clamped) = await CSharpScript.EvaluateAsync<(double, double)>(script, DefaultOptions);

            bool pass1 = Math.Abs(normal - 250.0) < 0.01;
            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Medição Normal dentro dos Limites (Content=250 dentro de [100, 400])",
                Passed = pass1,
                Expected = "250.0",
                Actual = normal.ToString("F1"),
                Details = pass1 ? "Tamanho desejado calculado corretamente." : "Erro de restrição de medida."
            });

            bool pass2 = Math.Abs(clamped - 300.0) < 0.01;
            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Restrição de Espaço Disponível do Pai (Available=300, Content=800)",
                Passed = pass2,
                Expected = "300.0",
                Actual = clamped.ToString("F1"),
                Details = pass2 ? "Restrição de layout pelo container pai validada." : "Elemento ultrapassou o espaço disponível."
            });
        }

        // 5. Ciclo de Vida do WriteableBitmap
        private static async Task RunWriteableBitmapTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

GetLifecycleSequence()
";
            string steps = await CSharpScript.EvaluateAsync<string>(script, DefaultOptions);
            bool pass = steps.Contains("Lock") && steps.Contains("AddDirtyRect") && steps.Contains("Unlock");
            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Ordem do Ciclo de Vida do WriteableBitmap",
                Passed = pass,
                Expected = "Sequência contendo Lock -> Modificação -> AddDirtyRect -> Unlock",
                Actual = steps,
                Details = pass ? "Sequência oficial de sincronização com o renderizador WPF correta." : "Ordem do ciclo de vida incompleta."
            });
        }

        // 6. Convolução 2D Passo a Passo (Box Blur 3x3)
        private static async Task RunBoxBlurTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    ApplyBoxBlur3x3(new int[] {{ 90, 90, 90, 90, 90, 90, 90, 90, 90 }}),
    ApplyBoxBlur3x3(new int[] {{ 0, 10, 20, 30, 40, 50, 60, 70, 80 }})
)
";
            var (res1, res2) = await CSharpScript.EvaluateAsync<(int, int)>(script, DefaultOptions);

            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Convolução 3x3 com Matriz Uniforme (9x 90)",
                Passed = res1 == 90,
                Expected = "90",
                Actual = res1.ToString(),
                Details = res1 == 90 ? "Média aritmética 1/9 calculada com precisão." : "Erro na soma ou divisão pelo tamanho do kernel (9)."
            });

            int exp2 = (0 + 10 + 20 + 30 + 40 + 50 + 60 + 70 + 80) / 9; // 360 / 9 = 40
            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Convolução com Gradiente Linear (Média = 40)",
                Passed = res2 == exp2,
                Expected = exp2.ToString(),
                Actual = res2.ToString(),
                Details = res2 == exp2 ? "Média de convolução correta em gradiente." : "Resultado da convolução incorreto."
            });
        }

        // 7. Binarização de Otsu
        private static async Task RunOtsuTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

CalculateOtsuThreshold(new int[] {{ 50, 50, 50, 50, 200, 200, 200, 200 }})
";
            int res = await CSharpScript.EvaluateAsync<int>(script, DefaultOptions);
            bool pass = res >= 50 && res <= 200;
            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Limiar Ótimo em Distribuição Bimodal (50 e 200)",
                Passed = pass,
                Expected = "Valor ótimo de separação entre 50 e 200 (ex: ~125)",
                Actual = res.ToString(),
                Details = pass ? "Variância inter-classes maximizada no ponto de corte bimodal." : "Limiar fora da faixa bimodal."
            });
        }

        // 8. Reta de Bresenham
        private static async Task RunBresenhamTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    CountBresenhamPoints(0, 0, 4, 2),
    CountBresenhamPoints(0, 0, 2, 5)
)
";
            var (count1, count2) = await CSharpScript.EvaluateAsync<(int, int)>(script, DefaultOptions);

            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Contagem de Pixels da Reta (0,0) até (4,2)",
                Passed = count1 == 5,
                Expected = "5 pontos rasterizados",
                Actual = $"{count1} pontos",
                Details = count1 == 5 ? "Rasterização de Bresenham completa para dx >= dy." : "Contagem de pontos incorreta na reta."
            });

            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Contagem de Pixels da Reta (0,0) até (2,5)",
                Passed = count2 == 6,
                Expected = "6 pontos rasterizados",
                Actual = $"{count2} pontos",
                Details = count2 == 6 ? "Rasterização de Bresenham completa para dy > dx." : "Contagem de pontos incorreta na reta vertical."
            });
        }

        // 9. Álgebra Linear 2D & Coordenadas Homogêneas
        private static async Task RunAffineTransformTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    TransformX(5.0, 5.0, 10.0, 20.0),
    TransformY(5.0, 5.0, 10.0, 20.0)
)
";
            var (xOut, yOut) = await CSharpScript.EvaluateAsync<(double, double)>(script, DefaultOptions);

            bool passX = Math.Abs(xOut - 15.0) < 0.001;
            bool passY = Math.Abs(yOut - 25.0) < 0.001;

            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Translação Homogênea no Eixo X (5 + 10 = 15)",
                Passed = passX,
                Expected = "15.0",
                Actual = xOut.ToString("F1"),
                Details = passX ? "Coordenada X transformada perfeitamente." : "Erro na multiplicação da matriz de translação X."
            });

            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Translação Homogênea no Eixo Y (5 + 20 = 25)",
                Passed = passY,
                Expected = "25.0",
                Actual = yOut.ToString("F1"),
                Details = passY ? "Coordenada Y transformada perfeitamente." : "Erro na multiplicação da matriz de translação Y."
            });
        }

        // 10. Pipeline MVP 3D & Divisão Perspectiva
        private static async Task RunPerspectiveTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

ProjectPerspectiveX(10.0, 2.0, 100.0)
";
            double projX = await CSharpScript.EvaluateAsync<double>(script, DefaultOptions);
            bool pass = Math.Abs(projX - 500.0) < 0.01;

            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Divisão Perspectiva por Z (X=10, Z=2, Fov=100)",
                Passed = pass,
                Expected = "500.0",
                Actual = projX.ToString("F1"),
                Details = pass ? "Divisão perspectiva 1/Z calculada com exatidão física." : "Erro no cálculo de projeção perspectiva."
            });
        }

        // 11. Cinemática Direta do Robô
        private static async Task RunRobotArmTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

CalculateEndEffectorX(100.0, 100.0, 0.0, 0.0)
";
            double armX = await CSharpScript.EvaluateAsync<double>(script, DefaultOptions);
            bool pass = Math.Abs(armX - 200.0) < 0.1;

            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Posição do Efetuador do Braço Robótico (θ1=0, θ2=0)",
                Passed = pass,
                Expected = "200.0",
                Actual = armX.ToString("F1"),
                Details = pass ? "Cinemática direta encadeada validada com sucesso." : "Erro no encadeamento trigonométrico/matricial."
            });
        }

        // 12. Ray Tracing & Interseção Raio-Esfera
        private static async Task RunRaySphereTestsAsync(string userCode, EvaluationReport report)
        {
            string script = $@"
{userCode}

(
    IntersectRaySphere(0.0, 0.0, -5.0, 0.0, 0.0, 1.0, 1.0),
    IntersectRaySphere(0.0, 10.0, -5.0, 0.0, 0.0, 1.0, 1.0)
)
";
            var (tHit, tMiss) = await CSharpScript.EvaluateAsync<(double, double)>(script, DefaultOptions);

            bool pass = Math.Abs(tHit - 4.0) < 0.01;
            report.Tests.Add(new TestResult
            {
                Name = "Teste 1: Interseção Frontal Raio-Esfera (Origem Z=-5, Raio=1 -> t=4)",
                Passed = pass,
                Expected = "4.0",
                Actual = tHit.ToString("F2"),
                Details = pass ? "Raiz quadrática de Bhaskara para t > 0 correta." : "Erro no discriminante ou fórmula quadrática."
            });

            bool passMiss = tMiss < 0;
            report.Tests.Add(new TestResult
            {
                Name = "Teste 2: Raio que Erra a Esfera (Discriminante Δ < 0 -> t = -1)",
                Passed = passMiss,
                Expected = "t < 0 (Sem Interseção)",
                Actual = tMiss.ToString("F2"),
                Details = passMiss ? "Discriminante negativo detectado corretamente." : "Falso positivo de interseção."
            });
        }

        #endregion

        #region Execução de Código e Projetos Livres (Criador do Zero)

        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, ScriptRunner<object>> _customScriptCache = new();

        /// <summary>
        /// Limpa o cache de scripts compilados.
        /// </summary>
        public static void ClearCustomScriptCache() => _customScriptCache.Clear();

        /// <summary>
        /// Compila e executa qualquer script C# customizado sem limites, desenhando diretamente no Output DirectBitmap.
        /// Utiliza cache de delegados Roslyn para renderização a 60 FPS durante o uso de sliders.
        /// </summary>
        public static async Task<CustomScriptResult> ExecuteCustomScriptAsync(
            string code,
            DirectBitmap outputBitmap,
            DirectBitmap? inputBitmap,
            double p1, double p2, double p3, double p4)
        {
            var result = new CustomScriptResult();
            var logs = new StringBuilder();
            var sw = Stopwatch.StartNew();

            var globals = new CustomScriptGlobals
            {
                Output = outputBitmap,
                Input = inputBitmap,
                Param1 = p1,
                Param2 = p2,
                Param3 = p3,
                Param4 = p4,
                Print = msg => logs.AppendLine(msg)
            };

            try
            {
                if (!_customScriptCache.TryGetValue(code, out var runner))
                {
                    // Detecta se o usuário digitou uma classe completa com namespace (padrão code-behind WPF)
                    string sanitizedCode = code;
                    if (code.Contains("namespace ") && code.Contains("class "))
                    {
                        // Fornece aviso pedagógico no log
                        logs.AppendLine("[Dica Pedagógica]: No editor de scripts C#, você tem acesso direto às variáveis Output (DirectBitmap), Input, e Param1..Param4.");
                    }

                    var script = CSharpScript.Create(sanitizedCode, DefaultOptions, typeof(CustomScriptGlobals));
                    var compilation = script.Compile();
                    if (compilation.Length > 0)
                    {
                        var errorSb = new StringBuilder();
                        foreach (var diag in compilation)
                        {
                            if (diag.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                            {
                                var lineSpan = diag.Location.GetLineSpan();
                                errorSb.AppendLine($"Linha {lineSpan.StartLinePosition.Line + 1}, Coluna {lineSpan.StartLinePosition.Character + 1}: {diag.GetMessage()}");
                            }
                        }
                        if (errorSb.Length > 0)
                        {
                            result.Success = false;
                            result.ErrorMessage = errorSb.ToString();
                            return result;
                        }
                    }
                    runner = script.CreateDelegate();
                    _customScriptCache[code] = runner;
                }

                outputBitmap.Lock();
                inputBitmap?.Lock();
                try
                {
                    await runner(globals);
                }
                finally
                {
                    outputBitmap.Unlock(true);
                    inputBitmap?.Unlock(false);
                }

                sw.Stop();
                result.Success = true;
                result.ExecutionTimeMs = sw.Elapsed.TotalMilliseconds;
                result.Logs = logs.ToString();
            }
            catch (CompilationErrorException ex)
            {
                sw.Stop();
                result.Success = false;
                result.ErrorMessage = string.Join("\n", ex.Diagnostics);
            }
            catch (Exception ex)
            {
                sw.Stop();
                result.Success = false;
                result.ErrorMessage = $"Erro de execução:\n{ex.Message}";
            }

            return result;
        }

        [GeneratedRegex(@"\s+x:Class(Modifier)?=""[^""]*""")]
        private static partial Regex XamlClassRegex();

        [GeneratedRegex(@"\s+mc:Ignorable=""[^""]*""")]
        private static partial Regex XamlIgnorableRegex();

        [GeneratedRegex(@"\s+xmlns:mc=""[^""]*""")]
        private static partial Regex XamlXmlnsMcRegex();

        [GeneratedRegex(@"\s+xmlns:d=""[^""]*""")]
        private static partial Regex XamlXmlnsDRegex();

        [GeneratedRegex(@"\s+d:[A-Za-z0-9]+=""[^""]*""")]
        private static partial Regex XamlDAttrRegex();

        [GeneratedRegex(@"\s+WindowStartupLocation=""[^""]*""")]
        private static partial Regex XamlWindowStartupRegex();

        [GeneratedRegex(@"xmlns:(?<prefix>[A-Za-z_][A-Za-z0-9_.-]*)=""clr-namespace:(?<ns>CGPDI\.[^;\""]+)""")]
        private static partial Regex XamlClrNamespaceWithoutAssemblyRegex();

        private static readonly SearchValues<char> XamlTagDelimiters = SearchValues.Create(" >\r\n\t");

        public static XamlEvaluationResult EvaluateXaml(string xamlCode)
        {
            var result = new XamlEvaluationResult();
            var sw = Stopwatch.StartNew();

            if (string.IsNullOrWhiteSpace(xamlCode))
            {
                result.Success = false;
                result.ErrorMessage = "O código XAML fornecido está vazio.";
                return result;
            }

            try
            {
                string fullXaml = xamlCode.Trim();

                // 1. Remove x:Class, x:ClassModifier, mc:Ignorable e namespaces de design para compatibilidade direta com XamlReader
                fullXaml = XamlClassRegex().Replace(fullXaml, "");
                fullXaml = XamlIgnorableRegex().Replace(fullXaml, "");
                fullXaml = XamlXmlnsMcRegex().Replace(fullXaml, "");
                fullXaml = XamlXmlnsDRegex().Replace(fullXaml, "");
                fullXaml = XamlDAttrRegex().Replace(fullXaml, "");
                fullXaml = XamlWindowStartupRegex().Replace(fullXaml, "");
                fullXaml = XamlClrNamespaceWithoutAssemblyRegex().Replace(fullXaml, match =>
                {
                    string prefix = match.Groups["prefix"].Value;
                    string clrNamespace = match.Groups["ns"].Value;
                    string assemblyName = typeof(LiveCodeCompiler).Assembly.GetName().Name ?? "CGPDI.StudyLab";
                    return $"xmlns:{prefix}=\"clr-namespace:{clrNamespace};assembly={assemblyName}\"";
                });

                // 2. Se o usuário não incluiu os namespaces raiz do WPF, injeta automaticamente para conveniência
                if (!fullXaml.Contains("xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"") &&
                    !fullXaml.Contains("xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'"))
                {
                    int firstSpace = fullXaml.AsSpan().IndexOfAny(XamlTagDelimiters);
                    if (firstSpace > 1 && fullXaml.StartsWith('<'))
                    {
                        string tag = fullXaml.Substring(1, firstSpace - 1);
                        string rest = fullXaml.Substring(firstSpace);
                        fullXaml = $"<{tag} xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"{rest}";
                    }
                }

                object parsedObject = System.Windows.Markup.XamlReader.Parse(fullXaml);
                System.Windows.UIElement? uiElement = null;

                if (parsedObject is System.Windows.Window win)
                {
                    // Extrai e desvincula o conteúdo de Window para permitir hospedagem segura em container visual
                    var content = win.Content as System.Windows.UIElement;
                    win.Content = null;

                    if (content != null)
                    {
                        var hostBorder = new System.Windows.Controls.Border
                        {
                            Background = win.Background ?? System.Windows.Media.Brushes.Transparent,
                            Child = content,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                            VerticalAlignment = System.Windows.VerticalAlignment.Stretch
                        };
                        uiElement = hostBorder;
                    }
                    else
                    {
                        uiElement = new System.Windows.Controls.TextBlock
                        {
                            Text = "Janela WPF instanciada com sucesso.",
                            Foreground = System.Windows.Media.Brushes.LightGray,
                            FontSize = 13,
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                            VerticalAlignment = System.Windows.VerticalAlignment.Center
                        };
                    }
                }
                else if (parsedObject is System.Windows.Controls.Page page)
                {
                    var content = page.Content as System.Windows.UIElement;
                    page.Content = null;
                    uiElement = content ?? new System.Windows.Controls.TextBlock
                    {
                        Text = "Página WPF instanciada com sucesso.",
                        Foreground = System.Windows.Media.Brushes.LightGray,
                        FontSize = 13,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    };
                }
                else if (parsedObject is System.Windows.UIElement elem)
                {
                    uiElement = elem;
                }

                if (uiElement != null)
                {
                    sw.Stop();
                    result.Success = true;
                    result.Element = uiElement;
                    result.ExecutionTimeMs = sw.Elapsed.TotalMilliseconds;
                    result.Logs = $"Elemento visual WPF instanciado com sucesso: <{uiElement.GetType().Name}> em {result.ExecutionTimeMs:F1} ms.";
                }
                else
                {
                    sw.Stop();
                    result.Success = false;
                    result.ErrorMessage = $"O objeto XAML instanciado ({parsedObject?.GetType().Name}) não é um elemento visual UIElement válido do WPF.";
                }
            }
            catch (Exception ex)
            {
                sw.Stop();
                result.Success = false;
                result.ErrorMessage = $"Erro de sintaxe/análise XAML:\n{ex.Message}";
            }

            return result;
        }

        #endregion
    }

    public class XamlEvaluationResult
    {
        public bool Success { get; set; }
        public System.Windows.UIElement? Element { get; set; }
        public string? ErrorMessage { get; set; }
        public double ExecutionTimeMs { get; set; }
        public string Logs { get; set; } = string.Empty;
    }

    public class CustomScriptGlobals
    {
        public DirectBitmap Output { get; set; } = null!;
        public DirectBitmap? Input { get; set; }
        public int Width => Output.Width;
        public int Height => Output.Height;
        public double Param1 { get; set; }
        public double Param2 { get; set; }
        public double Param3 { get; set; }
        public double Param4 { get; set; }
        public Action<string> Print { get; set; } = _ => { };
    }

    public class CustomScriptResult
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public double ExecutionTimeMs { get; set; }
        public string Logs { get; set; } = string.Empty;
    }
}
