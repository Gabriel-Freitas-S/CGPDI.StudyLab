using System.Runtime.CompilerServices;
using System.Windows;
using CGPDI.StudyLab.Core;
using Velopack;
using UpdateManager = CGPDI.StudyLab.Core.UpdateManager;

[assembly: InternalsVisibleTo("CGPDI.StudyLab.Tests")]

namespace CGPDI.StudyLab
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Hook do Velopack: trata instalação, atalhos e atualizações pendentes.
            // Em execução portátil (sem instalação) é inofensivo e retorna imediatamente.
            // Em instalação para todos os usuários, cria a tarefa SYSTEM que permite
            // atualizar sem UAC a cada versão — o professor não depende da TI.
            VelopackApp.Build()
                .OnAfterInstallFastCallback(_ => UpdateManager.EnsureSystemUpdateTask())
                .Run();

            bool silentUpdate = Array.IndexOf(e.Args, UpdateManager.ApplyUpdateArg) >= 0
                                || Array.IndexOf(e.Args, UpdateManager.ApplyUpdateTaskArg) >= 0;

            base.OnStartup(e);

            if (silentUpdate)
            {
                // Processo elevado/SYSTEM acionado pela atualização: aplica tudo em
                // segundo plano, sem janelas, e reinicia o app normalmente.
                StartupUri = null;
                RunSilentUpdateAsync();
                return;
            }

            AppIconHelper.EnsureIconFilesExist();
        }

        private async void RunSilentUpdateAsync()
        {
            bool applied = await UpdateManager.ApplyPendingUpdateSilentlyAsync();
            if (!applied)
            {
                try { Shutdown(); }
                catch { }
            }
        }
    }
}