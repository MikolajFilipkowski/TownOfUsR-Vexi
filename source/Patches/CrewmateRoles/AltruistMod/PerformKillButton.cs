using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.CustomOption;
using Reactor.Networking.Rpc;
using static UnityEngine.GraphicsBuffer;

namespace TownOfUs.CrewmateRoles.AltruistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Altruist);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Altruist>(PlayerControl.LocalPlayer);

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            if (player.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }

            if(role.Player.Is(ModifierEnum.Insane))
            {
                switch(CustomGameOptions.InsaneAltruistRevive)
                {
                    case Patches.Roles.Modifiers.AltruistRevive.Dies:
                        Utils.RpcMurderPlayer(role.Player, role.Player);
                        break;
                    case Patches.Roles.Modifiers.AltruistRevive.Report:
                        Utils.Rpc(CustomRPC.BaitReport, role.Player.PlayerId, player.PlayerId);
                        break;
                    case Patches.Roles.Modifiers.AltruistRevive.DiesAndReport:
                        Utils.Rpc(CustomRPC.BaitReport, role.Player.PlayerId, player.PlayerId);
                        Utils.RpcMurderPlayer(role.Player, role.Player);
                        break;
                }
                return false;
            }

            Utils.Rpc(CustomRPC.AltruistRevive, PlayerControl.LocalPlayer.PlayerId, playerId);

            Coroutines.Start(Coroutine.AltruistRevive(role.CurrentTarget, role));
            return false;
        }
    }
}