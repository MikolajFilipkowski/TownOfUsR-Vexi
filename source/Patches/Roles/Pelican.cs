using System;
using System.Linq;
using Hazel;
using UnityEngine;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Pelican : Role
    {
        private KillButton _devourButton;
        public bool Enabled;
        public bool PelicanWins;
        public PlayerControl ClosestPlayer;
        public DateTime LastDevoured;
        public float TimeRemaining;


        public Pelican(PlayerControl player) : base(player)
        {
            Name = "Pelican";
            ImpostorText = () => "Rampage To Kill Everyone";
            TaskText = () => "Rampage to kill everyone\nFake Tasks:";
            Color = Patches.Colors.Pelican;
            LastDevoured = DateTime.UtcNow;
            RoleType = RoleEnum.Pelican;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public KillButton DevourButton
        {
            get => _devourButton;
            set
            {
                _devourButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(RoleEnum.Glitch) || x.Is(RoleEnum.Arsonist) ||
                    x.Is(RoleEnum.Juggernaut) || x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence) || x.Is(RoleEnum.Werewolf))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.PelicanWin,
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

        public void Devour(PlayerControl devouredPlayer)
        {
            devouredPlayer.SetOutfit(CustomPlayerOutfitType.Swooper, new GameData.PlayerOutfit()
            {
                ColorId = Player.CurrentOutfit.ColorId,
                HatId = "",
                SkinId = "",
                VisorId = "",
                PlayerName = " "
            });
            devouredPlayer.myRend().color = Color.clear;
            devouredPlayer.nameText().color = Color.clear;
            devouredPlayer.cosmetics.colorBlindText.color = Color.clear;

        }

        public void Wins()
        {
            PelicanWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var pelicanTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            pelicanTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = pelicanTeam;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDevoured;
            var num = CustomGameOptions.RampageKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
