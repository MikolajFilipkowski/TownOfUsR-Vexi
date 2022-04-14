using HarmonyLib;
using UnityEngine;

namespace TownOfUs.Patches {
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch {
        [HarmonyPostfix]
        public static void Postfix() {
            // Fix Grenadier blind in lobby
            
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = false;

            // Apologies, but this fixes broken load in screen. I think the CutSceneIntro object is a child of the HudManager object, so when HudManager is made non-fullscreen it freaks out and breaks
            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).enabled = true;
        }
    }
}
