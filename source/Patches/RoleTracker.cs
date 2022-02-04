using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using System.Collections.Generic;
using System.Linq;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class RoleTracker
    {
        public static List<KeyValuePair<byte, RoleEnum>> RoleHistory = new List<KeyValuePair<byte, RoleEnum>>();
        
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var role = Role.GetRole(player);
                if (role != null && !RoleHistory.Any(x => x.Key == player.PlayerId && x.Value == role.RoleType))
                {
                    RoleHistory.Add(KeyValuePair.Create(player.PlayerId, role.RoleType));
                }
            }
        }

        [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
        [HarmonyPriority(Priority.Last)]
        public static void Postfix()
        {
            if (RoleHistory.Any()) RoleHistory.Clear();
        }
    }
}