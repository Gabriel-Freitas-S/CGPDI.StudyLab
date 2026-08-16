using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Motor de Syntax Highlighting de alta performance para marcação WPF / XAML / XML.
    /// </summary>
    public static class XamlSyntaxHighlighter
    {
        // Paleta Visual Studio / VS Code Dark para XAML
        private static readonly SolidColorBrush BrushDefault = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E2E8F0"));
        private static readonly SolidColorBrush BrushTag = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#569CD6"));
        private static readonly SolidColorBrush BrushAttribute = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#9CDCFE"));
        private static readonly SolidColorBrush BrushString = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CE9178"));
        private static readonly SolidColorBrush BrushComment = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#6A9955"));
        private static readonly SolidColorBrush BrushNamespace = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#DCDCAA"));

        static XamlSyntaxHighlighter()
        {
            BrushDefault.Freeze();
            BrushTag.Freeze();
            BrushAttribute.Freeze();
            BrushString.Freeze();
            BrushComment.Freeze();
            BrushNamespace.Freeze();
        }

        private static readonly Regex XamlTokenRegex = new Regex(
            @"(?<Comment><!--.*?-->)|" +
            @"(?<String>""(?:\\.|[^""\\])*"")|" +
            @"(?<TagClose></[a-zA-Z0-9_\.:]+>)|" +
            @"(?<TagOpen><[a-zA-Z0-9_\.:]+)|" +
            @"(?<TagEnd>/?>)|" +
            @"(?<Attribute>[a-zA-Z0-9_\.:]+)(?=\s*=)|" +
            @"(?<Symbol>[=])",
            RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>
        /// Aplica Syntax Highlighting de XAML no FlowDocument de um RichTextBox.
        /// </summary>
        public static void Highlight(RichTextBox rtb, string xamlCode)
        {
            var doc = new FlowDocument
            {
                PagePadding = new System.Windows.Thickness(8),
                FontFamily = new FontFamily("Consolas, Cascadia Code, Courier New, monospace"),
                FontSize = 13.0,
                LineHeight = 19.0
            };

            var paragraph = new Paragraph { Margin = new System.Windows.Thickness(0) };

            string[] lines = xamlCode.Replace("\r\n", "\n").Split('\n');

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];

                if (string.IsNullOrEmpty(line))
                {
                    paragraph.Inlines.Add(new Run("\n"));
                    continue;
                }

                int lastIndex = 0;
                var matches = XamlTokenRegex.Matches(line);

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
                    else if (match.Groups["TagOpen"].Success || match.Groups["TagClose"].Success || match.Groups["TagEnd"].Success)
                    {
                        paragraph.Inlines.Add(new Run(text) { Foreground = BrushTag, FontWeight = System.Windows.FontWeights.SemiBold });
                    }
                    else if (match.Groups["Attribute"].Success)
                    {
                        var brush = text.StartsWith("xmlns", StringComparison.OrdinalIgnoreCase) || text.StartsWith("x:", StringComparison.OrdinalIgnoreCase)
                            ? BrushNamespace
                            : BrushAttribute;
                        paragraph.Inlines.Add(new Run(text) { Foreground = brush });
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

        public static string GetPlainText(RichTextBox rtb)
        {
            var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            string text = textRange.Text;
            if (text.EndsWith("\r\n"))
            {
                text = text.Substring(0, text.Length - 2);
            }
            return text;
        }

        public static void SetCode(RichTextBox rtb, string xamlCode)
        {
            Highlight(rtb, xamlCode ?? string.Empty);
        }

        /// <summary>
        /// Obtém o deslocamento de caractere do cursor no documento.
        /// </summary>
        public static int GetCaretCharIndex(RichTextBox rtb)
        {
            var caret = rtb.CaretPosition;
            var start = rtb.Document.ContentStart;
            return Math.Max(0, start.GetOffsetToPosition(caret));
        }

        /// <summary>
        /// Restaura o cursor para a posição especificada.
        /// </summary>
        public static void SetCaretCharIndex(RichTextBox rtb, int offset)
        {
            try
            {
                var start = rtb.Document.ContentStart;
                var target = start.GetPositionAtOffset(offset);
                if (target != null)
                {
                    rtb.CaretPosition = target;
                }
                else
                {
                    rtb.CaretPosition = rtb.Document.ContentEnd;
                }
            }
            catch
            {
                // Fallback seguro
            }
        }
    }
}
