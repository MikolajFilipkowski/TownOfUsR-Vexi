using System;
using System.Linq;
using Hazel;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Vulture : Role
    {
        public KillButton _eatButton;

        public bool VultureWins;

        public DateTime LastAte;
        public float TimeRemaining;

        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            ImpostorText = () => "Eat Bodies";
            TaskText = () => $"Eat {CustomGameOptions.VultureBodiesToEat} bodies to win";
            Color = Patches.Colors.Vulture;
            RoleType = RoleEnum.Vulture;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralOther;
        }

        public DeadBody CurrentTarget { get; set; }
        
        public KillButton EatButton
        {
            get => _eatButton;
            set
            {
                _eatButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public void CheckIfWon()
        {
            if (BodyEatten >= CustomGameOptions.VultureBodiesToEat)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.VultureWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
            }
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (!(BodyEatten >= CustomGameOptions.VultureBodiesToEat) || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.VultureWin,
                    SendOption.Reliable,
                    -1
                );
            writer.Write(Player.PlayerId);
            Wins();
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            VultureWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        public bool Ate => TimeRemaining > 0f;

        public float EatTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAte;
            var num = CustomGameOptions.VultureEatCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}