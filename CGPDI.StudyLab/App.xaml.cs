using System.Runtime.CompilerServices;
using System.Windows;
using CGPDI.StudyLab.Core;

[assembly: InternalsVisibleTo("CGPDI.StudyLab.Tests")]

namespace CGPDI.StudyLab
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppIconHelper.EnsureIconFilesExist();
        }
    }
}
