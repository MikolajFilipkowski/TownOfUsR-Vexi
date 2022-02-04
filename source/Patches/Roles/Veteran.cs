using System;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Veteran : Role
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;

        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            ImpostorText = () => "Alert to kill whoever interacts with you";
            TaskText = () => "You have " + RemainingAlerts + " alerts left";
            Color = Patches.Colors.Veteran;
            RoleType = RoleEnum.Veteran;

            RemainingAlerts = CustomGameOptions.MaxAlerts;
        }

        public bool OnAlert => TimeRemaining > 0f;

        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            ;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }

        public int RemainingAlerts { get; set; }
    }
}