using System.Text.Json;

namespace TinyCity
{
    public class TinyCitySettings
    {
        public List<string> MarkdownFiles { get; set; } = new List<string>();

        public string HomeDirectory { get; set; }
        
        public static TinyCitySettings Load()
        {
            string homeDirectory = GetApplicationDirectory();
            var settings = new TinyCitySettings();
            var configFilePath = Path.Combine(homeDirectory, "config.json");
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                settings = JsonSerializer.Deserialize<TinyCitySettings>(json) ?? new TinyCitySettings();
            }

            if (!settings.MarkdownFiles.Contains("bookmarks.md"))
            {
                settings.MarkdownFiles.Add("bookmarks.md");
            }
            settings.HomeDirectory = homeDirectory;
            return settings;
        }

        public static void Save(TinyCitySettings settings)
        {
            string homeDirectory = GetApplicationDirectory();
            var configFilePath = Path.Combine(homeDirectory, "config.json");
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        }

        private static string GetApplicationDirectory()
        {
            string homePath = "";

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                // todo
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                homePath = Environment.GetEnvironmentVariable("LOCALAPPDATA");
            }

            if (string.IsNullOrEmpty(homePath))
            {
                throw new Exception("Unable to determine home directory.");
            }

            string applicationPath = Path.Combine(homePath, "tinycity");
            if (!Directory.Exists(applicationPath))
            {
                Directory.CreateDirectory(applicationPath);
            }

            return applicationPath;
        }
    }
}
