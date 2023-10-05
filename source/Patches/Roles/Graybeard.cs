using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
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

        public Dictionary<PlayerControl, DateTime> trappedPlayers;

        public int TimeToDeath;

        public bool ButtonUsable;
        public bool MeetingInProgress;
        public bool CanDie;
        public Graybeard(PlayerControl player) : base(player)
        {
            Name = "Graybeard";
            ImpostorText = () => "You have a weak, loving heart";
            TaskText = () => "Place force fields and do tasks to stay alive";
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
