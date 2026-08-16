using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Velopack;
using Velopack.Exceptions;
using Velopack.Sources;
using VelopackManager = Velopack.UpdateManager;

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

        /// <summary>True quando a atualização é fornecida pelo Velopack (delta).</summary>
        public bool IsVelopack { get; set; }
        public long DeltaSizeBytes { get; set; }
        public UpdateInfo? VelopackInfo { get; set; }
    }

    /// <summary>
    /// Gerenciador de verificação e instalação automática de atualizações.
    /// Instalações Velopack usam pacotes delta; execuções portáteis caem no fluxo GitHub Releases.
    /// </summary>
    public static class UpdateManager
    {
        private const string RepoOwner = "Gabriel-Freitas-S";
        private const string RepoName = "CGPDI.StudyLab";
        private const string RepoUrl = $"https://github.com/{RepoOwner}/{RepoName}";
        private const string ApiUrl = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";

        // HttpClient estático: evita esgotamento de conexões (TIME_WAIT) a cada verificação de atualização.
        private static readonly HttpClient SharedClient = CreateClient();
        private static readonly object SyncRoot = new();
        private static VelopackManager? _velopackManager;

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "CGPDI.StudyLab-App");
            client.Timeout = TimeSpan.FromSeconds(8);
            return client;
        }

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
        /// Indica se o processo atual roda com privilégios de administrador.
        /// </summary>
        public static bool IsElevated
        {
            get
            {
                try
                {
                    using var identity = WindowsIdentity.GetCurrent();
                    var principal = new WindowsPrincipal(identity);
                    return principal.IsInRole(WindowsBuiltInRole.Administrator);
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Indica se o app está instalado para todos os usuários (ex.: Program Files),
        /// caso em que atualizações exigem permissão de administrador.
        /// </summary>
        public static bool IsMachineWideInstall
        {
            get
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\', '/');
                    string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles).TrimEnd('\\', '/');
                    string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86).TrimEnd('\\', '/');

                    if (baseDir.StartsWith(programFiles, StringComparison.OrdinalIgnoreCase)) return true;
                    if (!string.IsNullOrEmpty(programFilesX86) &&
                        baseDir.StartsWith(programFilesX86, StringComparison.OrdinalIgnoreCase)) return true;

                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Verifica se o app está instalado via Velopack (capaz de aplicar atualizações delta).
        /// </summary>
        public static bool IsVelopackInstalled
        {
            get
            {
                try
                {
                    return GetVelopackManager().IsInstalled;
                }
                catch
                {
                    return false;
                }
            }
        }

        private static VelopackManager GetVelopackManager()
        {
            lock (SyncRoot)
            {
                if (_velopackManager == null)
                {
                    var source = new GithubSource(RepoUrl, accessToken: null, prerelease: false);
                    _velopackManager = new VelopackManager(source, new UpdateOptions(), locator: null);
                }

                return _velopackManager;
            }
        }

        /// <summary>
        /// Verifica de forma assíncrona se existe uma versão mais recente.
        /// Prefere o Velopack (instalado) e cai no GitHub Releases para execuções portáteis.
        /// </summary>
        public static async Task<ReleaseInfo?> CheckForUpdatesAsync()
        {
            try
            {
                var velopack = GetVelopackManager();
                if (velopack.IsInstalled)
                {
                    var info = await velopack.CheckForUpdatesAsync();
                    if (info != null && !info.IsDowngrade && info.TargetFullRelease != null)
                    {
                        var release = BuildVelopackReleaseInfo(info);
                        await EnrichWithGitHubDetailsAsync(release);
                        return release;
                    }

                    return null;
                }
            }
            catch (NotInstalledException)
            {
                // Execução portátil: segue para o fluxo GitHub Releases.
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Erro na verificação Velopack: {ex.Message}");
            }

            return await CheckGitHubReleaseAsync();
        }

        private static ReleaseInfo BuildVelopackReleaseInfo(UpdateInfo info)
        {
            var target = info.TargetFullRelease;
            var version = new Version(target.Version.Major, target.Version.Minor, target.Version.Patch);
            long deltaSize = info.DeltasToTarget is { Length: > 0 } deltas ? deltas.Min(d => d.Size) : 0;

            return new ReleaseInfo
            {
                TagName = $"v{version}",
                Version = version,
                Name = $"CGPDI StudyLab v{version}",
                ReleaseNotes = target.NotesMarkdown ?? "",
                HtmlUrl = $"{RepoUrl}/releases/tag/v{version}",
                SetupSizeBytes = target.Size,
                PortableSizeBytes = 0,
                IsVelopack = true,
                DeltaSizeBytes = deltaSize,
                VelopackInfo = info
            };
        }

        /// <summary>
        /// Completa o ReleaseInfo com dados da release no GitHub (URLs de download,
        /// tamanhos e data de publicação) — usado para o instalador de fallback da TI.
        /// </summary>
        private static async Task EnrichWithGitHubDetailsAsync(ReleaseInfo release)
        {
            try
            {
                string url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/tags/{release.TagName}";
                using var response = await SharedClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("published_at", out var p)) release.PublishedAt = p.GetString() ?? "";
                if (root.TryGetProperty("html_url", out var h)) release.HtmlUrl = h.GetString() ?? release.HtmlUrl;

                if (root.TryGetProperty("assets", out var assets) && assets.ValueKind == JsonValueKind.Array)
                {
                    foreach (var asset in assets.EnumerateArray())
                    {
                        string assetName = asset.GetProperty("name").GetString() ?? "";
                        string downloadUrl = asset.GetProperty("browser_download_url").GetString() ?? "";
                        long size = asset.TryGetProperty("size", out var s) ? s.GetInt64() : 0;

                        if (assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) &&
                            !assetName.EndsWith("-delta.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            release.SetupDownloadUrl ??= downloadUrl;
                            release.SetupSizeBytes = size;
                        }
                        else if (assetName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            release.PortableDownloadUrl ??= downloadUrl;
                            release.PortableSizeBytes = size;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Falha ao enriquecer release com detalhes do GitHub: {ex.Message}");
            }
        }

        private static async Task<ReleaseInfo?> CheckGitHubReleaseAsync()
        {
            try
            {
                var response = await SharedClient.GetAsync(ApiUrl);
                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                string tagName = root.GetProperty("tag_name").GetString() ?? "";
                string cleanTag = tagName.TrimStart('v', 'V');

                if (!Version.TryParse(cleanTag, out Version? latestVersion)) return null;
                if (latestVersion <= CurrentVersion) return null;

                var release = new ReleaseInfo
                {
                    TagName = tagName,
                    Version = latestVersion,
                    Name = root.TryGetProperty("name", out var n) ? n.GetString() ?? tagName : tagName,
                    ReleaseNotes = root.TryGetProperty("body", out var b) ? b.GetString() ?? "" : "",
                    PublishedAt = root.TryGetProperty("published_at", out var p) ? p.GetString() ?? "" : "",
                    HtmlUrl = root.TryGetProperty("html_url", out var h) ? h.GetString() ?? "" : $"{RepoUrl}/releases"
                };

                if (root.TryGetProperty("assets", out var assets) && assets.ValueKind == JsonValueKind.Array)
                {
                    foreach (var asset in assets.EnumerateArray())
                    {
                        string assetName = asset.GetProperty("name").GetString() ?? "";
                        string downloadUrl = asset.GetProperty("browser_download_url").GetString() ?? "";
                        long size = asset.TryGetProperty("size", out var s) ? s.GetInt64() : 0;

                        if (assetName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase) &&
                            !assetName.EndsWith("-delta.exe", StringComparison.OrdinalIgnoreCase))
                        {
                            release.SetupDownloadUrl ??= downloadUrl;
                            release.SetupSizeBytes = size;
                        }
                        else if (assetName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            release.PortableDownloadUrl ??= downloadUrl;
                            release.PortableSizeBytes = size;
                        }
                    }
                }

                return release;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Erro na verificação: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Baixa a atualização (delta quando possível) e inicia a instalação e reinicialização.
        /// </summary>
        public static async Task DownloadAndApplyUpdateAsync(
            ReleaseInfo release,
            bool preferInstaller,
            IProgress<double>? progress = null,
            CancellationToken ct = default)
        {
            if (release.IsVelopack && release.VelopackInfo != null)
            {
                await ApplyVelopackUpdateAsync(release, progress, ct);
                return;
            }

            await ApplyManualUpdateAsync(release, preferInstaller, progress, ct);
        }

        private static async Task ApplyVelopackUpdateAsync(
            ReleaseInfo release,
            IProgress<double>? progress,
            CancellationToken ct)
        {
            var velopack = GetVelopackManager();

            // Instalação para todos os usuários sem privilégios: tenta a tarefa agendada
            // SYSTEM criada na instalação ou elevação com UAC.
            if (IsMachineWideInstall && !IsElevated)
            {
                if (TryTriggerSystemUpdateTask() || TryRelaunchElevated())
                {
                    progress?.Report(100);
                    ShutdownApp();
                    return;
                }

                // Fallback inteligente para laboratórios universitários:
                // Se a tarefa SYSTEM não puder ser acionada e o usuário for padrão (sem UAC/admin),
                // aplica a atualização no diretório local do usuário (%LocalAppData%) sem depender da TI.
                Debug.WriteLine("[UpdateManager] Fallback inteligente: aplicando atualização no espaço de usuário.");
                await ApplyManualUpdateAsync(release, preferInstaller: false, progress, ct);
                return;
            }

            await velopack.DownloadUpdatesAsync(
                release.VelopackInfo!,
                percent => progress?.Report(percent),
                ct);

            velopack.ApplyUpdatesAndRestart(release.VelopackInfo!.TargetFullRelease, Array.Empty<string>());
        }

        internal const string UpdateTaskName = "CGPDI StudyLab AutoUpdate";
        internal const string ApplyUpdateArg = "--apply-update";
        internal const string ApplyUpdateTaskArg = "--apply-update-task";

        private static string CurrentExePath =>
            Process.GetCurrentProcess().MainModule?.FileName
            ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CGPDI.StudyLab.exe");

        private static void ShutdownApp()
        {
            try
            {
                Application.Current?.Dispatcher?.Invoke(() => Application.Current.Shutdown());
            }
            catch
            {
                // App já encerrando.
            }
        }

        private static int RunProcess(string fileName, string arguments)
        {
            var psi = new ProcessStartInfo(fileName, arguments)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            using var process = Process.Start(psi);
            if (process == null) return -1;

            process.WaitForExit(30_000);
            return process.HasExited ? process.ExitCode : -1;
        }

        /// <summary>
        /// Cria a tarefa agendada que aplica atualizações como SYSTEM em instalações
        /// para todos os usuários, sem exigir UAC a cada atualização. É chamado durante
        /// a instalação, quando o processo já roda elevado. Inofensivo em execução portátil.
        /// </summary>
        public static bool EnsureSystemUpdateTask()
        {
            if (!IsMachineWideInstall) return false;

            try
            {
                string arguments = $"/create /f /tn \"{UpdateTaskName}\" " +
                    $"/tr \"\\\"{CurrentExePath}\\\" {ApplyUpdateTaskArg}\" " +
                    "/sc once /sd 01/01/2099 /st 00:00 /ru SYSTEM /rl HIGHEST";
                return RunProcess("schtasks.exe", arguments) == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Falha ao criar tarefa agendada: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Dispara a tarefa SYSTEM de atualização, se existir (criada na instalação).
        /// O processo elevado baixa e aplica a atualização sem qualquer diálogo.
        /// </summary>
        public static bool TryTriggerSystemUpdateTask()
        {
            try
            {
                return RunProcess("schtasks.exe", $"/run /tn \"{UpdateTaskName}\"") == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Falha ao disparar tarefa agendada: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Reinicia o app elevado (UAC) para aplicar a atualização em segundo plano.
        /// Retorna false se o usuário não tiver como elevar (UAC negado/sem conta admin).
        /// </summary>
        public static bool TryRelaunchElevated()
        {
            try
            {
                var psi = new ProcessStartInfo(CurrentExePath)
                {
                    UseShellExecute = true,
                    Verb = "runas",
                    Arguments = ApplyUpdateArg
                };
                Process.Start(psi);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executado pelo processo elevado/SYSTEM acionado durante a atualização de uma
        /// instalação para todos os usuários: baixa a versão mais recente em silêncio e
        /// reinicia o app. Retorna false quando não há nada a aplicar.
        /// </summary>
        public static async Task<bool> ApplyPendingUpdateSilentlyAsync()
        {
            try
            {
                var velopack = GetVelopackManager();
                if (!velopack.IsInstalled) return false;

                var info = await velopack.CheckForUpdatesAsync();
                if (info == null || info.IsDowngrade || info.TargetFullRelease == null) return false;

                await velopack.DownloadUpdatesAsync(info, null);
                velopack.ApplyUpdatesAndRestart(info.TargetFullRelease, Array.Empty<string>());
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[UpdateManager] Falha ao aplicar atualização silenciosa: {ex.Message}");
                return false;
            }
        }

        private static async Task ApplyManualUpdateAsync(
            ReleaseInfo release,
            bool preferInstaller,
            IProgress<double>? progress,
            CancellationToken ct)
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
            using (var response = await SharedClient.GetAsync(targetUrl, HttpCompletionOption.ResponseHeadersRead, ct))
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
                string targetDir = appDir;

                // Se o diretório atual não for gravável (ex: Program Files com usuário sem admin),
                // extrai no diretório do usuário (%LocalAppData%)
                if (!IsDirectoryWritable(targetDir))
                {
                    targetDir = GetLocalAppDirectory();
                    if (!Directory.Exists(targetDir)) Directory.CreateDirectory(targetDir);
                }

                string targetExe = Path.Combine(targetDir, "CGPDI.StudyLab.exe");
                int currentPid = Process.GetCurrentProcess().Id;

                string batchPath = Path.Combine(tempFolder, "apply_update.bat");
                string batchScript = $@"@echo off
echo Aguardando finalizacao do CGPDI StudyLab (PID: {currentPid})...
timeout /t 2 /nobreak > nul
taskkill /F /PID {currentPid} >nul 2>&1

echo Extraindo nova versao em {targetDir}...
powershell -Command ""Expand-Archive -Path '{destFilePath}' -DestinationPath '{targetDir}' -Force""

echo Reiniciando CGPDI StudyLab...
start """" ""{targetExe}""
del ""%~f0""
";
                File.WriteAllText(batchPath, batchScript);

                var psi = new ProcessStartInfo("cmd.exe", $"/c \"{batchPath}\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
                Application.Current?.Dispatcher?.Invoke(() => Application.Current.Shutdown());
            }
        }

        /// <summary>
        /// Obtém o diretório local do usuário em %LocalAppData% para atualizações sem necessidade de administrador.
        /// </summary>
        public static string GetLocalAppDirectory()
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(localAppData, "CGPDI.StudyLab", "app");
        }

        /// <summary>
        /// Verifica se um diretório possui permissão de gravação no processo atual.
        /// </summary>
        public static bool IsDirectoryWritable(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                string testFile = Path.Combine(dirPath, $"writable_test_{Guid.NewGuid():N}.tmp");
                using (var fs = File.Create(testFile, 1, FileOptions.DeleteOnClose))
                {
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}