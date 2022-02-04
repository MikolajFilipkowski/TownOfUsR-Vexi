using System;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Tracker : Role
    {
        public List<ArrowBehaviour> TrackerArrows = new List<ArrowBehaviour>();
        public List<PlayerControl> TrackerTargets = new List<PlayerControl>();
        public PlayerControl ClosestPlayer;
        public List<byte> Tracked = new List<byte>();
        public DateTime LastTracked { get; set; }
        public int RemainingTracks { get; set; }

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            ImpostorText = () => "Track everyone's movement";
            TaskText = () => "Track suspicious players";
            Color = Patches.Colors.Tracker;
            RoleType = RoleEnum.Tracker;

            RemainingTracks = CustomGameOptions.MaxTracks;
        }
        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = CustomGameOptions.TrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}