using HarmonyLib;
using Reactor.Utilities.Extensions;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.AurialMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Aurial)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
            if (!(role.RadiateTimer() == 0f)) return false;
            if (!__instance.enabled) return false;
            role.LastRadiated = System.DateTime.UtcNow;

            var players = Utils.GetClosestPlayers(PlayerControl.LocalPlayer.GetTruePosition(), CustomGameOptions.RadiateRange, false);
            foreach ( var player in players)
            {
                if (UnityEngine.Random.Range(0, 100) > CustomGameOptions.RadiateChance) continue;

                if(role.Player.Is(ModifierEnum.Insane))
                {
                    if(!role.InsaneKnownRoles.Any(x => x.Key == player.PlayerId))
                    {
                        var pRole = Role.GetRole(player);
                        Color newColor;

                        switch(CustomGameOptions.InsaneAurialAbility)
                        {
                            case Patches.Roles.Modifiers.SeerSees.Opposite:
                                if (pRole.Faction == Faction.NeutralKilling || pRole.Faction == Faction.NeutralBenign || pRole.Faction == Faction.NeutralEvil || pRole.Faction == Faction.Impostors)
                                    newColor = Color.green;
                                else
                                    newColor = Color.red;
                                break;
                            default:
                                Color[] possibleColors = new Color[] { Color.white, Color.green, Color.red, Color.gray };
                                newColor = possibleColors.Random();
                                break;
                        }

                        role.InsaneKnownRoles.Add(new System.Collections.Generic.KeyValuePair<byte, Color>(player.PlayerId, newColor));
                    }
                }

                if (role.knownPlayerRoles.TryGetValue(player.PlayerId, out int val)) {
                    role.knownPlayerRoles[player.PlayerId] = val + 1;
                } else role.knownPlayerRoles.Add(player.PlayerId, 1);
            }

            return false;
        }
    }
}