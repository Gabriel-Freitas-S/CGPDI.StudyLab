using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using FluentAssertions;
using Xunit;

namespace CGPDI.StudyLab.Tests.UnitTests
{
    /// <summary>
    /// Testes automatizados para validação e garantia de integridade da documentação Astro Starlight (docs/).
    /// Previne links quebrados (404), erros de layout em cards, rotas inválidas e garante a presença de favicons e assets.
    /// </summary>
    public class DocsValidationTests
    {
        private static string GetDocsRoot()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string docsPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "..", "docs"));
            if (!Directory.Exists(docsPath))
            {
                docsPath = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", "docs"));
            }
            return docsPath;
        }

        [Fact]
        public void Docs_FaviconAndLogoAssets_ExistAndAreValidSvg()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string faviconPath = Path.Combine(docsDir, "public", "favicon.svg");
            string logoPath = Path.Combine(docsDir, "src", "assets", "logo.svg");

            File.Exists(faviconPath).Should().BeTrue("o arquivo favicon.svg deve existir na pasta docs/public");
            File.Exists(logoPath).Should().BeTrue("o arquivo logo.svg deve existir na pasta docs/src/assets");

            // Valida se o SVG é XML válido
            var faviconDoc = XDocument.Load(faviconPath);
            faviconDoc.Root.Should().NotBeNull();
            faviconDoc.Root!.Name.LocalName.Should().Be("svg");

            var logoDoc = XDocument.Load(logoPath);
            logoDoc.Root.Should().NotBeNull();
            logoDoc.Root!.Name.LocalName.Should().Be("svg");
        }

        [Fact]
        public void Docs_AstroConfig_ConfiguresFaviconAndValidSite()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string configPath = Path.Combine(docsDir, "astro.config.mjs");
            File.Exists(configPath).Should().BeTrue();

            string configContent = File.ReadAllText(configPath);
            configContent.Should().Contain("favicon: '/favicon.svg'", "astro.config.mjs deve configurar o favicon oficial");
            configContent.Should().Contain("site: 'https://cgpdi.gabrielfs.dev'");
            configContent.Should().Contain("src: './src/assets/logo.svg'");
        }

        [Fact]
        public void Docs_DownloadLinks_PointToValidReleaseAssets()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string indexPath = Path.Combine(docsDir, "src", "content", "docs", "index.mdx");
            File.Exists(indexPath).Should().BeTrue();

            string indexContent = File.ReadAllText(indexPath);

            // Valida links de download válidos
            string[] validAssetNames = {
                "CGPDIStudyLab-win-Setup.exe",
                "CGPDI-StudyLab-MachineWide.msi",
                "CGPDI-StudyLab-Portable-win-x64.zip"
            };

            foreach (var asset in validAssetNames)
            {
                indexContent.Should().Contain(asset, $"index.mdx deve conter link para o artefato oficial '{asset}'");
            }

            // Garante que não contenha links quebrados antigos
            indexContent.Should().NotContain("download/CGPDI-StudyLab-Portable.exe", "não deve referenciar executável portátil inexistente");
        }

        [Fact]
        public void Docs_SidebarLinksInAstroConfig_AllPointToExistingMarkdownFiles()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string configPath = Path.Combine(docsDir, "astro.config.mjs");
            string contentDir = Path.Combine(docsDir, "src", "content", "docs");

            string configContent = File.ReadAllText(configPath);

            var matches = Regex.Matches(configContent, @"link:\s*'([^']+)'");
            matches.Count.Should().BeGreaterThan(10, "devem existir rotas configuradas na barra lateral");

            foreach (Match match in matches)
            {
                string route = match.Groups[1].Value.Trim();
                if (route == "/" || string.IsNullOrWhiteSpace(route)) continue;

                // Rota ex: /iniciantes/o-que-e-dotnet-csharp/
                string cleanRoute = route.Trim('/');
                string targetMd = Path.Combine(contentDir, cleanRoute.Replace('/', Path.DirectorySeparatorChar) + ".md");
                string targetMdx = Path.Combine(contentDir, cleanRoute.Replace('/', Path.DirectorySeparatorChar) + ".mdx");
                string targetIndex = Path.Combine(contentDir, cleanRoute.Replace('/', Path.DirectorySeparatorChar), "index.md");
                string targetIndexMdx = Path.Combine(contentDir, cleanRoute.Replace('/', Path.DirectorySeparatorChar), "index.mdx");

                bool exists = File.Exists(targetMd) || File.Exists(targetMdx) || File.Exists(targetIndex) || File.Exists(targetIndexMdx);
                exists.Should().BeTrue($"A rota '{route}' configurada na barra lateral deve apontar para um arquivo markdown existente em docs/src/content/docs");
            }
        }

        [Fact]
        public void Docs_NoRawEmojisInHeadingsOrMermaid_StandardTypography()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string contentDir = Path.Combine(docsDir, "src", "content", "docs");
            var files = Directory.GetFiles(contentDir, "*.md*", SearchOption.AllDirectories);

            var emojiRegex = new Regex(@"[\uD83C-\uDBFF\uDC00-\uDFFF\u2600-\u26FF\u2700-\u27BF]", RegexOptions.Compiled);

            var violations = new List<string>();

            foreach (var file in files)
            {
                string[] lines = File.ReadAllLines(file);
                for (int i = 0; i < lines.Length; i++)
                {
                    string line = lines[i];

                    // Verifica títulos (##, ###) e Mermaid nodes
                    if (line.TrimStart().StartsWith("#") || line.Contains("graph ") || line.Contains("flowchart ") || line.Contains("-->") || line.Contains(":::"))
                    {
                        if (emojiRegex.IsMatch(line))
                        {
                            violations.Add($"{Path.GetFileName(file)}:L{i + 1}: {line.Trim()}");
                        }
                    }
                }
            }

            violations.Should().BeEmpty("títulos e diagramas Mermaid na documentação não devem conter emojis crus");
        }

        [Fact]
        public void Docs_AllMarkdownFiles_HaveFrontmatterTitleAndDescription()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string contentDir = Path.Combine(docsDir, "src", "content", "docs");
            var files = Directory.GetFiles(contentDir, "*.md*", SearchOption.AllDirectories);

            files.Length.Should().BeGreaterThan(25);

            foreach (var file in files)
            {
                string content = File.ReadAllText(file);
                content.Should().StartWith("---", $"o arquivo {Path.GetFileName(file)} deve conter cabeçalho YAML frontmatter");
                content.Should().Contain("title:", $"o arquivo {Path.GetFileName(file)} deve conter o atributo 'title'");
                content.Should().Contain("description:", $"o arquivo {Path.GetFileName(file)} deve conter o atributo 'description'");
            }
        }

        [Fact]
        public void Docs_FilesWithImports_MustUseMdxExtension()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string contentDir = Path.Combine(docsDir, "src", "content", "docs");
            var plainMdFiles = Directory.GetFiles(contentDir, "*.md", SearchOption.AllDirectories);

            var violations = new List<string>();

            foreach (var file in plainMdFiles)
            {
                string content = File.ReadAllText(file);
                if (content.Contains("import {") && content.Contains("@astrojs/starlight"))
                {
                    violations.Add(Path.GetFileName(file));
                }
            }

            violations.Should().BeEmpty("arquivos com imports de componentes JSX/Starlight devem usar a extensão .mdx para não renderizar o import como texto visível");
        }

        [Fact]
        public void Docs_NpmSecurityRules_ConfiguredProperly()
        {
            string docsDir = GetDocsRoot();
            if (!Directory.Exists(docsDir)) return;

            string npmrcPath = Path.Combine(docsDir, ".npmrc");
            File.Exists(npmrcPath).Should().BeTrue("o arquivo .npmrc deve existir na pasta docs/ para blindar contra ataques de cadeia de suprimentos");

            string npmrcContent = File.ReadAllText(npmrcPath);
            npmrcContent.Should().Contain("ignore-scripts=true", "deve bloquear a execução arbitrária de lifecycle scripts");
            npmrcContent.Should().Contain("save-exact=true", "deve forçar a fixação exata de versões");
            npmrcContent.Should().Contain("package-lock=true", "deve exigir a integridade do package-lock");
            npmrcContent.Should().Contain("registry=https://registry.npmjs.org/", "deve apontar para o registry oficial HTTPS");

            string packageJsonPath = Path.Combine(docsDir, "package.json");
            File.Exists(packageJsonPath).Should().BeTrue();

            string packageJsonContent = File.ReadAllText(packageJsonPath);
            packageJsonContent.Should().NotContain("\"^", "todas as dependências do docs/package.json devem estar com versão exata fixada (sem ^)");
        }
    }
}
