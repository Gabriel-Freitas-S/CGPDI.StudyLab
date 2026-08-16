using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class ChangelogParserTests
    {
        [Fact]
        public void Parse_ClassifiesSectionsByHeading()
        {
            const string markdown = """
                ## [v2.0.0] - 2026-08-16

                ### Adicionado
                - Novo estúdio 3D.

                ### Corrigido
                - Crash ao abrir o laboratório.
                """;

            var blocks = ChangelogParser.Parse(markdown);

            blocks.Should().HaveCount(3);
            blocks[0].Heading.Should().Be("[v2.0.0] - 2026-08-16");
            blocks[0].Kind.Should().Be(ChangelogSectionKind.Other);
            blocks[1].Kind.Should().Be(ChangelogSectionKind.Added);
            blocks[2].Kind.Should().Be(ChangelogSectionKind.Fixed);
        }

        [Fact]
        public void Parse_CollectsBulletsIntoSection()
        {
            const string markdown = """
                ### Corrigido
                - Erro A.
                - Erro B.
                """;

            var blocks = ChangelogParser.Parse(markdown);

            blocks.Should().HaveCount(1);
            blocks[0].Entries.Should().HaveCount(2);
            blocks[0].Entries[0].Text.Should().Be("Erro A.");
            blocks[0].Entries[0].IsBullet.Should().BeTrue();
        }

        [Fact]
        public void Parse_PlainTextGoesToIntroBlock()
        {
            var blocks = ChangelogParser.Parse("Atualização recomendada para todos os usuários.");

            blocks.Should().HaveCount(1);
            blocks[0].Kind.Should().Be(ChangelogSectionKind.Intro);
            blocks[0].Entries[0].IsBullet.Should().BeFalse();
        }

        [Fact]
        public void Parse_ReturnsEmpty_WhenMarkdownNullOrBlank()
        {
            ChangelogParser.Parse("").Should().BeEmpty();
            ChangelogParser.Parse("   ").Should().BeEmpty();
            ChangelogParser.Parse(null!).Should().BeEmpty();
        }

        [Theory]
        [InlineData("Adicionado", ChangelogSectionKind.Added)]
        [InlineData("Adicionados", ChangelogSectionKind.Added)]
        [InlineData("Added", ChangelogSectionKind.Added)]
        [InlineData("Corrigido", ChangelogSectionKind.Fixed)]
        [InlineData("Fixed", ChangelogSectionKind.Fixed)]
        [InlineData("Alterado", ChangelogSectionKind.Changed)]
        [InlineData("Melhorias", ChangelogSectionKind.Changed)]
        [InlineData("Removido", ChangelogSectionKind.Removed)]
        [InlineData("Segurança", ChangelogSectionKind.Security)]
        [InlineData("Outra Coisa", ChangelogSectionKind.Other)]
        public void Parse_ClassifiesHeadingVariants(string heading, ChangelogSectionKind expected)
        {
            var blocks = ChangelogParser.Parse($"### {heading}\n- item");

            blocks[0].Kind.Should().Be(expected);
        }

        [Fact]
        public void ParseInline_SplitsBoldAndCode()
        {
            var segments = ChangelogParser.ParseInline("Veja **novo modo** e `UpdateManager`.");

            segments.Should().HaveCount(5);
            segments[0].Text.Should().Be("Veja ");
            segments[1].Text.Should().Be("novo modo");
            segments[1].IsBold.Should().BeTrue();
            segments[2].Text.Should().Be(" e ");
            segments[3].Text.Should().Be("UpdateManager");
            segments[3].IsCode.Should().BeTrue();
            segments[4].Text.Should().Be(".");
        }

        [Fact]
        public void ParseInline_ReturnsWholeText_WhenNoFormatting()
        {
            var segments = ChangelogParser.ParseInline("texto simples");

            segments.Should().HaveCount(1);
            segments[0].Text.Should().Be("texto simples");
        }
    }
}