using System.Text.Json;

namespace TinyCity
{
    
    public class TinyCitySettings
    {
        public List<string> MarkdownFiles { get; set; } = new List<string>();

        public string HomeDirectory { get; set; }

        public string BrowserPath { get; set; } = BrowserKnownPaths.ChromePath;
        
        public static TinyCitySettings Load()
        {
            string homeDirectory = GetApplicationDirectory();
            var settings = new TinyCitySettings();
            var configFilePath = GetConfigFilePath();
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                settings = JsonSerializer.Deserialize<TinyCitySettings>(json) ?? new TinyCitySettings();
            }

            settings.HomeDirectory = homeDirectory;

            // Ensure the config file exists
            if (!File.Exists(configFilePath))
            {
                Save(settings);
            }

            return settings;
        }

        public static void Save(TinyCitySettings settings)
        {
            var configFilePath = GetConfigFilePath();
            string json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(configFilePath, json);
        }

        public static string GetConfigFilePath()
        {
            string homeDirectory = GetApplicationDirectory();
            return Path.Combine(homeDirectory, "config.json");
        }

        private static string GetApplicationDirectory()
        {
            string homePath = "";

            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                homePath = Path.Combine(homePath, ".config");
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
