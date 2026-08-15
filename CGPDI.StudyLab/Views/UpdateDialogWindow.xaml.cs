using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CGPDI.StudyLab.Core;

namespace CGPDI.StudyLab.Views
{
    public partial class UpdateDialogWindow : Window
    {
        private readonly ReleaseInfo _release;
        private CancellationTokenSource? _cts;

        public UpdateDialogWindow(ReleaseInfo release)
        {
            InitializeComponent();
            Icon = AppIconHelper.GetAppIcon();
            _release = release;

            TxtVersionCompare.Text = $"v{UpdateManager.CurrentVersionString} ➔ {release.TagName}";
            TxtReleaseDate.Text = !string.IsNullOrEmpty(release.PublishedAt)
                ? $"Publicado em: {release.PublishedAt.Replace("T", " às ").Replace("Z", "")}"
                : "Nova versão publicada no GitHub Releases";

            TxtChangelog.Text = !string.IsNullOrEmpty(release.ReleaseNotes)
                ? release.ReleaseNotes
                : $"{release.Name}\n\nAtualização recomendada para todos os usuários.";
        }

        private void BtnWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(_release.HtmlUrl) { UseShellExecute = true });
            }
            catch { }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            Close();
        }

        private async void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            BtnApply.IsEnabled = false;
            BtnCancel.IsEnabled = false;
            BtnWeb.IsEnabled = false;
            RbInstaller.IsEnabled = false;
            RbPortable.IsEnabled = false;

            PanelProgress.Visibility = Visibility.Visible;
            _cts = new CancellationTokenSource();

            var progress = new Progress<double>(percent =>
            {
                PbDownload.Value = percent;
                TxtProgressPercent.Text = $"{percent:F0}%";
                TxtProgressStatus.Text = percent < 100
                    ? $"Baixando atualização... ({percent:F0}%)"
                    : "Finalizando download e preparando instalação...";
            });

            try
            {
                bool preferInstaller = RbInstaller.IsChecked == true;
                await Task.Run(() => UpdateManager.DownloadAndApplyUpdateAsync(_release, preferInstaller, progress, _cts.Token));
            }
            catch (Exception ex)
            {
                PanelProgress.Visibility = Visibility.Collapsed;
                BtnApply.IsEnabled = true;
                BtnCancel.IsEnabled = true;
                BtnWeb.IsEnabled = true;
                RbInstaller.IsEnabled = true;
                RbPortable.IsEnabled = true;

                MessageBox.Show($"Não foi possível concluir o download da atualização:\n{ex.Message}\n\nVocê pode baixar manualmente pelo GitHub.",
                    "Erro na Atualização", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
