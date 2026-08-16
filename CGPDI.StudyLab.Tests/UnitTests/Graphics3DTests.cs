using System;
using System.Numerics;
using System.Windows.Media.Media3D;
using CGPDI.StudyLab.Graphics3D;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class Graphics3DTests
    {
        [Fact]
        public void Vector3_DotProductAndNormalization_WorksCorrectly()
        {
            var v = new Vector3(3, 4, 0);
            v.Length().Should().Be(5.0f);

            var norm = Vector3.Normalize(v);
            norm.Length().Should().BeApproximately(1.0f, 0.0001f);

            var up = new Vector3(0, 1, 0);
            var dot = Vector3.Dot(v, up);
            dot.Should().Be(4.0f);
        }

        [Fact]
        public void HierarchicalRobotArm_CreationAndJointAngles_BuildsSceneTree()
        {
            var arm = new HierarchicalRobotArm();
            arm.RootNode.Should().NotBeNull();
            arm.RootNode.ModelGroup.Should().NotBeNull();

            var act = () => arm.SetJointAngles(30, 45, -30, 15);
            act.Should().NotThrow();
        }

        [Fact]
        public void SphereObject_RayIntersection_FindsClosestHit()
        {
            var sphere = new SphereObject(new Vec3(0, 0, 5), 1.0, new MaterialRay());
            var ray = new Ray3D(new Vec3(0, 0, 0), new Vec3(0, 0, 1));

            bool hit = sphere.Intersect(ray, out double t, out Vec3 normal);
            hit.Should().BeTrue();
            t.Should().Be(4.0); // Center at 5 minus radius 1
        }
    }
}
