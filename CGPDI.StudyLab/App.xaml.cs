using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using CGPDI.StudyLab.Core;
using Velopack;
using UpdateManager = CGPDI.StudyLab.Core.UpdateManager;

[assembly: InternalsVisibleTo("CGPDI.StudyLab.Tests")]

namespace CGPDI.StudyLab
{
    public partial class App : Application
    {
        public App()
        {
            SetupGlobalExceptionHandling();
        }

        private void SetupGlobalExceptionHandling()
        {
            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            LogCrashReport(e.Exception, "DispatcherUnhandledException");
            ShowCrashDialog(e.Exception);
            e.Handled = true;
        }

        private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogCrashReport(ex, "AppDomainUnhandledException");
            }
        }

        private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            LogCrashReport(e.Exception, "TaskSchedulerUnobservedTaskException");
            e.SetObserved();
        }

        public static string GetCrashLogPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "CGPDI.StudyLab",
                "logs",
                "crash.log");
        }

        public static void LogCrashReport(Exception ex, string source)
        {
            try
            {
                string logFile = GetCrashLogPath();
                string? dir = Path.GetDirectoryName(logFile);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var sb = new StringBuilder();
                sb.AppendLine("============================================================");
                sb.AppendLine($"[DATA/HORA]: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                sb.AppendLine($"[FONTE]: {source}");
                sb.AppendLine($"[VERSÃO APP]: {UpdateManager.CurrentVersionString}");
                sb.AppendLine($"[OS]: {Environment.OSVersion} ({(Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit")})");
                sb.AppendLine($"[.NET RUNTIME]: {Environment.Version}");
                sb.AppendLine($"[TIPO]: {ex.GetType().FullName}");
                sb.AppendLine($"[MENSAGEM]: {ex.Message}");
                sb.AppendLine($"[STACK TRACE]:");
                sb.AppendLine(ex.StackTrace ?? "(sem stack trace)");
                if (ex.InnerException != null)
                {
                    sb.AppendLine($"[INNER EXCEPTION]: {ex.InnerException.GetType().FullName}: {ex.InnerException.Message}");
                    sb.AppendLine(ex.InnerException.StackTrace ?? "");
                }
                sb.AppendLine("============================================================");
                sb.AppendLine();

                File.AppendAllText(logFile, sb.ToString(), Encoding.UTF8);
            }
            catch
            {
                // Evita falha secundária durante o log de falhas
            }
        }

        private static void ShowCrashDialog(Exception ex)
        {
            try
            {
                string logPath = GetCrashLogPath();
                string message = $"Ocorreu um erro durante a execução do CGPDI StudyLab:\n\n" +
                                 $"{ex.Message}\n\n" +
                                 $"Detalhes gravados em:\n{logPath}\n\n" +
                                 $"A aplicação tentará continuar.";
                MessageBox.Show(message, "CGPDI StudyLab — Diagnóstico", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch
            {
                // Silencioso se a interface não puder exibir MessageBox
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Hook do Velopack: trata instalação, atalhos e atualizações pendentes.
                // Em execução portátil (sem instalação) é inofensivo e retorna imediatamente.
                // Em instalação para todos os usuários, cria a tarefa SYSTEM que permite
                // atualizar sem UAC a cada versão — o professor não depende da TI.
                VelopackApp.Build()
                    .OnAfterInstallFastCallback(_ => UpdateManager.EnsureSystemUpdateTask())
                    .Run();
            }
            catch (Exception ex)
            {
                LogCrashReport(ex, "VelopackStartup");
            }

            bool silentUpdate = Array.IndexOf(e.Args, UpdateManager.ApplyUpdateArg) >= 0
                                || Array.IndexOf(e.Args, UpdateManager.ApplyUpdateTaskArg) >= 0;

            base.OnStartup(e);

            if (silentUpdate)
            {
                // Processo elevado/SYSTEM acionado pela atualização: aplica tudo em
                // segundo plano, sem janelas, e reinicia o app normalmente.
                StartupUri = null;
                _ = RunSilentUpdateAsync();
                return;
            }

            try
            {
                AppIconHelper.EnsureIconFilesExist();
            }
            catch (Exception ex)
            {
                LogCrashReport(ex, "AppIconHelperStartup");
            }
        }

        private async Task RunSilentUpdateAsync()
        {
            bool applied = await UpdateManager.ApplyPendingUpdateSilentlyAsync();
            if (!applied)
            {
                try
                {
                    Shutdown();
                }
                catch (Exception)
                {
                    // Ignorar falha no encerramento silencioso
                }
            }
        }
    }
}