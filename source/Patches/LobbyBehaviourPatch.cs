using HarmonyLib;
using UnityEngine;


namespace TownOfUs.Patches {
    
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch {
        [HarmonyPostfix]
        public static void Postfix() {

            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = false;
            //((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = false;

            // Apologies, but this fixes broken load in screen. Need to change how grenadier flash works anyway
        }
    }
}
