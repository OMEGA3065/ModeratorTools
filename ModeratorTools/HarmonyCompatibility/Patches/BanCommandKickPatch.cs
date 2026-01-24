using System.Reflection.Emit;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin;
using LabApi.Events;
using LabApi.Features.Permissions;

namespace ModeratorTools.HarmonyCompatibility.Patches;

internal static class BanCommandKickPatch
{
    internal static event LabEventHandler<KickPermissionCheckEventArgs> KickPermissionCheckEvent;

    internal static bool CanKick(ICommandSender sender, out string response)
    {
        var canKick = sender.CheckPermission([
            PlayerPermissions.KickingAndShortTermBanning,
            PlayerPermissions.BanningUpToDay,
            PlayerPermissions.LongTermBanning
        ]) || (!ModeratorToolsPlugin.Cfg.VanillaPermissions &&
               !string.IsNullOrWhiteSpace(ModeratorToolsPlugin.Cfg.KickCommandPermissionName) &&
               sender.HasPermissions(ModeratorToolsPlugin.Cfg.KickCommandPermissionName));
        
        var eventArgs = new KickPermissionCheckEventArgs(sender, canKick);
        KickPermissionCheckEvent?.Invoke(eventArgs);

        response = "";
        if (!eventArgs.IsAllowed) response = "You do not have the required permission to execute this command.";
        
        return eventArgs.IsAllowed;
    }
    
    internal static IEnumerable<HarmonyLib.CodeInstruction> Transpiler(IEnumerable<HarmonyLib.CodeInstruction> instructions, ILGenerator generator)
    {
        var checkPermission = HarmonyLib.AccessTools.Method(typeof(BanCommandKickPatch), nameof(CanKick));
        HarmonyLib.CodeInstruction[] additionalCheckInstructions = [
            new(OpCodes.Ldarg_2),
            new(OpCodes.Ldarg_3),
            new(OpCodes.Callvirt, checkPermission)
        ];
        
        int len = instructions.Count();
        bool deleteMode = false;
        bool finished = false;
        
        for (int i = 0; i < len; i++)
        {
            var curInstruction = instructions.ElementAt(i);
            if (!finished && i < len - 1)
            {
                var nextInstruction = instructions.ElementAt(i+1);
                if (deleteMode)
                {
                    if (curInstruction.opcode != OpCodes.Ldc_I4_0 || nextInstruction.opcode != OpCodes.Ret)
                        continue;
                    finished = true;
                    deleteMode = false;
                }
                else if (curInstruction.opcode == OpCodes.Ldloc_3 &&
                         nextInstruction.opcode == OpCodes.Brtrue_S)
                {
                    yield return curInstruction;
                    yield return nextInstruction;
                    foreach (var instruction in additionalCheckInstructions)
                        yield return instruction;

                    yield return new HarmonyLib.CodeInstruction(OpCodes.Brtrue_S, nextInstruction.operand);
                    deleteMode = true;
                    continue;
                }
            }
            yield return curInstruction;
        }
    }
}