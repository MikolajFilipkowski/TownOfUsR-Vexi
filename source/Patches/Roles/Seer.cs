using System;
using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Seer : Role
    {
        public List<byte> Investigated = new List<byte>();
        public List<KeyValuePair<byte, Color>> InsaneInvestigated = new List<KeyValuePair<byte, Color>>();

        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            ImpostorText = () => "Reveal The Alliance Of Other Players";
            TaskText = () => "Reveal alliances of other players to find the Impostors";
            Color = Patches.Colors.Seer;
            LastInvestigated = DateTime.UtcNow;
            RoleType = RoleEnum.Seer;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.SeerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}