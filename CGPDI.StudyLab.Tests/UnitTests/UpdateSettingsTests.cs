using System;
using System.IO;
using CGPDI.StudyLab.Core;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    public class UpdateSettingsTests
    {
        [Fact]
        public void ShouldNotifyFor_ReturnsTrue_WhenVersionNotHandled()
        {
            var settings = new UpdateSettings();

            settings.ShouldNotifyFor(new Version(2, 0, 0)).Should().BeTrue();
        }

        [Fact]
        public void ShouldNotifyFor_ReturnsFalse_WhenVersionSkipped()
        {
            var settings = new UpdateSettings();
            settings.Skip(new Version(1, 5, 0));

            settings.ShouldNotifyFor(new Version(1, 5, 0)).Should().BeFalse();
            settings.ShouldNotifyFor(new Version(1, 5, 1)).Should().BeTrue();
        }

        [Fact]
        public void ShouldNotifyFor_ReturnsFalse_WhileSnoozed()
        {
            var settings = new UpdateSettings();
            settings.Snooze(new Version(1, 5, 0), TimeSpan.FromDays(7));

            settings.ShouldNotifyFor(new Version(1, 5, 0)).Should().BeFalse();
        }

        [Fact]
        public void ShouldNotifyFor_ReturnsTrue_AfterSnoozeExpires()
        {
            var settings = new UpdateSettings
            {
                SnoozedVersion = "1.5.0",
                SnoozeUntilUtc = DateTimeOffset.UtcNow.AddDays(-1)
            };

            settings.ShouldNotifyFor(new Version(1, 5, 0)).Should().BeTrue();
        }

        [Fact]
        public void Skip_ClearsSnoozeForSameVersion()
        {
            var settings = new UpdateSettings();
            settings.Snooze(new Version(1, 5, 0), TimeSpan.FromDays(7));

            settings.Skip(new Version(1, 5, 0));

            settings.SnoozedVersion.Should().BeNull();
            settings.SnoozeUntilUtc.Should().BeNull();
            settings.SkippedVersions.Should().Contain("1.5.0");
        }

        [Fact]
        public void Store_SaveAndLoad_RoundTripsSettings()
        {
            string file = Path.Combine(Path.GetTempPath(), $"upd-settings-{Guid.NewGuid():N}.json");
            try
            {
                var settings = new UpdateSettings();
                settings.Skip(new Version(1, 5, 0));
                settings.Snooze(new Version(1, 6, 0), TimeSpan.FromDays(2));

                UpdateSettingsStore.Save(settings, file);
                var loaded = UpdateSettingsStore.Load(file);

                loaded.Should().NotBeNull();
                loaded.SkippedVersions.Should().Contain("1.5.0");
                loaded.SnoozedVersion.Should().Be("1.6.0");
                loaded.SnoozeUntilUtc.Should().NotBeNull();
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }

        [Fact]
        public void Store_Load_ReturnsDefaults_WhenFileMissing()
        {
            string file = Path.Combine(Path.GetTempPath(), $"upd-settings-missing-{Guid.NewGuid():N}.json");

            var loaded = UpdateSettingsStore.Load(file);

            loaded.Should().NotBeNull();
            loaded.SkippedVersions.Should().BeEmpty();
            loaded.SnoozedVersion.Should().BeNull();
        }

        [Fact]
        public void Store_Load_ReturnsDefaults_WhenFileCorrupt()
        {
            string file = Path.Combine(Path.GetTempPath(), $"upd-settings-corrupt-{Guid.NewGuid():N}.json");
            try
            {
                File.WriteAllText(file, "{ not valid json !!!");

                var loaded = UpdateSettingsStore.Load(file);

                loaded.Should().NotBeNull();
                loaded.SkippedVersions.Should().BeEmpty();
            }
            finally
            {
                if (File.Exists(file)) File.Delete(file);
            }
        }

        [Fact]
        public void NormalizeVersion_IgnoresBuildNumber()
        {
            UpdateSettings.NormalizeVersion(new Version(1, 5, 0)).Should().Be("1.5.0");
            UpdateSettings.NormalizeVersion(new Version(1, 5, 0, 123)).Should().Be("1.5.0");
        }
    }
}