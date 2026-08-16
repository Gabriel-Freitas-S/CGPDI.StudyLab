using System;
using System.IO;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    /// <summary>
    /// Testes automatizados para validação e garantia das políticas de segurança na cadeia de suprimentos
    /// (Supply Chain Security & Anti-Poisoning) para os ecossistemas .NET/NuGet e Node.js/npm.
    /// </summary>
    public class SupplyChainSecurityTests
    {
        private static string GetSolutionRoot()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string root = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            if (File.Exists(Path.Combine(root, "CGPDI.StudyLab.slnx")))
            {
                return root;
            }
            return Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        }

        [Fact]
        public void DotNet_NugetConfig_ConfiguresCleanSourcesAndPackageMapping()
        {
            string root = GetSolutionRoot();
            string nugetConfigPath = Path.Combine(root, "nuget.config");

            File.Exists(nugetConfigPath).Should().BeTrue("o arquivo nuget.config deve existir na raiz para blindar o restore de pacotes");

            var doc = XDocument.Load(nugetConfigPath);
            doc.Root.Should().NotBeNull();

            string xmlContent = doc.ToString();
            xmlContent.Should().Contain("<clear />", "deve limpar fontes locais ou arbitrárias não confiáveis");
            xmlContent.Should().Contain("https://api.nuget.org/v3/index.json", "deve apontar para a fonte oficial HTTPS do nuget.org");
            xmlContent.Should().Contain("<packageSourceMapping>", "deve conter mapeamento restrito de pacotes para mitigar Dependency Confusion");
        }

        [Fact]
        public void DotNet_DirectoryBuildProps_EnforcesLockfilesAndAudit()
        {
            string root = GetSolutionRoot();
            string propsPath = Path.Combine(root, "Directory.Build.props");

            File.Exists(propsPath).Should().BeTrue("Directory.Build.props deve existir na raiz com diretivas de segurança globais");

            string propsContent = File.ReadAllText(propsPath);
            propsContent.Should().Contain("<RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>", "deve exigir a criação e validação de lockfile determinístico (packages.lock.json)");
            propsContent.Should().Contain("<NuGetAudit>true</NuGetAudit>", "deve ativar a auditoria contínua de vulnerabilidades em tempo de restore");
            propsContent.Should().Contain("<NuGetAuditMode>all</NuGetAuditMode>", "deve auditar dependências diretas e transitivas");
        }

        [Fact]
        public void DotNet_Lockfiles_ExistForBothMainAndTestProjects()
        {
            string root = GetSolutionRoot();

            string appLockfile = Path.Combine(root, "CGPDI.StudyLab", "packages.lock.json");
            string testLockfile = Path.Combine(root, "CGPDI.StudyLab.Tests", "packages.lock.json");

            File.Exists(appLockfile).Should().BeTrue("o arquivo packages.lock.json deve existir no projeto principal CGPDI.StudyLab");
            File.Exists(testLockfile).Should().BeTrue("o arquivo packages.lock.json deve existir no projeto de testes CGPDI.StudyLab.Tests");

            string appLockContent = File.ReadAllText(appLockfile);
            appLockContent.Should().Contain("\"contentHash\":", "o lockfile deve conter hashes criptográficos das dependências");
        }

        [Fact]
        public void Npm_NpmrcAndPackageJson_EnforceStrictSupplyChainRules()
        {
            string root = GetSolutionRoot();

            string rootNpmrc = Path.Combine(root, ".npmrc");
            string docsNpmrc = Path.Combine(root, "docs", ".npmrc");
            string packageJson = Path.Combine(root, "docs", "package.json");

            File.Exists(rootNpmrc).Should().BeTrue("deve existir .npmrc na raiz do repositório");
            File.Exists(docsNpmrc).Should().BeTrue("deve existir .npmrc na pasta docs/");

            string rootContent = File.ReadAllText(rootNpmrc);
            rootContent.Should().Contain("ignore-scripts=true", "deve bloquear execução de lifecycle scripts");
            rootContent.Should().Contain("save-exact=true", "deve forçar versões exatas");
            rootContent.Should().Contain("package-lock=true", "deve exigir integridade do package-lock");

            string docsContent = File.ReadAllText(docsNpmrc);
            docsContent.Should().Contain("ignore-scripts=true");
            docsContent.Should().Contain("save-exact=true");

            string pkgContent = File.ReadAllText(packageJson);
            pkgContent.Should().NotContain("\"^", "docs/package.json deve conter versões fixadas sem ^");
        }
    }
}
