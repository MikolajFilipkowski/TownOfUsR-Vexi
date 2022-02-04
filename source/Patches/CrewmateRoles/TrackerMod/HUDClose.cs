using System;
using HarmonyLib;
using TownOfUs.Roles;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var tracker = (Tracker) role;
                tracker.LastTracked = DateTime.UtcNow;
                tracker.RemainingTracks = CustomGameOptions.MaxTracks;
                if (CustomGameOptions.ResetOnNewRound)
                {
                    tracker.Tracked.RemoveRange(0, tracker.Tracked.Count);
                    tracker.TrackerArrows.DestroyAll();
                    tracker.TrackerArrows.Clear();
                    tracker.TrackerArrows.RemoveRange(0, tracker.TrackerArrows.Count);
                    tracker.TrackerTargets.RemoveRange(0, tracker.TrackerTargets.Count);
                }
            }
        }
    }
}