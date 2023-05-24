using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System;

namespace TownOfUs.NeutralRoles.VultureMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton

    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vulture);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Vulture>(PlayerControl.LocalPlayer);

            if (__instance == role.EatButton)
            {
                var flag2 = __instance.isCoolingDown;
                if (flag2) return false;
                if (!__instance.enabled) return false;
                if (role.EatTimer() != 0) return false;
                var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }

                role.LastAte = DateTime.UtcNow;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.VultureEat, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(playerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                Coroutines.Start(Coroutine.CleanCoroutine(role.CurrentTarget, role));
                return false;
            }

            return true;
        }
    }
}