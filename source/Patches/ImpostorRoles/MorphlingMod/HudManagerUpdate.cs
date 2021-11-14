using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.MorphlingMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite SampleSprite => TownOfUs.SampleSprite;
        public static Sprite MorphSprite => TownOfUs.MorphSprite;


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Morphling)) return;
            var role = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
            if (role.MorphButton == null)
            {
                role.MorphButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.MorphButton.graphic.enabled = true;
                role.MorphButton.graphic.sprite = SampleSprite;
            }

            if (role.MorphButton.graphic.sprite != SampleSprite && role.MorphButton.graphic.sprite != MorphSprite)
                role.MorphButton.graphic.sprite = SampleSprite;

            role.MorphButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            var position = __instance.KillButton.transform.localPosition;
            role.MorphButton.transform.localPosition = new Vector3(position.x,
                __instance.ReportButton.transform.localPosition.y, position.z);
            if (role.MorphButton.graphic.sprite == SampleSprite)
            {
                role.MorphButton.SetCoolDown(0f, 1f);
                Utils.SetTarget(ref role.ClosestPlayer, role.MorphButton);
            }
            else
            {
                if (role.Morphed)
                {
                    role.MorphButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.MorphlingDuration);
                    return;
                }

                role.MorphButton.SetCoolDown(role.MorphTimer(), CustomGameOptions.MorphlingCd);
                role.MorphButton.graphic.color = Palette.EnabledColor;
                role.MorphButton.graphic.material.SetFloat("_Desat", 0f);
            }
        }
    }
}
