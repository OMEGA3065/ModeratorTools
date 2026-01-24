using System.Reflection;
using CommandSystem.Commands.RemoteAdmin;
using HarmonyLib;
using ModeratorTools.HarmonyCompatibility.Patches;

namespace ModeratorTools.HarmonyCompatibility;

public static class HarmonyCompat
{
    private static Harmony _harmony;

    public static void Initialize()
    {
        _harmony = new Harmony("axwabo.moderatortools");
        _harmony.PatchAll();
    }
    
    public static void Uninitialize()
    {
        _harmony?.UnpatchAll();
    }
}