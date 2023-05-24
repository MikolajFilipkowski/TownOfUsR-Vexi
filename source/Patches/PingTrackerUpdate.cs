using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTrackerPatch
    {
        public static void Postfix(PingTracker __instance)
        {
            AspectPosition position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(4f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text =
                $"Ping: {AmongUsClient.Instance.Ping} ms" +
                $"\n<size=50%><color=#FEA600><b>TownOfUsReborn v{TownOfUs.VersionString}</b></color> <color=red>(MODIFIED)</color></size>";

            if (!MeetingHud.Instance)
                __instance.text.text +=
                    "\n<size=30%><alpha=#99>Modified by: <b>Vexi & Tajemniczy Typiarz</b>";

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined)
                __instance.text.text +=
                    "\nReborn Version by: <b>Donners, Term, -H & MyDragonBreath</b>" +
                    "\nFormerly made by: <b>Slushiegoose & Polus.gg</b>";
        }
    }
}
