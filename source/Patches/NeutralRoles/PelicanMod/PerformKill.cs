using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.PelicanMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pelican) && __instance.isActiveAndEnabled &&
                !__instance.isCoolingDown)
                return Role.GetRole<Pelican>(PlayerControl.LocalPlayer).UseAbility(__instance);

            return true;
        }
    }
}