using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles;
using Reactor.Utilities;
using TownOfUs.Patches.Roles.Modifiers;

namespace TownOfUs.Patches.Modifiers.InsaneMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (__instance != PlayerControl.LocalPlayer) return;
            if (__instance.Data.IsDead) return;
            if (!__instance.Is(ModifierEnum.Insane)) return;

            var taskinfos = __instance.Data.Tasks.ToArray();
            int tasksLeft = taskinfos.Count(x => !x.Complete);

            if(tasksLeft < 1 && CustomGameOptions.InsaneRevealOnTasksDone && (CustomGameOptions.InsaneRevealsTo == RevealsTo.Self || CustomGameOptions.InsaneRevealsTo == RevealsTo.Everyone))
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Insane));
            }
        }
    }
}
