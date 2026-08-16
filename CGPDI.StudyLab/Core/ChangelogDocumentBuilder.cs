using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Converte notas de release (markdown) em um FlowDocument estilizado com a paleta escura do app.
    /// </summary>
    public static class ChangelogDocumentBuilder
    {
        private static readonly Brush AddedColor = FromHex("#4ADE80");
        private static readonly Brush FixedColor = FromHex("#FBBF24");
        private static readonly Brush ChangedColor = FromHex("#60A5FA");
        private static readonly Brush RemovedColor = FromHex("#F87171");
        private static readonly Brush SecurityColor = FromHex("#C084FC");
        private static readonly Brush HeadingColor = FromHex("#E2E8F0");
        private static readonly Brush TextColor = FromHex("#E2E8F0");
        private static readonly Brush MutedColor = FromHex("#94A3B8");
        private static readonly Brush CodeBg = FromHex("#1E293B");
        private static readonly Brush CodeFg = FromHex("#38BDF8");

        public static FlowDocument Build(string markdown)
        {
            var document = new FlowDocument
            {
                PagePadding = new Thickness(0),
                FontFamily = new FontFamily("Segoe UI, Inter, Arial, sans-serif"),
                FontSize = 12.5,
                Foreground = TextColor,
                LineHeight = 19
            };

            var blocks = ChangelogParser.Parse(markdown);
            foreach (var block in blocks)
            {
                AppendBlock(document, block);
            }

            return document;
        }

        private static void AppendBlock(FlowDocument document, ChangelogBlock block)
        {
            if (!string.IsNullOrEmpty(block.Heading))
            {
                var heading = new Paragraph
                {
                    Margin = new Thickness(0, 12, 0, 4),
                    Foreground = ColorFor(block.Kind),
                    FontSize = 13.5,
                    FontWeight = FontWeights.Bold
                };
                heading.Inlines.Add(new Run($"{(block.Kind == ChangelogSectionKind.Other ? "" : "✦ ")}{block.Heading}"));
                document.Blocks.Add(heading);
            }

            foreach (var entry in block.Entries)
            {
                var paragraph = new Paragraph
                {
                    Margin = new Thickness(entry.IsBullet ? 14 : 0, 0, 0, 3),
                    Foreground = TextColor,
                    TextIndent = 0
                };

                if (entry.IsBullet)
                {
                    paragraph.Inlines.Add(new Run("• ")
                    {
                        Foreground = ColorFor(block.Kind),
                        FontWeight = FontWeights.Bold
                    });
                }

                foreach (var segment in ChangelogParser.ParseInline(entry.Text))
                {
                    paragraph.Inlines.Add(SegmentToInline(segment));
                }

                document.Blocks.Add(paragraph);
            }
        }

        private static Inline SegmentToInline(ChangelogInlineSegment segment)
        {
            if (segment.IsCode)
            {
                return new Span(new Run(segment.Text))
                {
                    Background = CodeBg,
                    Foreground = CodeFg,
                    FontFamily = new FontFamily("Consolas, Cascadia Mono, monospace"),
                    FontSize = 11.5
                };
            }

            var run = new Run(segment.Text);
            if (segment.IsBold)
            {
                run.FontWeight = FontWeights.Bold;
                run.Foreground = FromHex("#F8FAFC");
            }

            return run;
        }

        private static Brush ColorFor(ChangelogSectionKind kind) => kind switch
        {
            ChangelogSectionKind.Added => AddedColor,
            ChangelogSectionKind.Fixed => FixedColor,
            ChangelogSectionKind.Changed => ChangedColor,
            ChangelogSectionKind.Removed => RemovedColor,
            ChangelogSectionKind.Security => SecurityColor,
            ChangelogSectionKind.Other => HeadingColor,
            _ => MutedColor
        };

        private static Brush FromHex(string hex)
        {
            Color color = (Color)ColorConverter.ConvertFromString(hex);
            return new SolidColorBrush(color);
        }
    }
}