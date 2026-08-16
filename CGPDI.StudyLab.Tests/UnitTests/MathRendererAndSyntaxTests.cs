using System.Windows.Controls;
using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class MathRendererAndSyntaxTests
    {
        [WpfFact]
        public void MathFormulaRenderer_ShouldRenderFormulasIntoInlines()
        {
            var textBlock = new TextBlock();
            string formula = @"v_{clip} = M_{proj} \times M_{view} \times M_{model} \times v_{local}
I = I_a·k_a + I_d·k_d·(N·L) + I_s·k_s·(N·H)^\alpha";

            MathFormulaRenderer.RenderToTextBlock(textBlock, formula);

            textBlock.Inlines.Count.Should().BeGreaterThan(5);
        }

        [WpfFact]
        public void CSharpSyntaxHighlighter_ShouldHighlightKeywordsAndTypes()
        {
            var rtb = new RichTextBox();
            string code = @"public static unsafe DirectBitmap ConvertToGrayscale(DirectBitmap src)
{
    int x = 42;
    // Comentário de teste
    string label = ""PDI Test"";
    return src;
}";

            CSharpSyntaxHighlighter.Highlight(rtb, code);

            rtb.Document.Blocks.Count.Should().BeGreaterThan(0);
            string extracted = CSharpSyntaxHighlighter.GetPlainText(rtb);
            extracted.Should().Contain("ConvertToGrayscale");
            extracted.Should().Contain("DirectBitmap");
        }

        [WpfFact]
        public void CSharpSyntaxHighlighter_CaretIndex_ShouldBeCalculable()
        {
            var rtb = new RichTextBox();
            string code = "public void Test() { }";
            CSharpSyntaxHighlighter.SetCode(rtb, code);

            int caretIndex = CSharpSyntaxHighlighter.GetCaretCharIndex(rtb);
            caretIndex.Should().BeGreaterThanOrEqualTo(0);

            CSharpSyntaxHighlighter.SetCaretCharIndex(rtb, 5);
            int newCaret = CSharpSyntaxHighlighter.GetCaretCharIndex(rtb);
            newCaret.Should().BeGreaterThanOrEqualTo(0);
        }

        [WpfFact]
        public void XamlSyntaxHighlighter_ShouldHighlightXamlMarkup()
        {
            var rtb = new RichTextBox();
            string xaml = @"<Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
    <!-- Test comment -->
    <Button Content=""Click me"" Width=""100"" />
</Grid>";

            XamlSyntaxHighlighter.Highlight(rtb, xaml);

            rtb.Document.Blocks.Count.Should().BeGreaterThan(0);
            string extracted = XamlSyntaxHighlighter.GetPlainText(rtb);
            extracted.Should().Contain("Button");
            extracted.Should().Contain("Click me");

            int caret = XamlSyntaxHighlighter.GetCaretCharIndex(rtb);
            caret.Should().BeGreaterThanOrEqualTo(0);
            XamlSyntaxHighlighter.SetCaretCharIndex(rtb, 4);
            XamlSyntaxHighlighter.GetCaretCharIndex(rtb).Should().BeGreaterThanOrEqualTo(0);
        }

        [WpfFact]
        public void ChangelogDocumentBuilder_ShouldBuildStyledFlowDocument()
        {
            string md = @"## [v1.5.0] - 2026-08-16
### Adicionado
- Nova ferramenta de **Transformações 3D** com suporte a `Viewport3D`.
### Corrigido
- Correção de cálculo em `ColorSpaces.HsvToRgb`.";

            var doc = ChangelogDocumentBuilder.Build(md);
            doc.Should().NotBeNull();
            doc.Blocks.Count.Should().BeGreaterThan(0);
        }

        [Fact]
        public void AlgorithmCodeSnippets_ShouldContainValidCSharpSnippets()
        {
            AlgorithmCodeSnippets.GrayscaleCode.Should().Contain("ConvertToGrayscale");
            AlgorithmCodeSnippets.SobelCode.Should().Contain("ApplySobel");
            AlgorithmCodeSnippets.GaussianCode.Should().Contain("GaussianBlur");
            AlgorithmCodeSnippets.OtsuCode.Should().Contain("OtsuThreshold");
            AlgorithmCodeSnippets.BresenhamLineCode.Should().Contain("DrawLineBresenham");
            AlgorithmCodeSnippets.MidpointCircleCode.Should().Contain("DrawCircleMidpoint");
            AlgorithmCodeSnippets.Matrix3x3Code.Should().Contain("Matrix3x3");
            AlgorithmCodeSnippets.Pipeline3DMVPCode.Should().Contain("TransformVertex");
            AlgorithmCodeSnippets.BlinnPhongCode.Should().Contain("CalculateBlinnPhong");
            AlgorithmCodeSnippets.RayTracingSphereCode.Should().Contain("Intersect");
        }
    }
}
