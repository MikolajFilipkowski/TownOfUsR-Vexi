using HarmonyLib;
using Reactor.Utilities;
using Rewired;
using System;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.GraybeardMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Graybeard)) return;
            var graybeardRole = Role.GetRole<Graybeard>(PlayerControl.LocalPlayer);

            Coroutines.Stop(DeathTimer.FrameTimer());

            graybeardRole.MeetingInProgress = true;
            graybeardRole.TimeToDeath -= (int)Math.Floor((DateTime.UtcNow - graybeardRole.LastMeeting).TotalSeconds);

            DateTime timeNow = DateTime.UtcNow;

            if (graybeardRole.trappedPlayers.Count == 0)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No players entered any of your traps");
            }
            else
            {
                string message = "Players caught in your trap:\n";
                foreach (var player in graybeardRole.trappedPlayers.OrderBy(x => -(timeNow - x.Value).TotalSeconds))
                {
                    var timeRn = (Math.Floor((timeNow - player.Value).TotalSeconds) == 0) ? "right now" : $"{Math.Floor((timeNow - player.Value).TotalSeconds)}s ago";
                    message += $" {player.Key.GetDefaultOutfit().PlayerName} {timeRn}\n";
                }
                message.Remove(message.Length - 1, 1);
                if (DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }
    }
}
