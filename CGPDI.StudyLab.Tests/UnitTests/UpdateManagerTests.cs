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

        [Fact]
        public void IsVelopackInstalled_ReturnsFalse_WhenRunningUninstalled()
        {
            // Em ambiente de testes o app não está instalado via Velopack.
            UpdateManager.IsVelopackInstalled.Should().BeFalse();
        }

        [Fact]
        public void IsMachineWideInstall_ReturnsFalse_WhenRunningFromDevFolder()
        {
            // O diretório de testes não fica sob Program Files.
            UpdateManager.IsMachineWideInstall.Should().BeFalse();
        }

        [Fact]
        public void IsElevated_ReturnsBooleanWithoutThrowing()
        {
            Action get = () => { bool _ = UpdateManager.IsElevated; };
            get.Should().NotThrow();
        }

        [Fact]
        public void EnsureSystemUpdateTask_ReturnsFalse_WhenRunningUninstalled()
        {
            // Em desenvolvimento (não machine-wide) a tarefa SYSTEM não deve ser criada.
            UpdateManager.EnsureSystemUpdateTask().Should().BeFalse();
        }

        [Fact]
        public void TryTriggerSystemUpdateTask_ReturnsFalse_WhenTaskDoesNotExist()
        {
            // Sem a tarefa criada na instalação (caso de testes), o disparo falha
            // e o fluxo segue para o reinício elevado.
            UpdateManager.TryTriggerSystemUpdateTask().Should().BeFalse();
        }

        [Fact]
        public async Task ApplyPendingUpdateSilentlyAsync_ReturnsFalse_WhenNotInstalled()
        {
            bool applied = await UpdateManager.ApplyPendingUpdateSilentlyAsync();
            applied.Should().BeFalse();
        }

        [Fact]
        public void GetLocalAppDirectory_ReturnsNonEmptyPathUnderLocalAppData()
        {
            string localDir = UpdateManager.GetLocalAppDirectory();
            localDir.Should().NotBeNullOrWhiteSpace();
            localDir.Should().Contain("CGPDI.StudyLab");
            localDir.Should().Contain("app");
        }

        [Fact]
        public void IsDirectoryWritable_ReturnsTrue_ForTempDirectory()
        {
            string tempDir = System.IO.Path.GetTempPath();
            UpdateManager.IsDirectoryWritable(tempDir).Should().BeTrue();
        }
    }
}
