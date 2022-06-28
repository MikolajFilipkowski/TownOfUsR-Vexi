using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.TrapperMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HudTrap
    {
        public static void Postfix(PlayerControl __instance)
        {
            UpdateTrapButton(__instance);
        }

        public static void UpdateTrapButton(PlayerControl __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return;
            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var trapButton = DestroyableSingleton<HudManager>.Instance.KillButton;

            var role = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(trapButton.cooldownTimerText, trapButton.transform);
                role.UsesText.gameObject.SetActive(true);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }

            if (isDead)
            {
                trapButton.gameObject.SetActive(false);
            }
            else
            {
                trapButton.gameObject.SetActive(!MeetingHud.Instance);
                if (role.ButtonUsable)
                {
                    trapButton.SetCoolDown(role.TrapTimer(), CustomGameOptions.TrapCooldown);
                }
                
            }


            var renderer = trapButton.graphic;
            if (!trapButton.isCoolingDown && trapButton.gameObject.active && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}
