using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.RetributionistMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Retributionist))
            {
                var retributionist = Role.GetRole<Retributionist>(PlayerControl.LocalPlayer);
                ShowHideButtonsRetri.HideButtonsRetri(retributionist);
            }
        }
    }
}