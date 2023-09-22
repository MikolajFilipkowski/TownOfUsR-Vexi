using System.Linq;
using HarmonyLib;
using InnerNet;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.PelicanMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal class Update
    {
        private static void Postfix(HudManager __instance)
        {
            var pelican = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pelican);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (pelican != null)
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Pelican))
                        Role.GetRole<Pelican>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }
}