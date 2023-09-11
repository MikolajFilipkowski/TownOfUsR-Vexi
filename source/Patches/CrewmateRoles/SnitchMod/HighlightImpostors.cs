using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SnitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        private static void UpdateMeeting(MeetingHud __instance)
        {
            if(PlayerControl.LocalPlayer.Is(ModifierEnum.Insane))
            {
                var role = (Snitch)Role.GetRole(PlayerControl.LocalPlayer);
                foreach(var state in __instance.playerStates)
                {
                    if (!role.InsaneImpostors.Contains(state.TargetPlayerId)) 
                        continue;

                    state.NameText.color = Palette.ImpostorRed;
                }
                return;
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = Role.GetRole(player);
                    if (player.Is(Faction.Impostors) && !player.Is(RoleEnum.Traitor))
                        state.NameText.color = Palette.ImpostorRed;
                    else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor)
                        state.NameText.color = Palette.ImpostorRed;
                    if (player.Is(Faction.NeutralKilling) && CustomGameOptions.SnitchSeesNeutrals)
                        state.NameText.color = role.Color;
                }
            }
        }

        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Snitch)) return;
            var role = Role.GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (!role.TasksDone) return;
            if (MeetingHud.Instance && CustomGameOptions.SnitchSeesImpInMeeting) UpdateMeeting(MeetingHud.Instance);

            if(PlayerControl.LocalPlayer.Is(ModifierEnum.Insane))
            {
                foreach(var playerId in role.InsaneImpostors)
                {
                    var player = Utils.PlayerById(playerId);
                    player.nameText().color = Palette.ImpostorRed;
                }
                return;
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsImpostor() && !player.Is(RoleEnum.Traitor)) player.nameText().color = Palette.ImpostorRed;
                else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.SnitchSeesTraitor) player.nameText().color = Palette.ImpostorRed;
                var playerRole = Role.GetRole(player);
                if (playerRole.Faction == Faction.NeutralKilling && CustomGameOptions.SnitchSeesNeutrals)
                    player.nameText().color = playerRole.Color;
            }
        }
    }
}