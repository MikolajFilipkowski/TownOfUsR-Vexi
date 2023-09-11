using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System.Collections.Generic;
using TownOfUs.Patches.Roles.Modifiers;
using Reactor.Utilities.Extensions;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Seer);
            if (!flag) return true;
            var role = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.SeerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if(PlayerControl.LocalPlayer.Is(ModifierEnum.Insane))
                {
                    if(CustomGameOptions.InsaneSeerAbility == SeerSees.Random)
                    {
                        Color[] randomColor = new Color[] { Color.green, Color.red };
                        role.InsaneInvestigated.Add(new KeyValuePair<byte, Color>(role.ClosestPlayer.PlayerId, randomColor.Random()));
                    }
                    else if(CustomGameOptions.InsaneSeerAbility == SeerSees.Opposite)
                    {
                        Color newColor;
                        PlayerControl player = role.ClosestPlayer;
                        if ((player.Is(Faction.Crewmates) && !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.VampireHunter))) ||
                        ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.VampireHunter)) && !CustomGameOptions.CrewKillingRed) ||
                        (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                        (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                        (player.Is(Faction.NeutralKilling) && !CustomGameOptions.NeutKillingRed))
                        {
                            newColor = Color.red;
                        }
                        else
                            newColor = Color.green;

                        role.InsaneInvestigated.Add(new KeyValuePair<byte, Color>(role.ClosestPlayer.PlayerId, newColor));
                    }
                }
                else
                    role.Investigated.Add(role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastInvestigated = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastInvestigated = DateTime.UtcNow;
                role.LastInvestigated = role.LastInvestigated.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.SeerCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
