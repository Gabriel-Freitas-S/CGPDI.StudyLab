using System;
using System.Diagnostics;
using System.Reflection;
using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class UpdateManagerTests
    {
        [Fact]
        public void CurrentVersion_MatchesAssemblyFileVersion()
        {
            string fileVersion = FileVersionInfo.GetVersionInfo(typeof(UpdateManager).Assembly.Location).FileVersion ?? "1.0.0.0";
            Version expected = Version.Parse(fileVersion);

            UpdateManager.CurrentVersion.Should().Be(expected);
        }

        [Fact]
        public void CurrentVersion_IsValidAndNotLowerThanOne()
        {
            UpdateManager.CurrentVersion.Should().BeGreaterThanOrEqualTo(new Version(1, 0, 0));
            UpdateManager.CurrentVersionString.Should().NotBeNullOrWhiteSpace();
        }
    }
}
