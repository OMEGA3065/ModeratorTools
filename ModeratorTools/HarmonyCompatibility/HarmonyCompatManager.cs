
using System.IO;

namespace ModeratorTools.HarmonyCompatibility;

public static class HarmonyCompatManager
{
    private static bool _initialized = false;
    
    public static bool TryEnableHarmonyCompat(bool logOnSuccess = false, bool logOnFailure = true)
    {
        try
        {
            HarmonyCompat.Initialize();
            if (logOnSuccess) LabApi.Features.Console.Logger.Info("Successfully initialized Harmony Compatibility Layer!");
            _initialized = true;
            return true;
        }
        catch (Exception ex) when (ex is TypeLoadException || ex is FileNotFoundException || ex is FileLoadException)
        {
            if (logOnFailure) LabApi.Features.Console.Logger.Info("Failed to initialize Harmony Compatibility Layer! Some features will be disabled.");
            _initialized = false;
            return false;
        }
    }
    
    public static bool TryDisableHarmonyCompat(bool logOnSuccess = false, bool logOnFailure = false)
    {
        try
        {
            HarmonyCompat.Uninitialize();
            if (logOnSuccess) LabApi.Features.Console.Logger.Info("Successfully disabled Harmony Compatibility Layer!");
            return true;
        }
        catch (Exception ex) when (ex is TypeLoadException || ex is FileNotFoundException || ex is FileLoadException)
        {
            if (logOnFailure) LabApi.Features.Console.Logger.Info($"Failed to disable Harmony Compatibility Layer! Was it ever enabled? Enabled: {_initialized}");
            return false;
        }
    }
}