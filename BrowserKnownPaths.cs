namespace TinyCity
{
    public class BrowserKnownPaths
    {
        public static string ChromePath => Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "google-chrome"),
            PlatformID.MacOSX => throw new NotImplementedException("MacOS path not implemented."),
            PlatformID.Win32NT => Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Google", "Chrome", "User Data"),
            _ => throw new NotSupportedException("Unsupported platform.")
        };

        public static string EdgePath => Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => throw new NotImplementedException("Linux path not implemented."),
            PlatformID.MacOSX => throw new NotImplementedException("MacOS path not implemented."),
            PlatformID.Win32NT => Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "Microsoft", "Edge", "User Data"),
            _ => throw new NotSupportedException("Unsupported platform.")
        };

        public static string BravePath => Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "BraveSoftware", "Brave-Browser"),
            PlatformID.MacOSX => throw new NotImplementedException("MacOS path not implemented."),
            PlatformID.Win32NT => Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "BraveSoftware", "Brave-Browser", "User Data"),
            _ => throw new NotSupportedException("Unsupported platform.")
        };

        public static string OperaPath => Environment.OSVersion.Platform switch
        {
            PlatformID.Unix => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".config", "opera"),
            PlatformID.MacOSX => throw new NotImplementedException("MacOS path not implemented."),
            PlatformID.Win32NT => Path.Combine(Environment.GetEnvironmentVariable("APPDATA"), "Opera Software", "Opera Stable", "User Data"),
            _ => throw new NotSupportedException("Unsupported platform.")
        };
    }
}
