using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CGPDI.StudyLab.Core
{
    /// <summary>
    /// Preferências locais do usuário sobre notificações de atualização
    /// (adiar ou ignorar versões específicas).
    /// </summary>
    public sealed class UpdateSettings
    {
        public string? SnoozedVersion { get; set; }
        public DateTimeOffset? SnoozeUntilUtc { get; set; }
        public List<string> SkippedVersions { get; set; } = new();

        public static string NormalizeVersion(Version version) =>
            $"{version.Major}.{version.Minor}.{version.Build}";

        /// <summary>
        /// Indica se o app deve notificar o usuário sobre a versão informada.
        /// </summary>
        public bool ShouldNotifyFor(Version version)
        {
            string key = NormalizeVersion(version);
            if (SkippedVersions.Contains(key)) return false;
            if (SnoozedVersion == key && SnoozeUntilUtc is DateTimeOffset until && DateTimeOffset.UtcNow < until)
                return false;
            return true;
        }

        public void Snooze(Version version, TimeSpan duration)
        {
            SnoozedVersion = NormalizeVersion(version);
            SnoozeUntilUtc = DateTimeOffset.UtcNow.Add(duration);
        }

        public void Skip(Version version)
        {
            string key = NormalizeVersion(version);
            if (!SkippedVersions.Contains(key)) SkippedVersions.Add(key);
            SnoozedVersion = null;
            SnoozeUntilUtc = null;
        }

        public void Clear(Version version)
        {
            string key = NormalizeVersion(version);
            SkippedVersions.Remove(key);
            if (SnoozedVersion == key)
            {
                SnoozedVersion = null;
                SnoozeUntilUtc = null;
            }
        }
    }

    /// <summary>
    /// Persistência das preferências de atualização em JSON sob %LocalAppData%\CGPDI.StudyLab.
    /// </summary>
    public static class UpdateSettingsStore
    {
        public static string DefaultFilePath { get; } = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "CGPDI.StudyLab",
            "update-settings.json");

        public static UpdateSettings Load(string? path = null)
        {
            string file = path ?? DefaultFilePath;
            try
            {
                if (File.Exists(file))
                {
                    string json = File.ReadAllText(file);
                    var settings = JsonSerializer.Deserialize<UpdateSettings>(json);
                    if (settings != null) return settings;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateSettings] Falha ao ler preferências: {ex.Message}");
            }

            return new UpdateSettings();
        }

        public static void Save(UpdateSettings settings, string? path = null)
        {
            string file = path ?? DefaultFilePath;
            try
            {
                string dir = Path.GetDirectoryName(file) ?? "";
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(file, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[UpdateSettings] Falha ao salvar preferências: {ex.Message}");
            }
        }
    }
}