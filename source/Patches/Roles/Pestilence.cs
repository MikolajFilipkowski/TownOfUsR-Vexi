using Hazel;
using System;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Pestilence : Role
    {
        public Pestilence(PlayerControl owner) : base(owner)
        {
            Name = "Pestilence";
            Color = Patches.Colors.Pestilence;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Pestilence;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "";
            TaskText = () => "Kill everyone with your unstoppable abilities!\nFake Tasks:";
            Faction = Faction.NeutralKilling;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool PestilenceWins { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(RoleEnum.Glitch) || x.Is(RoleEnum.Arsonist) ||
                    x.Is(RoleEnum.Juggernaut) || x.Is(RoleEnum.Werewolf) || x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pelican))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PestilenceWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            PestilenceWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.PestKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}