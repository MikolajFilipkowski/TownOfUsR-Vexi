using HarmonyLib;
using InnerNet;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPriority(Priority.VeryHigh)] // to show this message first, or be overrided if any plugins do
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTrackerPatch
    {
        public static List<PingColor> PingColors = new()
        {
            new(0, "#1f9900"),
            new(60, "#ffc800"),
            new(150, "#ff0000"),
            new(300, "#570b02")
        };

        public class PingColor
        {
            public PingColor(int ping, string color)
            {
                Ping = ping;
                Color = color;
            }
            public int Ping { get; set; }
            public string Color { get; set; }
        }

        public static string pingColorHandler() {
            int ping = AmongUsClient.Instance.Ping;
            string curCol = $"{ping}";
            foreach (PingColor color in PingColors) {
                if (color.Ping < ping) { 
                    curCol = $"<color={color.Color}>{ping}</color>";
                }
            }
            return curCol;
        }

        public static void Postfix(PingTracker __instance)
        {
            AspectPosition position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(5f, 0.1f, 0);
            position.AdjustPosition();

            __instance.text.text =
                $"Ping: {pingColorHandler()} ms" +
                $"\n<size=65%><color=#FEA600><b>TownOfUsR v{TownOfUs.VersionString}</b></color> <color=red>(MODIFIED)</color></size>";

            if (!MeetingHud.Instance)
                __instance.text.text +=
                    "\n<size=60%><alpha=#AA>Modified by: <b>Vexi & Tajemniczy Typiarz</b>";

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined)
                __instance.text.text +=
                    "\nReactivated by: <b>Donners, Term, -H & MyDragonBreath</b>" +
                    "\nFormerly made by: <b>Slushiegoose & Polus.gg</b>";
        }
    }
}
