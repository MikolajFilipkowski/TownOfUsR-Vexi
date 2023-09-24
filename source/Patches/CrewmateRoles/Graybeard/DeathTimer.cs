using Reactor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.GraybeardMod
{
    public class DeathTimer
    {
        public static IEnumerator FrameTimer()
        {
            while (!PlayerControl.LocalPlayer.Data.IsDead)
            {
                yield return 0;
                Update();
            }
        }

        public static void Update()
        {
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Graybeard)) return;

            Graybeard role = Role.GetRole<Graybeard>(PlayerControl.LocalPlayer);

            if (role.MeetingInProgress || role.TasksLeft == 0)
            {
                Coroutines.Stop(FrameTimer());
                return;
            }

            DateTime timeNow = DateTime.UtcNow;
            int timeElapsed = (int)Math.Floor((timeNow - role.LastMeeting).TotalSeconds);

            //PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"{timeElapsed}s / {role.TimeToDeath}s");

            if (timeElapsed>role.TimeToDeath)
            {
                Utils.RpcMurderPlayer(role.Player, role.Player);
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "You died of old age");
            }
        }
    }
}
