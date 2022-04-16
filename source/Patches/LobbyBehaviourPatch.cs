using HarmonyLib;
using UnityEngine;


namespace TownOfUs.Patches {
    
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    static class LobbyBehaviourPatch {
        [HarmonyPostfix]
        public static void Postfix() {

            ((Renderer)DestroyableSingleton<HudManager>.Instance.FullScreen).gameObject.active = false;
        }
    }
}
