using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TrapperMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
            if (!(role.TrapTimer() == 0f)) return false;
            if (!__instance.enabled) return false;
            if (!role.ButtonUsable) return false;
            role.UsesLeft--;
            role.LastTrapped = System.DateTime.UtcNow;
            role.traps.Add(TrapExtentions.CreateTrap(PlayerControl.LocalPlayer.GetTruePosition()));

            return false;
        }
    }
}
