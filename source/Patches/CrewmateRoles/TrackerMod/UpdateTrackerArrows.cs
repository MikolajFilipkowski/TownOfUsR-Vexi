using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.ImpostorRoles.CamouflageMod;
using TownOfUs.Extensions;

namespace TownOfUs.CrewmateRoles.TrackerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateTrackerArrows
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        private static float _time = 0f;
        private static float Interval => CustomGameOptions.UpdateInterval*10;
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Tracker))
            {
                var tracker = (Tracker) role;
                if (PlayerControl.LocalPlayer.Data.IsDead || tracker.Player.Data.IsDead)
                {
                    tracker.TrackerArrows.DestroyAll();
                    tracker.TrackerArrows.Clear();
                    return;
                }
                else
                {
                    _time += Time.deltaTime;
                    if (_time >= Interval)
                    {
                        _time -= Interval;
                        var role2 = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                        tracker.TrackerArrows.DestroyAll();
                        tracker.TrackerArrows.Clear();
                        tracker.TrackerArrows.RemoveRange(0, tracker.TrackerArrows.Count);
                        tracker.TrackerTargets.RemoveRange(0, tracker.TrackerTargets.Count);
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            if (!role2.Tracked.Contains(player.PlayerId)) continue;
                            if (player.Data.IsDead) continue;
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = Sprite;
                            if (!CamouflageUnCamouflage.IsCamoed)
                            {
                                if (RainbowUtils.IsRainbow(player.GetDefaultOutfit().ColorId))
                                {
                                    renderer.color = RainbowUtils.Rainbow;
                                }
                                else
                                {
                                    renderer.color = Palette.PlayerColors[player.GetDefaultOutfit().ColorId];
                                }
                            }
                            else
                            {
                                renderer.color = new Color(0.2f, 0.2f, 0.2f, 1f);
                            }
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            role2.TrackerArrows.Add(arrow);
                            role2.TrackerTargets.Add(player);
                        }
                        foreach (var (arrow, target) in Utils.Zip(tracker.TrackerArrows, tracker.TrackerTargets))
                        {
                            arrow.target = target.transform.position;
                        }
                    }
                }
            }
        }
    }
}