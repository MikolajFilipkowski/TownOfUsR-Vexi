using System;
using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.Modifiers.LoversMod
{
    public static class ChatPatch
    {
        private static DateTime MeetingStartTime = DateTime.MinValue;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingStart
        {
            public static void Prefix(MeetingHud __instance)
            {
                MeetingStartTime = DateTime.UtcNow;
            }
        }
        [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
        public static class AddChat
        {
            public static bool Prefix(ChatController __instance, [HarmonyArgument(0)] PlayerControl sourcePlayer)
            {
                if (__instance != HudManager.Instance.Chat) return true;
                var localPlayer = PlayerControl.LocalPlayer;
                if (localPlayer == null) return true;
                Boolean shouldSeeMessage = localPlayer.Data.IsDead || 
                    (localPlayer.IsLover() && sourcePlayer.IsLover()) ||
                    ((localPlayer.Is(RoleEnum.Pelican) || localPlayer.IsDevoured()) && (sourcePlayer.Is(RoleEnum.Pelican) || sourcePlayer.IsDevoured())) ||
                    (sourcePlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId);
                if (DateTime.UtcNow - MeetingStartTime < TimeSpan.FromSeconds(1))
                {
                    return shouldSeeMessage;
                }
                return MeetingHud.Instance != null || LobbyBehaviour.Instance != null || shouldSeeMessage;
            }
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public static class EnableChat
        {
            public static void Postfix(HudManager __instance)
            {
                if ((PlayerControl.LocalPlayer.IsLover() || PlayerControl.LocalPlayer.IsDevoured() || PlayerControl.LocalPlayer.Is(RoleEnum.Pelican))
                    & !__instance.Chat.isActiveAndEnabled)
                    __instance.Chat.SetVisible(true);
            }
        }
    }
}