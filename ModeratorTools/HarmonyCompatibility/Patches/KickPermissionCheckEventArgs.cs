using CommandSystem;
using LabApi.Events.Arguments.Interfaces;

namespace ModeratorTools.HarmonyCompatibility.Patches;

public class KickPermissionCheckEventArgs : EventArgs, ICommandSenderEvent, ICancellableEvent
{
    public KickPermissionCheckEventArgs(ICommandSender sender, bool isAllowed)
    {
        Sender = sender as CommandSender;
        IsAllowed = isAllowed;
    }
    
    public CommandSender Sender { get; }
    
    /// <summary>
    /// Whether is the person executing the kick command allowed to kick, as in has the permissions required to do so.
    /// </summary>
    public bool IsAllowed { get; set; }
}