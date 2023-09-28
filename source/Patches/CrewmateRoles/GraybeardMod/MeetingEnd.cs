using HarmonyLib;
using Reactor.Utilities;
using System;
using TownOfUs.Patches.Roles.Modifiers;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.GraybeardMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Graybeard)) return;

            var graybeardRole = Role.GetRole<Graybeard>(PlayerControl.LocalPlayer);
            graybeardRole.LastMeeting = DateTime.UtcNow;
            graybeardRole.MeetingInProgress = false;
            graybeardRole.CanDie = true;

            if (graybeardRole.TasksLeft == 0) return;

            Coroutines.Start(DeathTimer.FrameTimer());
        }
    }
}