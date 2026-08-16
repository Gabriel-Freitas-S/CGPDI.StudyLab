using System.Windows.Media;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.ImageProcessing;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class ImageProcessingTests
    {
        [Fact]
        public void DirectBitmap_AllocateAndSetPixel_ReturnsCorrectColor()
        {
            using var bmp = new DirectBitmap(64, 64);
            bmp.Width.Should().Be(64);
            bmp.Height.Should().Be(64);

            bmp.Lock();
            bmp.SetPixel(10, 10, Color.FromArgb(255, 200, 100, 50));
            Color color = bmp.GetPixel(10, 10);
            bmp.Unlock(false);

            color.R.Should().Be(200);
            color.G.Should().Be(100);
            color.B.Should().Be(50);
            color.A.Should().Be(255);
        }

        [Fact]
        public void ColorSpaces_HsvToRgb_ConvertsCorrectly()
        {
            Color red = ColorSpaces.HsvToRgb(0, 1.0, 1.0);
            red.R.Should().Be(255);
            red.G.Should().Be(0);
            red.B.Should().Be(0);

            Color green = ColorSpaces.HsvToRgb(120, 1.0, 1.0);
            green.R.Should().Be(0);
            green.G.Should().Be(255);
            green.B.Should().Be(0);
        }

        [Fact]
        public void PointAndHistograms_Brightness_IncreasesPixelIntensity()
        {
            using var src = new DirectBitmap(16, 16);
            src.Lock();
            src.Clear(Color.FromRgb(100, 100, 100));
            src.Unlock(false);

            using var dst = PointAndHistograms.AdjustBrightness(src, 50);
            dst.Lock();
            Color c = dst.GetPixel(5, 5);
            dst.Unlock(false);

            c.R.Should().Be(150);
            c.G.Should().Be(150);
            c.B.Should().Be(150);
        }

        [Fact]
        public void Morphology_OtsuThreshold_CalculatesValidThreshold()
        {
            using var src = new DirectBitmap(32, 32);
            src.Lock();
            for (int y = 0; y < 32; y++)
            {
                for (int x = 0; x < 32; x++)
                {
                    byte val = (byte)(x < 16 ? 40 : 210);
                    src.SetPixel(x, y, Color.FromRgb(val, val, val));
                }
            }
            src.Unlock(false);

            using var binarized = Morphology.OtsuThreshold(src, out byte threshold);
            threshold.Should().BeInRange(40, 210);
            binarized.Should().NotBeNull();
        }

        [Fact]
        public void SpatialFilters_BoxBlur_SmoothsImage()
        {
            using var src = new DirectBitmap(32, 32);
            src.Lock();
            src.Clear(Color.FromRgb(0, 0, 0));
            src.SetPixel(16, 16, Color.FromRgb(255, 255, 255));
            src.Unlock(false);

            using var blurred = SpatialFilters.BoxBlur(src, 3);
            blurred.Lock();
            Color center = blurred.GetPixel(16, 16);
            Color neighbor = blurred.GetPixel(15, 16);
            blurred.Unlock(false);

            center.R.Should().BeLessThan(255);
            neighbor.R.Should().BeGreaterThan(0);
        }
    }
}
