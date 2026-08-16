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
        private readonly UpdateSettings? _settings;
        private CancellationTokenSource? _cts;
        private static readonly TimeSpan SnoozeDuration = TimeSpan.FromDays(7);

        public UpdateDialogWindow(ReleaseInfo release, UpdateSettings? settings = null)
        {
            InitializeComponent();
            Icon = AppIconHelper.GetAppIcon();
            _release = release;
            _settings = settings;

            TxtVersionCompare.Text = $"v{UpdateManager.CurrentVersionString} ➔ {release.TagName}";
            TxtReleaseDate.Text = !string.IsNullOrEmpty(release.PublishedAt)
                ? $"Publicado em: {FormatPublishedAt(release.PublishedAt)}"
                : "Nova versão publicada no GitHub Releases";

            ChangelogViewer.Document = !string.IsNullOrEmpty(release.ReleaseNotes)
                ? ChangelogDocumentBuilder.Build(release.ReleaseNotes)
                : ChangelogDocumentBuilder.Build($"{release.Name}\n\nAtualização recomendada para todos os usuários.");

            if (release.IsVelopack)
            {
                // Velopack: download delta automático, sem escolha de formato
                PanelFormat.Visibility = Visibility.Collapsed;
                TxtDeltaInfo.Visibility = Visibility.Visible;
                if (release.DeltaSizeBytes > 0)
                {
                    TxtDeltaInfo.Text = $"Atualização delta: baixa apenas as alterações (~{FormatSize(release.DeltaSizeBytes)}).";
                }
            }
            else
            {
                if (release.SetupSizeBytes > 0)
                {
                    RbInstaller.Content = $"Instalador Automático (.exe) — {FormatSize(release.SetupSizeBytes)}";
                }

                if (release.PortableSizeBytes > 0)
                {
                    RbPortable.Content = $"Versão Portátil (.zip) — {FormatSize(release.PortableSizeBytes)}";
                }
            }

            // Exibe o badge informativo do ambiente de instalação
            BadgeEnvironment.Visibility = Visibility.Visible;
            if (UpdateManager.IsMachineWideInstall)
            {
                TxtEnvironmentBadge.Text = "Instalação da TI (Zero-Admin)";
            }
            else if (UpdateManager.IsVelopackInstalled)
            {
                TxtEnvironmentBadge.Text = "Instalação por Usuário (Zero-Admin)";
            }
            else
            {
                TxtEnvironmentBadge.Text = "Modo Portátil";
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            base.OnClosed(e);
        }

        private void BtnWeb_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(_release.HtmlUrl) { UseShellExecute = true });
            }
            catch (Exception)
            {
                // Ignorar erro ao abrir navegador externo
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            Snooze();
            Close();
        }

        private void BtnSkip_Click(object sender, RoutedEventArgs e)
        {
            _cts?.Cancel();
            if (_settings != null)
            {
                _settings.Skip(_release.Version);
                UpdateSettingsStore.Save(_settings);
            }

            Close();
        }

        private void Snooze()
        {
            if (_settings != null)
            {
                _settings.Snooze(_release.Version, SnoozeDuration);
                UpdateSettingsStore.Save(_settings);
            }
        }

        private async void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            BtnApply.IsEnabled = false;
            BtnCancel.IsEnabled = false;
            BtnSkip.IsEnabled = false;
            BtnWeb.IsEnabled = false;
            RbInstaller.IsEnabled = false;
            RbPortable.IsEnabled = false;

            PanelProgress.Visibility = Visibility.Visible;
            _cts?.Dispose();
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
                CancellationToken token = _cts.Token;
                await Task.Run(() => UpdateManager.DownloadAndApplyUpdateAsync(_release, preferInstaller, progress, token), token);
            }
            catch (Exception ex)
            {
                PanelProgress.Visibility = Visibility.Collapsed;
                BtnApply.IsEnabled = true;
                BtnCancel.IsEnabled = true;
                BtnSkip.IsEnabled = true;
                BtnWeb.IsEnabled = true;
                RbInstaller.IsEnabled = true;
                RbPortable.IsEnabled = true;

                MessageBox.Show($"Não foi possível concluir o download da atualização:\n{ex.Message}\n\nVocê pode baixar manualmente pelo GitHub.",
                    "Erro na Atualização", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private static string FormatPublishedAt(string publishedAt)
        {
            string s = publishedAt;
            int idx = s.IndexOf('T');
            if (idx >= 0)
            {
                string date = s.Substring(0, idx);
                string time = s.Substring(idx + 1).Replace("Z", "").TrimEnd('.', '0');
                if (time.Length > 5) time = time.Substring(0, 5);
                return $"{date} às {time}";
            }

            return s;
        }

        private static string FormatSize(long bytes)
        {
            if (bytes >= 1024 * 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0 * 1024.0):F1} GB";
            if (bytes >= 1024 * 1024)
                return $"{bytes / (1024.0 * 1024.0):F1} MB";
            if (bytes >= 1024)
                return $"{bytes / 1024.0:F0} KB";
            return $"{bytes} B";
        }
    }
}