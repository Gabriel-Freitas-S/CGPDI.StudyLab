using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Utilitário para exportação automática de projetos autônomos prontos para o Visual Studio 2022 (.NET 10 / WPF).
    /// Gera a solução completa (.sln, .csproj, App.xaml, MainWindow.xaml, .cs) para desenvolvimento local e submissão acadêmica.
    /// </summary>
    public static class AcademicProjectExporter
    {
        public static string SanitizeProjectName(string title)
        {
            var sb = new StringBuilder();
            bool nextUpper = true;
            foreach (char c in title)
            {
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(nextUpper ? char.ToUpperInvariant(c) : c);
                    nextUpper = false;
                }
                else
                {
                    nextUpper = true;
                }
            }
            string name = sb.ToString();
            if (string.IsNullOrWhiteSpace(name)) name = "CGPDIProject";
            if (char.IsDigit(name[0])) name = "Project" + name;
            return name;
        }

        public static void ExportProjectToDirectory(ProjectTemplate template, string targetDirectory)
        {
            if (string.IsNullOrWhiteSpace(targetDirectory))
                throw new ArgumentException("Diretório de destino inválido.", nameof(targetDirectory));

            Directory.CreateDirectory(targetDirectory);

            string projName = SanitizeProjectName(template.Title);
            string projFolder = Path.Combine(targetDirectory, projName);
            Directory.CreateDirectory(projFolder);

            // 1. Solution (.sln)
            string slnGuid = Guid.NewGuid().ToString("B").ToUpperInvariant();
            string projGuid = Guid.NewGuid().ToString("B").ToUpperInvariant();
            string slnContent = $@"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.10.35013.160
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{slnGuid}"") = ""{projName}"", ""{projName}\{projName}.csproj"", ""{projGuid}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{projGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{projGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{projGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{projGuid}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
EndGlobal
";
            File.WriteAllText(Path.Combine(targetDirectory, $"{projName}.sln"), slnContent.TrimStart(), Encoding.UTF8);

            // 2. .csproj (.NET 10 WPF)
            string csprojContent = @"<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net10.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>
</Project>
";
            File.WriteAllText(Path.Combine(projFolder, $"{projName}.csproj"), csprojContent, Encoding.UTF8);

            // 3. App.xaml e App.xaml.cs
            string appXaml = $@"<Application x:Class=""{projName}.App""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
             StartupUri=""MainWindow.xaml"">
    <Application.Resources>
    </Application.Resources>
</Application>";
            File.WriteAllText(Path.Combine(projFolder, "App.xaml"), appXaml, Encoding.UTF8);

            string appCs = $@"using System.Windows;

namespace {projName}
{{
    public partial class App : Application
    {{
    }}
}}";
            File.WriteAllText(Path.Combine(projFolder, "App.xaml.cs"), appCs, Encoding.UTF8);

            // 4. MainWindow.xaml
            string xamlBody = !string.IsNullOrWhiteSpace(template.XamlCode) ? template.XamlCode : @"<Canvas Background=""#111827""/>";
            string mainXaml = $@"<Window x:Class=""{projName}.MainWindow""
        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
        Title=""{template.Title} — Visual Studio 2022""
        Height=""600"" Width=""900""
        Background=""#0B0F19""
        WindowStartupLocation=""CenterScreen"">
    <Grid>
{IndentXml(xamlBody, 8)}
    </Grid>
</Window>";
            File.WriteAllText(Path.Combine(projFolder, "MainWindow.xaml"), mainXaml, Encoding.UTF8);

            // 5. MainWindow.xaml.cs
            string mainCs = $@"using System;
using System.Windows;
using System.Windows.Media;

namespace {projName}
{{
    /// <summary>
    /// {template.Title}
    /// {template.Description}
    /// </summary>
    public partial class MainWindow : Window
    {{
        public MainWindow()
        {{
            InitializeComponent();
        }}
    }}
}}";
            File.WriteAllText(Path.Combine(projFolder, "MainWindow.xaml.cs"), mainCs, Encoding.UTF8);

            // 6. README com Guia Passo a Passo
            string readmeContent = $@"# {template.Title}

{template.Description}

## Instruções de Execução:
1. Abra o arquivo `{projName}.sln` no Visual Studio 2022.
2. Certifique-se de que o SDK .NET 10 está instalado.
3. Pressione F5 (Debug) ou Ctrl+F5 (Start Without Debugging) para executar.

## Roteiro de Implementação:
{template.StepsGuide}
";
            File.WriteAllText(Path.Combine(targetDirectory, "README.md"), readmeContent, Encoding.UTF8);
        }

        public static void ExportProjectToZip(ProjectTemplate template, string zipFilePath)
        {
            if (string.IsNullOrWhiteSpace(zipFilePath))
                throw new ArgumentException("Caminho do arquivo ZIP inválido.", nameof(zipFilePath));

            string tempDir = Path.Combine(Path.GetTempPath(), "CGPDI_Export_" + Guid.NewGuid().ToString("N"));
            try
            {
                ExportProjectToDirectory(template, tempDir);

                if (File.Exists(zipFilePath))
                    File.Delete(zipFilePath);

                string? dir = Path.GetDirectoryName(zipFilePath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                ZipFile.CreateFromDirectory(tempDir, zipFilePath, CompressionLevel.Optimal, false);
            }
            finally
            {
                if (Directory.Exists(tempDir))
                {
                    try { Directory.Delete(tempDir, true); } catch (Exception) { /* Silencia falhas eventuais de limpeza em diretório temporário */ }
                }
            }
        }

        private static readonly string[] NewLineSeparators = ["\r\n", "\r", "\n"];

        private static string IndentXml(string xml, int spaces)
        {
            string indent = new string(' ', spaces);
            var lines = xml.Split(NewLineSeparators, StringSplitOptions.None);
            var sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                    sb.AppendLine(indent + line);
            }
            return sb.ToString().TrimEnd();
        }
    }
}
