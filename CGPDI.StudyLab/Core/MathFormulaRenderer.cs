using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Motor de renderização e formatação visual de alta qualidade para fórmulas matemáticas e teoria científica no WPF.
    /// Converte notações LaTeX, subscritos, sobrescritos, símbolos gregos e equações em elementos visuais ricos e tipografia elegante.
    /// </summary>
    public static partial class MathFormulaRenderer
    {
        // Paleta de Cores Matemáticas Modernas
        private static readonly SolidColorBrush BrushBullet = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#38BDF8"));
        private static readonly SolidColorBrush BrushHeader = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#60A5FA"));
        private static readonly SolidColorBrush BrushText = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0"));
        private static readonly SolidColorBrush BrushMathVariable = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#93C5FD"));
        private static readonly SolidColorBrush BrushMathOperator = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FDE047"));
        private static readonly SolidColorBrush BrushMathGreek = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F472B6"));
        private static readonly SolidColorBrush BrushMathNumber = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#86EFAC"));
        private static readonly SolidColorBrush BrushSubSuper = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C084FC"));

        private static readonly SearchValues<char> BulletSearchChars = SearchValues.Create("•-");

        static MathFormulaRenderer()
        {
            BrushBullet.Freeze();
            BrushHeader.Freeze();
            BrushText.Freeze();
            BrushMathVariable.Freeze();
            BrushMathOperator.Freeze();
            BrushMathGreek.Freeze();
            BrushMathNumber.Freeze();
            BrushSubSuper.Freeze();
        }

        private static readonly Dictionary<string, string> GreekAndSymbols = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            { "\\alpha", "α" },
            { "\\beta", "β" },
            { "\\gamma", "γ" },
            { "\\delta", "δ" },
            { "\\theta", "θ" },
            { "\\sigma", "σ" },
            { "\\pi", "π" },
            { "\\lambda", "λ" },
            { "\\mu", "μ" },
            { "\\omega", "ω" },
            { "\\phi", "φ" },
            { "\\Delta", "Δ" },
            { "\\Sigma", "Σ" },
            { "\\Omega", "Ω" },
            { "\\Phi", "Φ" },
            { "\\times", "×" },
            { "\\cdot", "·" },
            { "\\approx", "≈" },
            { "\\neq", "≠" },
            { "\\le", "≤" },
            { "\\leq", "≤" },
            { "\\ge", "≥" },
            { "\\geq", "≥" },
            { "\\infty", "∞" },
            { "\\sqrt", "√" },
            { "\\ominus", "⊖" },
            { "\\oplus", "⊕" },
            { "\\circ", "○" },
            { "\\bullet", "●" },
            { "\\sum", "∑" },
            { "\\int", "∫" },
            { "\\in", "∈" },
            { "\\pm", "±" },
            { "\\nabla", "∇" },
            { "\\partial", "∂" },
            { "\\forall", "∀" },
            { "\\exists", "∃" }
        };

        private static readonly Dictionary<char, char> SuperscriptMap = new Dictionary<char, char>
        {
            { '0', '⁰' }, { '1', '¹' }, { '2', '²' }, { '3', '³' }, { '4', '⁴' },
            { '5', '⁵' }, { '6', '⁶' }, { '7', '⁷' }, { '8', '⁸' }, { '9', '⁹' },
            { '+', '⁺' }, { '-', '⁻' }, { '=', '⁼' }, { '(', '⁽' }, { ')', '⁾' },
            { 'n', 'ⁿ' }, { 'i', 'ⁱ' }, { 't', 'ᵗ' }, { 'x', 'ˣ' }, { 'y', 'ʸ' },
            { 'a', 'ᵃ' }, { 'b', 'ᵇ' }, { 'c', 'ᶜ' }, { 'd', 'ᵈ' }, { 'e', 'ᵉ' }
        };

        private static readonly Dictionary<char, char> SubscriptMap = new Dictionary<char, char>
        {
            { '0', '₀' }, { '1', '₁' }, { '2', '₂' }, { '3', '₃' }, { '4', '₄' },
            { '5', '₅' }, { '6', '₆' }, { '7', '₇' }, { '8', '₈' }, { '9', '₉' },
            { '+', '₊' }, { '-', '₋' }, { '=', '₌' }, { '(', '₍' }, { ')', '₎' },
            { 'a', 'ₐ' }, { 'e', 'ₑ' }, { 'h', 'ₕ' }, { 'i', 'ᵢ' }, { 'j', 'ⱼ' },
            { 'k', 'ₖ' }, { 'l', 'ₗ' }, { 'm', 'ₘ' }, { 'n', 'ₙ' }, { 'o', 'ₒ' },
            { 'p', 'ₚ' }, { 'r', 'ᵣ' }, { 's', 'ₛ' }, { 't', 'ₜ' }, { 'u', 'ᵤ' },
            { 'v', 'ᵥ' }, { 'x', 'ₓ' }
        };

        [GeneratedRegex(@"\\frac\{([^}]+)\}\{([^}]+)\}")]
        private static partial Regex FracRegex();

        [GeneratedRegex(@"_\{([^}]+)\}")]
        private static partial Regex SubRegex();

        [GeneratedRegex(@"\^\{([^}]+)\}")]
        private static partial Regex SuperRegex();

        [GeneratedRegex(@"(?<Sub>_[a-zA-Z0-9α-ωΑ-Ω]+)|(?<Super>\^[a-zA-Z0-9α-ωΑ-Ω\-+]+)|(?<Greek>[α-ωΑ-ΩΔΣΩΦ])|(?<Op>[+\-*/=×·≈≠≤≥∞√⊖⊕○●∑∫∈±∇∂])|(?<Number>\b\d+(\.\d+)?\b)|(?<Ident>[a-zA-Z_]\w*)|(?<Other>[^\s])")]
        private static partial Regex MathTokensRegex();

        /// <summary>
        /// Sanitiza e traduz texto com notações LaTeX para caracteres tipográficos matemáticos Unicode.
        /// </summary>
        public static string PreprocessMathText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            string result = input;

            // Substituição de frações LaTeX \frac{A}{B} -> (A / B)
            result = FracRegex().Replace(result, "($1 / $2)");

            // Substituição de símbolos gregos e operadores
            foreach (var kv in GreekAndSymbols)
            {
                result = result.Replace(kv.Key, kv.Value);
            }

            // Converte subscritos simples como _{proj} -> _proj, _{clip} -> _clip
            result = SubRegex().Replace(result, "_$1");
            // Converte sobrescritos simples como ^{2} -> ^2, ^{\alpha} -> ^α
            result = SuperRegex().Replace(result, "^$1");

            return result;
        }

        /// <summary>
        /// Renderiza o texto de fórmulas matemáticas com suporte a estilização rica no TextBlock fornecido.
        /// </summary>
        public static void RenderToTextBlock(TextBlock target, string mathText, double baseFontSize = 12.5)
        {
            if (target == null) return;
            target.Inlines.Clear();

            if (string.IsNullOrWhiteSpace(mathText)) return;

            string processed = PreprocessMathText(mathText);
            string[] lines = processed.Replace("\r\n", "\n").Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    target.Inlines.Add(new LineBreak());
                    continue;
                }

                RenderLineInlines(target.Inlines, line, baseFontSize);

                if (i < lines.Length - 1)
                {
                    target.Inlines.Add(new LineBreak());
                }
            }
        }

        private static void RenderLineInlines(InlineCollection inlines, string line, double baseFontSize)
        {
            // Detecta tópicos com marcadores (ex: • Fórmula: ou 1. Passo)
            int mathStartIndex = 0;

            if (line.TrimStart().StartsWith('•') || line.TrimStart().StartsWith('-'))
            {
                int bulletPos = line.AsSpan().IndexOfAny(BulletSearchChars);
                if (bulletPos > 0)
                {
                    inlines.Add(new Run(line.Substring(0, bulletPos)));
                }

                inlines.Add(new Run("• ") { Foreground = BrushBullet, FontWeight = FontWeights.Bold });
                mathStartIndex = bulletPos + 1;
                while (mathStartIndex < line.Length && (line[mathStartIndex] == ' ' || line[mathStartIndex] == '\t'))
                {
                    mathStartIndex++;
                }
            }

            // Identifica se há um cabeçalho inicial como "Fórmula:", "Teoria:", "Transformação MVP:"
            int colonIdx = line.IndexOf(':', mathStartIndex);
            if (colonIdx != -1 && colonIdx - mathStartIndex < 35)
            {
                string header = line.Substring(mathStartIndex, colonIdx - mathStartIndex + 1);
                inlines.Add(new Run(header + " ") { Foreground = BrushHeader, FontWeight = FontWeights.Bold });
                mathStartIndex = colonIdx + 1;
            }

            if (mathStartIndex >= line.Length) return;

            string mathPart = line.Substring(mathStartIndex);
            ParseMathExpression(inlines, mathPart, baseFontSize);
        }

        private static void ParseMathExpression(InlineCollection inlines, string math, double baseFontSize)
        {
            int lastIdx = 0;
            var matches = MathTokensRegex().Matches(math);

            foreach (Match match in matches)
            {
                if (match.Index > lastIdx)
                {
                    string ws = math.Substring(lastIdx, match.Index - lastIdx);
                    inlines.Add(new Run(ws) { Foreground = BrushText, FontSize = baseFontSize });
                }

                inlines.Add(CreateRunForMathToken(match, baseFontSize));
                lastIdx = match.Index + match.Length;
            }

            if (lastIdx < math.Length)
            {
                inlines.Add(new Run(math.Substring(lastIdx)) { Foreground = BrushText, FontSize = baseFontSize });
            }
        }

        private static Run CreateRunForMathToken(Match match, double baseFontSize)
        {
            if (match.Groups["Sub"].Success)
            {
                string subVal = match.Value.Substring(1);
                string converted = TryConvertSubscript(subVal);
                return !string.IsNullOrEmpty(converted)
                    ? new Run(converted) { Foreground = BrushSubSuper, FontWeight = FontWeights.SemiBold, FontSize = baseFontSize }
                    : new Run(subVal) { BaselineAlignment = BaselineAlignment.Subscript, FontSize = baseFontSize * 0.8, Foreground = BrushSubSuper, FontWeight = FontWeights.SemiBold };
            }

            if (match.Groups["Super"].Success)
            {
                string supVal = match.Value.Substring(1);
                string converted = TryConvertSuperscript(supVal);
                return !string.IsNullOrEmpty(converted)
                    ? new Run(converted) { Foreground = BrushSubSuper, FontWeight = FontWeights.Bold, FontSize = baseFontSize }
                    : new Run(supVal) { BaselineAlignment = BaselineAlignment.Superscript, FontSize = baseFontSize * 0.8, Foreground = BrushSubSuper, FontWeight = FontWeights.Bold };
            }

            if (match.Groups["Greek"].Success)
            {
                return new Run(match.Value) { Foreground = BrushMathGreek, FontWeight = FontWeights.Bold, FontSize = baseFontSize * 1.05 };
            }

            if (match.Groups["Op"].Success)
            {
                return new Run(match.Value) { Foreground = BrushMathOperator, FontWeight = FontWeights.SemiBold, FontSize = baseFontSize };
            }

            if (match.Groups["Number"].Success)
            {
                return new Run(match.Value) { Foreground = BrushMathNumber, FontSize = baseFontSize };
            }

            if (match.Groups["Ident"].Success)
            {
                return new Run(match.Value) { Foreground = BrushMathVariable, FontWeight = FontWeights.SemiBold, FontSize = baseFontSize };
            }

            return new Run(match.Value) { Foreground = BrushText, FontSize = baseFontSize };
        }

        private static string TryConvertSuperscript(string val)
        {
            var sb = new StringBuilder();
            foreach (char c in val)
            {
                if (SuperscriptMap.TryGetValue(c, out char sup))
                    sb.Append(sup);
                else
                    return string.Empty; // Usa fallback via BaselineAlignment
            }
            return sb.ToString();
        }

        private static string TryConvertSubscript(string val)
        {
            var sb = new StringBuilder();
            foreach (char c in val)
            {
                if (SubscriptMap.TryGetValue(c, out char sub))
                    sb.Append(sub);
                else
                    return string.Empty; // Usa fallback via BaselineAlignment
            }
            return sb.ToString();
        }
    }
}
