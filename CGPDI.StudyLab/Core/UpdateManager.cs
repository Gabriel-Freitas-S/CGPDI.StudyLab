using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CGPDI.StudyLab.Core
{
    public class ReleaseInfo
    {
        public string TagName { get; set; } = "";
        public Version Version { get; set; } = new Version(1, 0, 0);
        public string Name { get; set; } = "";
        public string ReleaseNotes { get; set; } = "";
        public string PublishedAt { get; set; } = "";
        public string HtmlUrl { get; set; } = "";
        public string? SetupDownloadUrl { get; set; }
        public string? PortableDownloadUrl { get; set; }
        public long SetupSizeBytes { get; set; }
        public long PortableSizeBytes { get; set; }
    }

    /// <summary>
    /// Gerenciador de verificação e instalação automática de atualizações via GitHub Releases.
    /// </summary>
    public static class UpdateManager
    {
        private const string RepoOwner = "Gabriel-Freitas-S";
        private const string RepoName = "CGPDI.StudyLab";
        private const string ApiUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

        public static Version CurrentVersion
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();
                string? fileVersion = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
                if (!string.IsNullOrEmpty(fileVersion) && Version.TryParse(fileVersion, out Version? version))
                    return version;
                return assembly.GetName().Version ?? new Version(1, 0, 0);
            }
        }

        public static string CurrentVersionString =>
            $"{CurrentVersion.Major}.{CurrentVersion.Minor}.{CurrentVersion.Build}";

        /// <summary>
        /// Verifica de forma assíncrona se existe uma versão mais recente no repositório GitHub.
        /// </summary>
        public static async Task<ReleaseInfo?> CheckForUpdatesAsync()
        {
            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("User-Agent", "CGPDI.StudyLab-App");
                client.Timeout = TimeSpan.FromSeconds(8);

                var response = await client.GetAsync(ApiUrl);
                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string tagName = root.GetProperty("tag_name").GetString() ?? "";
                string cleanTag = tagName.TrimStart('v', 'V');

                if (Version.TryParse(cleanTag, out Version? latestVersion))
                {
                    // Compara as versões semânticas
                    if (latestVersion > CurrentVersion)
                    {
                        var release = new ReleaseInfo
                        {
                            TagName = tagName,
                            Version = latestVersion,
                            Name = root.TryGetProperty("name", out var n) ? n.GetString() ?? tagName : tagName,
                            ReleaseNotes = root.TryGetProperty("body", out var b) ? b.GetString() ?? "" : "",
                            PublishedAt = root.TryGetProperty("published_at", out var p) ? p.GetString() ?? "" : "",
                            HtmlUrl = root.TryGetProperty("html_url", out var h) ? h.GetString() ?? "" : $"https://github.com/{RepoOwner}/{RepoName}/releases"
                        };

                        if (root.TryGetProperty("assets", out var assets) && assets.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var asset in assets.EnumerateArray())
                            {
                                string assetName = asset.GetProperty("name").GetString() ?? "";
                                string downloadUrl = asset.GetProperty("browser_download_url").GetString() ?? "";
                                long size = asset.TryGetProperty("size", out var s) ? s.GetInt64() : 0;

                                if (assetName.EndsWith("-Setup.exe", StringComparison.OrdinalIgnoreCase) ||
                                    assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                                {
                                    release.SetupDownloadUrl = downloadUrl;
                                    release.SetupSizeBytes = size;
                                }
                                else if (assetName.EndsWith("-Portable.zip", StringComparison.OrdinalIgnoreCase) ||
                                         assetName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                                {
                                    release.PortableDownloadUrl = downloadUrl;
                                    release.PortableSizeBytes = size;
                                }
                            }
                        }

                        return release;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Erro na verificação: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Faz o download da atualização e inicia o processo de instalação e reinicialização.
        /// </summary>
        public static async Task DownloadAndApplyUpdateAsync(
            ReleaseInfo release,
            bool preferInstaller,
            IProgress<double>? progress = null,
            CancellationToken ct = default)
        {
            string? targetUrl = (preferInstaller && !string.IsNullOrEmpty(release.SetupDownloadUrl))
                ? release.SetupDownloadUrl
                : release.PortableDownloadUrl ?? release.SetupDownloadUrl;

            if (string.IsNullOrEmpty(targetUrl))
            {
                // Fallback para abrir a página de download no navegador
                Process.Start(new ProcessStartInfo(release.HtmlUrl) { UseShellExecute = true });
                return;
            }

            string tempFolder = Path.Combine(Path.GetTempPath(), "CGPDI_StudyLab_Update");
            if (!Directory.Exists(tempFolder)) Directory.CreateDirectory(tempFolder);

            string isZip = targetUrl.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) ? ".zip" : ".exe";
            string destFilePath = Path.Combine(tempFolder, $"Update_{release.TagName}{isZip}");

            // 1. Download do arquivo com progresso
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CGPDI.StudyLab-App");

            using (var response = await client.GetAsync(targetUrl, HttpCompletionOption.ResponseHeadersRead, ct))
            {
                response.EnsureSuccessStatusCode();
                long? totalBytes = response.Content.Headers.ContentLength;

                using var stream = await response.Content.ReadAsStreamAsync(ct);
                using var fs = new FileStream(destFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                var buffer = new byte[81920]; // 80 KB
                long totalRead = 0;
                int read;

                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, ct)) > 0)
                {
                    await fs.WriteAsync(buffer, 0, read, ct);
                    totalRead += read;

                    if (totalBytes.HasValue && totalBytes.Value > 0)
                    {
                        double percent = (double)totalRead / totalBytes.Value * 100.0;
                        progress?.Report(percent);
                    }
                }
            }

            progress?.Report(100.0);

            // 2. Executa o instalador ou aplica a versão portátil
            if (isZip == ".exe")
            {
                // É um instalador executável: executa e fecha a aplicação atual
                var psi = new ProcessStartInfo(destFilePath)
                {
                    UseShellExecute = true
                };
                Process.Start(psi);
                Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
            }
            else
            {
                // É um pacote portátil (.zip): gera um script auxiliar para substituir os arquivos
                string appDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                string currentExe = Process.GetCurrentProcess().MainModule?.FileName ?? Path.Combine(appDir, "CGPDI.StudyLab.exe");
                int currentPid = Process.GetCurrentProcess().Id;

                string batchPath = Path.Combine(tempFolder, "apply_update.bat");
                string batchScript = $@"@echo off
echo Aguardando finalizacao do CGPDI StudyLab (PID: {currentPid})...
timeout /t 2 /nobreak > nul
taskkill /F /PID {currentPid} >nul 2>&1

echo Extraindo nova versao...
powershell -Command ""Expand-Archive -Path '{destFilePath}' -DestinationPath '{appDir}' -Force""

echo Reiniciando CGPDI StudyLab...
start """" ""{currentExe}""
del ""%~f0""
";
                File.WriteAllText(batchPath, batchScript);

                var psi = new ProcessStartInfo("cmd.exe", $"/c \"{batchPath}\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
                Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
            }
        }
    }
}
