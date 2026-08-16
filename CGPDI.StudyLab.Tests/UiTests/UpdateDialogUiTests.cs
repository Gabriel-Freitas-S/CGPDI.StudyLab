using System;
using System.Windows;
using System.Windows.Documents;
using CGPDI.StudyLab.Core;
using CGPDI.StudyLab.Views;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UiTests
{
    public class UpdateDialogUiTests
    {
        private static ReleaseInfo CreateRelease(string notes = "### Adicionado\n- Nova funcionalidade.")
        {
            return new ReleaseInfo
            {
                TagName = "v2.0.0",
                Version = new Version(2, 0, 0),
                Name = "CGPDI StudyLab v2.0.0",
                ReleaseNotes = notes,
                PublishedAt = "2026-08-16T17:08:03Z",
                HtmlUrl = "https://github.com/Gabriel-Freitas-S/CGPDI.StudyLab/releases",
                SetupSizeBytes = 45 * 1024 * 1024,
                PortableSizeBytes = 30 * 1024 * 1024
            };
        }

        [UIFact]
        public void Dialog_ShowsVersionCompareAndFormattedChangelog()
        {
            var dialog = new UpdateDialogWindow(CreateRelease());

            dialog.TxtVersionCompare.Text.Should().Contain("v2.0.0");
            dialog.ChangelogViewer.Document.Should().NotBeNull();
            dialog.ChangelogViewer.Document.Blocks.Count.Should().BeGreaterThan(0);
            dialog.ChangelogViewer.Document.Blocks.FirstBlock.Should().BeOfType<Paragraph>();

            dialog.Close();
        }

        [UIFact]
        public void Dialog_RendersEachMarkdownSectionAsParagraph()
        {
            const string notes = """
                ### Adicionado
                - Novo estúdio 3D.

                ### Corrigido
                - Crash ao abrir o laboratório.
                """;
            var dialog = new UpdateDialogWindow(CreateRelease(notes));

            int paragraphCount = 0;
            foreach (var block in dialog.ChangelogViewer.Document.Blocks)
            {
                if (block is Paragraph) paragraphCount++;
            }

            paragraphCount.Should().Be(4); // 2 títulos + 2 bullets
            dialog.Close();
        }

        [UIFact]
        public void Dialog_ShowsAssetSizesInRadioButtons()
        {
            var dialog = new UpdateDialogWindow(CreateRelease());

            dialog.RbInstaller.Content.ToString().Should().Contain("MB");
            dialog.RbPortable.Content.ToString().Should().Contain("MB");

            dialog.Close();
        }

        [UIFact]
        public void Dialog_ShowsFallbackMessage_WhenReleaseNotesEmpty()
        {
            var dialog = new UpdateDialogWindow(CreateRelease(""));

            dialog.ChangelogViewer.Document.Should().NotBeNull();
            dialog.ChangelogViewer.Document.Blocks.Count.Should().BeGreaterThan(0);

            dialog.Close();
        }

        [UIFact]
        public void Dialog_VelopackMode_ShowsDeltaInfoAndHidesFormatOptions()
        {
            var release = CreateRelease();
            release.IsVelopack = true;
            release.DeltaSizeBytes = 3_500_000;

            var dialog = new UpdateDialogWindow(release);

            dialog.TxtDeltaInfo.Visibility.Should().Be(Visibility.Visible);
            dialog.TxtDeltaInfo.Text.Should().Contain("delta");
            dialog.TxtDeltaInfo.Text.Should().Contain("MB");
            dialog.PanelFormat.Visibility.Should().Be(Visibility.Collapsed);

            dialog.Close();
        }

        [UIFact]
        public void Dialog_NonVelopackMode_KeepsFormatOptionsVisible()
        {
            var dialog = new UpdateDialogWindow(CreateRelease());

            dialog.PanelFormat.Visibility.Should().Be(Visibility.Visible);
            dialog.TxtDeltaInfo.Visibility.Should().Be(Visibility.Collapsed);

            dialog.Close();
        }

        [UIFact]
        public void Dialog_ShowsEnvironmentBadge()
        {
            var dialog = new UpdateDialogWindow(CreateRelease());

            dialog.BadgeEnvironment.Visibility.Should().Be(Visibility.Visible);
            dialog.TxtEnvironmentBadge.Text.Should().NotBeNullOrWhiteSpace();

            dialog.Close();
        }

        [UIFact]
        public void Dialog_Cancel_SnoozesUpdate()
        {
            var settings = new UpdateSettings();
            var dialog = new UpdateDialogWindow(CreateRelease(), settings);
            dialog.BtnCancel.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            settings.SnoozedVersion.Should().Be("2.0.0");
        }

        [UIFact]
        public void Dialog_Skip_SkipsUpdateVersion()
        {
            var settings = new UpdateSettings();
            var dialog = new UpdateDialogWindow(CreateRelease(), settings);
            dialog.BtnSkip.RaiseEvent(new RoutedEventArgs(System.Windows.Controls.Primitives.ButtonBase.ClickEvent));
            settings.SkippedVersions.Should().Contain("2.0.0");
        }
    }
}