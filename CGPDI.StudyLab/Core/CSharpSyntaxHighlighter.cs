using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Formatador e motor de Syntax Highlighting de alta performance para código C# em temas escuros.
    /// </summary>
    public static class CSharpSyntaxHighlighter
    {
        // Paleta Visual Studio / VS Code Dark Moderna
        private static readonly SolidColorBrush BrushDefault = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0"));
        private static readonly SolidColorBrush BrushKeyword = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#569CD6"));
        private static readonly SolidColorBrush BrushType = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4EC9B0"));
        private static readonly SolidColorBrush BrushMethod = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCDCAA"));
        private static readonly SolidColorBrush BrushString = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CE9178"));
        private static readonly SolidColorBrush BrushNumber = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B5CEA8"));
        private static readonly SolidColorBrush BrushComment = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A9955"));
        private static readonly SolidColorBrush BrushDirective = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9B9B9B"));

        static CSharpSyntaxHighlighter()
        {
            BrushDefault.Freeze();
            BrushKeyword.Freeze();
            BrushType.Freeze();
            BrushMethod.Freeze();
            BrushString.Freeze();
            BrushNumber.Freeze();
            BrushComment.Freeze();
            BrushDirective.Freeze();
        }

        private static readonly HashSet<string> Keywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked",
            "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else",
            "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for",
            "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
            "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override",
            "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte",
            "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch",
            "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
            "ushort", "using", "virtual", "void", "volatile", "while", "var", "async", "await",
            "get", "set", "record", "init", "value", "nint", "nuint"
        };

        private static readonly HashSet<string> KnownTypes = new HashSet<string>(StringComparer.Ordinal)
        {
            "DirectBitmap", "ColorSpaces", "Vector3", "Matrix3x3", "Raster2D", "Rasterizer2D", "Math", "MathF",
            "List", "Dictionary", "HashSet", "IEnumerable", "Task", "Stopwatch", "Point", "Size", "Rect",
            "Color", "BitmapSource", "IntPtr", "Span", "ReadOnlySpan", "Memory", "StringBuilder",
            "Parallel", "Random", "Convert", "Console", "Debug", "Action", "Func", "Predicate",
            "MeshGeometry3D", "Viewport3D", "PerspectiveCamera", "DirectionalLight", "MaterialGroup",
            "DiffuseMaterial", "SpecularMaterial", "Raytracer3D", "SphereObject", "Ray3D", "Vec3"
        };

        private static readonly Regex TokenRegex = new Regex(
            @"(?<Comment>//.*?$|/\*.*?\*/)|" +
            @"(?<String>@""(?:""""|[^""])*""|\$""(?:\\.|[^""\\])*""|""(?:\\.|[^""\\])*""|'\\.'|'[^'\\]')|" +
            @"(?<Directive>^\s*#\w+.*?$)|" +
            @"(?<Number>\b0x[0-9a-fA-F]+\b|\b\d+(?:\.\d+)?(?:f|d|m|u|l|ul)?\b)|" +
            @"(?<Identifier>[a-zA-Z_]\w*)|" +
            @"(?<Symbol>[^\s\w])",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Aplica Syntax Highlighting completo gerando blocos formatados no FlowDocument de um RichTextBox.
        /// </summary>
        public static void Highlight(RichTextBox rtb, string code)
        {
            if (rtb == null) return;

            var doc = new FlowDocument
            {
                PagePadding = new System.Windows.Thickness(8),
                FontFamily = new FontFamily("Consolas, Cascadia Code, Courier New, monospace"),
                FontSize = 13.0,
                LineHeight = 19.0
            };

            var paragraph = new Paragraph { Margin = new System.Windows.Thickness(0) };

            string[] lines = (code ?? string.Empty).Replace("\r\n", "\n").Split('\n');

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];

                if (string.IsNullOrEmpty(line))
                {
                    paragraph.Inlines.Add(new Run("\n"));
                    continue;
                }

                int lastIndex = 0;
                var matches = TokenRegex.Matches(line);

                foreach (Match match in matches)
                {
                    if (match.Index > lastIndex)
                    {
                        string ws = line.Substring(lastIndex, match.Index - lastIndex);
                        paragraph.Inlines.Add(new Run(ws) { Foreground = BrushDefault });
                    }

                    string text = match.Value;

                    if (match.Groups["Comment"].Success)
                    {
                        paragraph.Inlines.Add(new Run(text) { Foreground = BrushComment });
                    }
                    else if (match.Groups["String"].Success)
                    {
                        paragraph.Inlines.Add(new Run(text) { Foreground = BrushString });
                    }
                    else if (match.Groups["Directive"].Success)
                    {
                        paragraph.Inlines.Add(new Run(text) { Foreground = BrushDirective });
                    }
                    else if (match.Groups["Number"].Success)
                    {
                        paragraph.Inlines.Add(new Run(text) { Foreground = BrushNumber });
                    }
                    else if (match.Groups["Identifier"].Success)
                    {
                        if (Keywords.Contains(text))
                        {
                            paragraph.Inlines.Add(new Run(text) { Foreground = BrushKeyword, FontWeight = System.Windows.FontWeights.SemiBold });
                        }
                        else if (KnownTypes.Contains(text) || (text.Length > 0 && char.IsUpper(text[0])))
                        {
                            paragraph.Inlines.Add(new Run(text) { Foreground = BrushType, FontWeight = System.Windows.FontWeights.SemiBold });
                        }
                        else
                        {
                            int nextPos = match.Index + match.Length;
                            bool isMethod = nextPos < line.Length && line[nextPos] == '(';
                            paragraph.Inlines.Add(new Run(text) { Foreground = isMethod ? BrushMethod : BrushDefault });
                        }
                    }
                    else
                    {
                        paragraph.Inlines.Add(new Run(text) { Foreground = BrushDefault });
                    }

                    lastIndex = match.Index + match.Length;
                }

                if (lastIndex < line.Length)
                {
                    paragraph.Inlines.Add(new Run(line.Substring(lastIndex)) { Foreground = BrushDefault });
                }

                if (lineIndex < lines.Length - 1)
                {
                    paragraph.Inlines.Add(new Run("\n"));
                }
            }

            doc.Blocks.Add(paragraph);
            rtb.Document = doc;
        }

        /// <summary>
        /// Obtém o texto plano de um RichTextBox.
        /// </summary>
        public static string GetPlainText(RichTextBox rtb)
        {
            if (rtb?.Document == null) return string.Empty;
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            string text = textRange.Text;
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        /// <summary>
        /// Define o texto plano e aplica o Syntax Highlighting.
        /// </summary>
        public static void SetCode(RichTextBox rtb, string code)
        {
            Highlight(rtb, code ?? string.Empty);
        }

        /// <summary>
        /// Obtém o deslocamento de caractere do cursor no documento de texto plano.
        /// </summary>
        public static int GetCaretCharIndex(RichTextBox rtb)
        {
            if (rtb?.Document == null || rtb.CaretPosition == null) return 0;
            try
            {
                var start = rtb.Document.ContentStart;
                var caret = rtb.CaretPosition;
                var range = new TextRange(start, caret);
                return range.Text.Length;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Restaura o cursor para a posição de caractere especificada no texto.
        /// </summary>
        public static void SetCaretCharIndex(RichTextBox rtb, int charIndex)
        {
            if (rtb?.Document == null) return;
            try
            {
                var navigator = rtb.Document.ContentStart;
                int count = 0;
                while (navigator != null && count < charIndex)
                {
                    if (navigator.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        string textRun = navigator.GetTextInRun(LogicalDirection.Forward);
                        if (count + textRun.Length >= charIndex)
                        {
                            int offsetInRun = charIndex - count;
                            var pos = navigator.GetPositionAtOffset(offsetInRun);
                            if (pos != null)
                            {
                                rtb.CaretPosition = pos;
                                return;
                            }
                        }
                        count += textRun.Length;
                    }
                    navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
                }
                rtb.CaretPosition = rtb.Document.ContentEnd;
            }
            catch
            {
                // Fallback seguro
            }
        }
    }
}
