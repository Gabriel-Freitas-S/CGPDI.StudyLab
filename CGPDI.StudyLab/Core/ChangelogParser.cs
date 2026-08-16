using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CGPDI.StudyLab.Core
{
    public enum ChangelogSectionKind
    {
        Intro,
        Added,
        Changed,
        Fixed,
        Removed,
        Security,
        Other
    }

    public sealed class ChangelogEntry
    {
        public string Text { get; set; } = "";
        public bool IsBullet { get; set; }
    }

    public sealed class ChangelogBlock
    {
        public ChangelogSectionKind Kind { get; set; } = ChangelogSectionKind.Other;
        public string? Heading { get; set; }
        public List<ChangelogEntry> Entries { get; } = new();
    }

    public sealed class ChangelogInlineSegment
    {
        public string Text { get; set; } = "";
        public bool IsBold { get; set; }
        public bool IsCode { get; set; }
    }

    /// <summary>
    /// Parser leve de markdown para notas de release (cabeçalhos, listas, negrito e código inline).
    /// </summary>
    public static partial class ChangelogParser
    {
        [GeneratedRegex(@"^(#{1,3})\s+(.+)$")]
        private static partial Regex HeadingRegex();

        [GeneratedRegex(@"^\s*(?:[-*•])\s+(.+)$")]
        private static partial Regex BulletRegex();

        [GeneratedRegex(@"(\*\*[^*]+\*\*|`[^`]+`)")]
        private static partial Regex InlineRegex();

        public static List<ChangelogBlock> Parse(string markdown)
        {
            var blocks = new List<ChangelogBlock>();
            if (string.IsNullOrWhiteSpace(markdown)) return blocks;

            ChangelogBlock? current = null;
            foreach (string rawLine in markdown.Split('\n'))
            {
                string line = rawLine.TrimEnd('\r');

                Match heading = HeadingRegex().Match(line);
                if (heading.Success)
                {
                    current = new ChangelogBlock
                    {
                        Heading = heading.Groups[2].Value.Trim(),
                        Kind = ClassifyHeading(heading.Groups[2].Value)
                    };
                    blocks.Add(current);
                    continue;
                }

                Match bullet = BulletRegex().Match(line);
                if (bullet.Success)
                {
                    current ??= NewIntroBlock(blocks);
                    current.Entries.Add(new ChangelogEntry { Text = bullet.Groups[1].Value.Trim(), IsBullet = true });
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    current ??= NewIntroBlock(blocks);
                    current.Entries.Add(new ChangelogEntry { Text = line.Trim(), IsBullet = false });
                }
            }

            return blocks;
        }

        public static List<ChangelogInlineSegment> ParseInline(string text)
        {
            var segments = new List<ChangelogInlineSegment>();
            if (string.IsNullOrEmpty(text)) return segments;

            int index = 0;
            foreach (Match match in InlineRegex().Matches(text))
            {
                if (match.Index > index)
                {
                    segments.Add(new ChangelogInlineSegment { Text = text.Substring(index, match.Index - index) });
                }

                string token = match.Value;
                if (token.StartsWith("**", StringComparison.Ordinal))
                {
                    segments.Add(new ChangelogInlineSegment { Text = token.Substring(2, token.Length - 4), IsBold = true });
                }
                else
                {
                    segments.Add(new ChangelogInlineSegment { Text = token.Substring(1, token.Length - 2), IsCode = true });
                }

                index = match.Index + match.Length;
            }

            if (index < text.Length)
            {
                segments.Add(new ChangelogInlineSegment { Text = text.Substring(index) });
            }

            return segments;
        }

        private static ChangelogBlock NewIntroBlock(List<ChangelogBlock> blocks)
        {
            var block = new ChangelogBlock { Kind = ChangelogSectionKind.Intro };
            blocks.Add(block);
            return block;
        }

        private static ChangelogSectionKind ClassifyHeading(string heading)
        {
            string normalized = heading.ToLowerInvariant().Trim();

            if (normalized.StartsWith("adicionad", StringComparison.Ordinal) ||
                normalized.StartsWith("added", StringComparison.Ordinal) ||
                normalized.StartsWith("novo", StringComparison.Ordinal) ||
                normalized.StartsWith("novos", StringComparison.Ordinal)) return ChangelogSectionKind.Added;

            if (normalized.StartsWith("corrigid", StringComparison.Ordinal) ||
                normalized.StartsWith("fixed", StringComparison.Ordinal) ||
                normalized.StartsWith("bug", StringComparison.Ordinal)) return ChangelogSectionKind.Fixed;

            if (normalized.StartsWith("alterad", StringComparison.Ordinal) ||
                normalized.StartsWith("changed", StringComparison.Ordinal) ||
                normalized.StartsWith("melhoria", StringComparison.Ordinal) ||
                normalized.StartsWith("melhorad", StringComparison.Ordinal)) return ChangelogSectionKind.Changed;

            if (normalized.StartsWith("removid", StringComparison.Ordinal) ||
                normalized.StartsWith("removed", StringComparison.Ordinal)) return ChangelogSectionKind.Removed;

            if (normalized.StartsWith("seguran", StringComparison.Ordinal) ||
                normalized.StartsWith("security", StringComparison.Ordinal)) return ChangelogSectionKind.Security;

            return ChangelogSectionKind.Other;
        }
    }
}