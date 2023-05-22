using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Reactor.Utilities;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class GameManagerPatchs
    {
        static void Postfix(GameStartManager __instance)
        {
            if (CustomGameOptions.DebugMode) __instance.MinPlayers = 1;
        }
    }

    [HarmonyPatch]
    public static class IntrodCutScenePatch
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameEnd))]
        public static bool Prefix()
        {
            if (CustomGameOptions.DebugMode) return false;
            return true;
        }
    }


    /*[HarmonyPatch(typeof(PlayerControl), nameof(ChatController.SendChat))]
    public class Forceclass
    {
        [HarmonyPostfix]
        public static void Postfix([HarmonyArgument(0)] String msg)
        {
            PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"{PlayerControl.LocalPlayer.name} sent: {msg}");
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, $"{PlayerControl.LocalPlayer.name} sent: {msg}");
        }
    }*/

    public class AddBot
    {
        public void Add()
        {

        }
    }

    public class SendChat {
        public static void SendLocalMessage(string message)
        {
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
        }
    }
}
