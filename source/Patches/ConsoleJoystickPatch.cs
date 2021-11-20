using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(ConsoleJoystick), nameof(ConsoleJoystick.HandleHUD))]
    public static class ConsoleJoystickPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (DestroyableSingleton<HudManager>.Instance != null && DestroyableSingleton<HudManager>.Instance.ImpostorVentButton != null && DestroyableSingleton<HudManager>.Instance.isActiveAndEnabled && ConsoleJoystick.player.GetButtonDown(50))
                DestroyableSingleton<HudManager>.Instance.ImpostorVentButton.DoClick();
        }
    }
}
