using System.Windows;
using System.Windows.Media;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.Graphics2D;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class Rasterization2DTests
    {
        [Fact]
        public void BresenhamLine_DrawsCorrectPixels()
        {
            using var bmp = new DirectBitmap(64, 64);
            bmp.Lock();
            bmp.Clear(Color.FromRgb(0, 0, 0));

            Color lineColor = Color.FromRgb(255, 255, 255);
            Rasterizer2D.DrawLineBresenham(bmp, 10, 10, 20, 10, lineColor);

            for (int x = 10; x <= 20; x++)
            {
                Color p = bmp.GetPixel(x, 10);
                p.R.Should().Be(255);
            }
            bmp.Unlock(false);
        }

        [Fact]
        public void MidpointCircle_DrawsSymmetricBoundary()
        {
            using var bmp = new DirectBitmap(64, 64);
            bmp.Lock();
            bmp.Clear(Color.FromRgb(0, 0, 0));

            Color col = Color.FromRgb(0, 255, 0);
            Rasterizer2D.DrawCircleMidpoint(bmp, 32, 32, 10, col);

            // Cardinal points of radius 10 around (32, 32)
            bmp.GetPixel(42, 32).G.Should().Be(255); // Right
            bmp.GetPixel(22, 32).G.Should().Be(255); // Left
            bmp.GetPixel(32, 42).G.Should().Be(255); // Bottom
            bmp.GetPixel(32, 22).G.Should().Be(255); // Top

            bmp.Unlock(false);
        }

        [Fact]
        public void Matrix3x3_TranslationAndScaling_TransformsPointCorrectly()
        {
            var p = new Point(10, 20);

            var t = Matrix3x3.CreateTranslation(5, -5);
            var translated = t.TransformPoint(p);
            translated.X.Should().Be(15);
            translated.Y.Should().Be(15);

            var s = Matrix3x3.CreateScale(2.0, 3.0);
            var scaled = s.TransformPoint(p);
            scaled.X.Should().Be(20);
            scaled.Y.Should().Be(60);
        }

        [Fact]
        public void Matrix3x3_Multiplication_MatchesCompositeTransform()
        {
            var p = new Point(10, 10);
            var t = Matrix3x3.CreateTranslation(10, 0);
            var s = Matrix3x3.CreateScale(2.0, 2.0);

            var comp = s * t; // Translate first, then scale: (10+10)*2 = 40
            var result = comp.TransformPoint(p);

            result.X.Should().Be(40);
            result.Y.Should().Be(20);
        }

        [Fact]
        public void CohenSutherland_TrivialAccept_ClipsCorrectly()
        {
            var p0 = new Point(10, 10);
            var p1 = new Point(20, 20);

            bool visible = Rasterizer2D.ClipLineCohenSutherland(new Rect(0, 0, 50, 50), ref p0, ref p1);
            visible.Should().BeTrue();
            p0.X.Should().Be(10);
            p1.X.Should().Be(20);
        }
    }
}
