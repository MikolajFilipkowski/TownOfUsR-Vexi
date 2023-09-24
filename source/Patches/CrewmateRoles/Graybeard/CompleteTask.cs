using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.GraybeardMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static Sprite Sprite => TownOfUs.Arrow;

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Graybeard)) return;
            if (__instance.Data.IsDead) return;

            var role = Role.GetRole<Graybeard>(__instance);
            role.TimeToDeath += CustomGameOptions.GraybeardTaskRegainTime;
        }
    }
}