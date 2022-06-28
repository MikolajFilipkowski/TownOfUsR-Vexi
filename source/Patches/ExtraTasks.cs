using HarmonyLib;

namespace TownOfUs.Patches
{

    [HarmonyPatch(typeof(ShipStatus))]
    public class ShipStatusPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.IsGameOverDueToDeath))]
        public static void Postfix(ShipStatus __instance, ref bool __result)
        {
            __result = false;
        }

        private static int CommonTasks = PlayerControl.GameOptions.NumCommonTasks;
        private static int ShortTasks = PlayerControl.GameOptions.NumShortTasks;
        private static int LongTasks = PlayerControl.GameOptions.NumLongTasks;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool Prefix(ShipStatus __instance)
        {
            var commonTask = __instance.CommonTasks.Count;
            var normalTask = __instance.NormalTasks.Count;
            var longTask = __instance.LongTasks.Count;
            CommonTasks = PlayerControl.GameOptions.NumCommonTasks;
            ShortTasks = PlayerControl.GameOptions.NumShortTasks;
            LongTasks = PlayerControl.GameOptions.NumLongTasks;
            if (PlayerControl.GameOptions.NumCommonTasks > commonTask) PlayerControl.GameOptions.NumCommonTasks = commonTask;
            if (PlayerControl.GameOptions.NumShortTasks > normalTask) PlayerControl.GameOptions.NumShortTasks = normalTask;
            if (PlayerControl.GameOptions.NumLongTasks > longTask) PlayerControl.GameOptions.NumLongTasks = longTask;
            return true;
            // Common: 2/2/4/x/2/3
            // Short: 19/13/14/x/26/15
            // Long: 8/12/15/x/15/15
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static void Postfix2(ShipStatus __instance)
        {
            PlayerControl.GameOptions.NumCommonTasks = CommonTasks;
            PlayerControl.GameOptions.NumShortTasks = ShortTasks;
            PlayerControl.GameOptions.NumLongTasks = LongTasks;
        }
    }
}
