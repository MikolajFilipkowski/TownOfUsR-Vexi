using HarmonyLib;
using TownOfUs.Patches.Roles.Modifiers;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using Object = UnityEngine.Object;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            Insane.MeetingInProgress = false;
            Utils.ResetCustomTimers();
        }
    }
}