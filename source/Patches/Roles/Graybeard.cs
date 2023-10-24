using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using TownOfUs.CrewmateRoles.GraybeardMod;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfUs.Roles
{
    public class Graybeard : Role
    {
        public static Material trapMaterial = TownOfUs.bundledAssets.Get<Material>("trap");

        public List<Trap> traps = new List<Trap>();
        public DateTime LastTrapped { get; set; }
        public TextMeshPro UsesText;
        public DateTime LastSabotaged { get; set; }
        public DateTime LastMeeting { get; set; }

        public int PlayersInTrap { get; set; } = 0;

        public Dictionary<PlayerControl, DateTime> trappedPlayers;

        public int TimeToDeath;

        public bool ButtonUsable;
        public bool MeetingInProgress;
        public bool CanDie;
        public Graybeard(PlayerControl player) : base(player)
        {
            Name = "Graybeard";
            ImpostorText = () => "You have a weak, loving heart";
            TaskText = () => traps.Count == 0 ?
            $"Place force fields and do tasks to stay alive\nThe force field is currently inactive" :
            $"Place force fields and do tasks to stay alive\n Players in the force field: {PlayersInTrap}";
            Color = Patches.Colors.Graybeard;
            RoleType = RoleEnum.Graybeard;
            LastTrapped = DateTime.UtcNow;
            LastSabotaged = DateTime.UtcNow;
            LastMeeting = DateTime.UtcNow;
            CanDie = CustomGameOptions.GraybeardDiesBeforeFirstMeeting;
            trappedPlayers = new Dictionary<PlayerControl, DateTime>();
            Coroutines.Start(DeathTimer.FrameTimer());
            ButtonUsable = true;
            MeetingInProgress = false;
            TimeToDeath = CustomGameOptions.GraybeardTimeToDeath + Random.RandomRangeInt(-CustomGameOptions.GraybeardRandomizeTimeToDeath, CustomGameOptions.GraybeardRandomizeTimeToDeath+1);
            AddToRoleHistory(RoleType);
        }

        public float TrapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTrapped;
            var num = CustomGameOptions.TrapCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Sabotage()
        {
            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            var specials = system.specials.ToArray();
            if (!specials.Any(s => s.IsActive)) return;

            if ((DateTime.UtcNow-LastSabotaged).TotalMilliseconds < 70f)
            {
                return;
            }

            LastSabotaged = DateTime.UtcNow;

            if (!CanDie || TasksLeft==0) return;

            float taskFraction = (TotalTasks - TasksLeft) / TotalTasks;
            int deathChance = (int)Math.Floor(CustomGameOptions.SabotageDeathPercentage - (CustomGameOptions.SabotageDeathPercentage * taskFraction));
            int randomChance = Random.RandomRangeInt(1, 101);
            
            if (deathChance >= randomChance)
            {
                //PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"Zabito - {deathChance} | {randomChance}");
                Utils.RpcMurderPlayer(Player, Player);
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You had a heart attack");
            }
            /*else
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"Przezyl - {deathChance} | {randomChance}");
            }*/
            
        }
    }
}
