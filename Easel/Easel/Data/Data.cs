using System.IO;
using Easel.Core;
using Easel.Data;
using Easel.Math;

namespace Easel.Configs;

public static class Data
{
    public static string AppBaseDir = "Data";

    public static string ConfigFile = "Config.cfg";

    public static EaselConfig LoadedConfig;

    public static bool LoadConfig<T>() where T : EaselConfig
    {
        LoadedConfig = GetConfigFromFile<T>(Path.Combine(AppBaseDir, ConfigFile));
        if (LoadedConfig == null)
            return false;

        return true;
    }

    public static bool LoadConfig<T>(ref GameSettings settings) where T : EaselConfig
    {
        if (!LoadConfig<T>())
        {
            settings.Size = new Size<int>(1280, 720);
            settings.VSync = true;

            return false;
        }

        settings.Size = LoadedConfig.DisplayConfig.Size;
        settings.VSync = LoadedConfig.DisplayConfig.VSync;
        // TODO: GameSettings doesn't have a "start in fullscreen" option??

        return true;
    }

    public static void SaveConfig()
    {
        SaveConfigFromFile(Path.Combine(AppBaseDir, ConfigFile), LoadedConfig);
    }

    public static void SaveConfigFromFile(string path, EaselConfig config)
    {
        Logger.Debug("Saving config file \"" + ConfigFile + "\".");
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, XmlSerializer.Serialize(config));
    }

    public static T GetConfigFromFile<T>(string path) where T : EaselConfig
    {
        Logger.Debug("Loading config file \"" + ConfigFile + "\".");
        if (!File.Exists(path))
            return null;
        return XmlSerializer.Deserialize<T>(File.ReadAllText(path));
    }
}