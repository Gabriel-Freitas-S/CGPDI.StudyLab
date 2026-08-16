using System.Windows.Media;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.Graphics3D;
using CGPDI.StudyLab.ImageProcessing;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class GeometricTransformsAndMath3DTests
    {
        [Fact]
        public void GeometricTransforms_Rotate90_KeepsSameDimensionsAndDoesNotThrow()
        {
            using var src = new DirectBitmap(32, 32);
            src.Lock();
            src.Clear(Color.FromRgb(120, 80, 40));
            src.Unlock(false);

            using var rotated = GeometricTransforms.Rotate(src, 90.0, InterpolationMode.NearestNeighbor);

            rotated.Width.Should().Be(32);
            rotated.Height.Should().Be(32);
        }

        [Fact]
        public void GeometricTransforms_Scale_CenterPixelPreservesColor()
        {
            using var src = new DirectBitmap(32, 32);
            src.Lock();
            src.Clear(Color.FromRgb(0, 0, 0));
            src.SetPixel(16, 16, Color.FromRgb(200, 50, 50));
            src.Unlock(false);

            using var scaled = GeometricTransforms.Scale(src, 2.0, 2.0, InterpolationMode.NearestNeighbor);

            scaled.Lock();
            Color center = scaled.GetPixel(16, 16);
            scaled.Unlock(false);

            center.R.Should().Be(200);
        }

        [Fact]
        public void Vec3_OperatorsAndMath_AreCorrect()
        {
            var a = new Vec3(1, 2, 3);
            var b = new Vec3(4, 5, 6);

            var sum = a + b;
            sum.X.Should().Be(5);
            sum.Y.Should().Be(7);
            sum.Z.Should().Be(9);

            double dot = Vec3.Dot(a, b);
            dot.Should().Be(32); // 1*4 + 2*5 + 3*6

            var cross = Vec3.Cross(new Vec3(1, 0, 0), new Vec3(0, 1, 0));
            cross.X.Should().BeApproximately(0, 1e-9);
            cross.Y.Should().BeApproximately(0, 1e-9);
            cross.Z.Should().BeApproximately(1, 1e-9);
        }

        [Fact]
        public void Vec3_Normalized_HasUnitLength()
        {
            var v = new Vec3(3, 4, 0);
            var n = v.Normalized;
            n.Length.Should().BeApproximately(1.0, 1e-9);
        }

        [Fact]
        public void Vec3_Reflect_WorksForPerpendicularIncidence()
        {
            var incident = new Vec3(0, -1, 0);
            var normal = new Vec3(0, 1, 0);

            var reflected = Vec3.Reflect(incident, normal);

            reflected.X.Should().BeApproximately(0, 1e-9);
            reflected.Y.Should().BeApproximately(1, 1e-9);
            reflected.Z.Should().BeApproximately(0, 1e-9);
        }
    }
}
