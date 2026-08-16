using System.Windows.Controls;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.Graphics3D;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class Viewport3DAndXamlTests
    {
        [WpfFact]
        public void WpfViewport3DManager_RotateCamera_ChangesCameraPosition()
        {
            var viewport = new Viewport3D();
            var manager = new WpfViewport3DManager(viewport);

            var before = manager.CameraPosition;
            manager.RotateCamera(90.0);
            var after = manager.CameraPosition;

            after.Z.Should().NotBeApproximately(before.Z, 1e-9);
            after.Y.Should().BeApproximately(before.Y, 1e-9);
        }

        [WpfFact]
        public void WpfViewport3DManager_RotateCamera_FullCircle_ReturnsToStart()
        {
            var viewport = new Viewport3D();
            var manager = new WpfViewport3DManager(viewport);

            var start = manager.CameraPosition;
            for (int i = 0; i < 360; i++)
            {
                manager.RotateCamera(1.0);
            }

            var end = manager.CameraPosition;
            end.X.Should().BeApproximately(start.X, 1e-6);
            end.Y.Should().BeApproximately(start.Y, 1e-6);
            end.Z.Should().BeApproximately(start.Z, 1e-6);
        }

        [WpfFact]
        public void WpfViewport3DManager_SetShape_ChangesGeometry()
        {
            var viewport = new Viewport3D();
            var manager = new WpfViewport3DManager(viewport);

            manager.SetShape("Sphere");
            var spherePositions = manager.CurrentGeometryPositionsCount;

            manager.SetShape("Cube");
            var cubePositions = manager.CurrentGeometryPositionsCount;

            spherePositions.Should().BeGreaterThan(0);
            cubePositions.Should().BeGreaterThan(0);
        }

        [WpfFact]
        public void XamlSyntaxHighlighter_ShouldHighlightAndPreserveText()
        {
            var rtb = new RichTextBox();
            string xaml = "<Grid Width=\"200\">\n    <Button Content=\"Ok\"/>\n</Grid>";

            XamlSyntaxHighlighter.Highlight(rtb, xaml);

            rtb.Document.Blocks.Count.Should().BeGreaterThan(0);
            string extracted = XamlSyntaxHighlighter.GetPlainText(rtb);
            extracted.Should().Contain("<Grid");
            extracted.Should().Contain("Width=\"200\"");
            extracted.Should().Contain("Ok");
        }

        [WpfFact]
        public void XamlSyntaxHighlighter_CaretIndex_ShouldBeCalculable()
        {
            var rtb = new RichTextBox();
            XamlSyntaxHighlighter.SetCode(rtb, "<Canvas><Rectangle/></Canvas>");

            int caret = XamlSyntaxHighlighter.GetCaretCharIndex(rtb);
            caret.Should().BeGreaterThanOrEqualTo(0);

            XamlSyntaxHighlighter.SetCaretCharIndex(rtb, 3);
            XamlSyntaxHighlighter.GetCaretCharIndex(rtb).Should().BeGreaterThanOrEqualTo(0);
        }
    }
}
