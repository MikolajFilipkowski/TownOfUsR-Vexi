using HarmonyLib;
using System.Linq;
using Hazel;
using UnityEngine;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.TraitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class RepickTraitor
    {
        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer != SetTraitor.WillBeTraitor) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Traitor)) return;
            if (!PlayerControl.LocalPlayer.Data.IsDead) return;
            var exeTarget = PlayerControl.LocalPlayer;
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exeRole = (Executioner)role;
                exeTarget = exeRole.target;
            }
            var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ModifierEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected && x != exeTarget).ToList();
            if (toChooseFrom.Count == 0) return;
            var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
            var pc = toChooseFrom[rand];

            SetTraitor.WillBeTraitor = pc;

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetTraitor, SendOption.Reliable, -1);
            writer.Write(pc.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            return;
        }
    }
}