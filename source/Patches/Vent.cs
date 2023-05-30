using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using InnerNet;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HudManager))]
    public static class HudManagerVentPatch
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if(__instance.ImpostorVentButton == null || __instance.ImpostorVentButton.gameObject == null || __instance.ImpostorVentButton.IsNullOrDestroyed())
                return;

            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Joined) return;

            bool active = PlayerControl.LocalPlayer != null && VentPatches.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer._cachedData) && !MeetingHud.Instance;
            if(active != __instance.ImpostorVentButton.gameObject.active)
            __instance.ImpostorVentButton.gameObject.SetActive(active);
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.CanUse))]
    public static class VentPatches
    {
        public static bool CanVent(PlayerControl player, GameData.PlayerInfo playerInfo)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return false;

            if (player.inVent)
                return true;

            if (playerInfo.IsDead)
                return false;
            if (player.IsDevoured())
                return false;

            if (CustomGameOptions.GameMode == GameMode.Cultist && !player.Is(RoleEnum.Engineer)) return false;
            else if (CustomGameOptions.GameMode == GameMode.Cultist && player.Is(RoleEnum.Engineer)) return true;

            if (player.Is(RoleEnum.Morphling) && !CustomGameOptions.MorphlingVent
                || player.Is(RoleEnum.Swooper) && !CustomGameOptions.SwooperVent
                || player.Is(RoleEnum.Grenadier) && !CustomGameOptions.GrenadierVent
                || player.Is(RoleEnum.Undertaker) && !CustomGameOptions.UndertakerVent
                || player.Is(RoleEnum.Escapist) && !CustomGameOptions.EscapistVent
                || player.Is(RoleEnum.Bomber) && !CustomGameOptions.BomberVent
                || (player.Is(RoleEnum.Undertaker) && Role.GetRole<Undertaker>(player).CurrentlyDragging != null && !CustomGameOptions.UndertakerVentWithBody))
                return false;

            if (player.Is(RoleEnum.Engineer) ||
                (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) || (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) ||
                (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) || (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) ||
                (player.Is(RoleEnum.Vulture) && CustomGameOptions.VultureVent))
                return true;

            if (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent)
            {
                var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                if (role.Rampaged) return true;
            }

            return playerInfo.IsImpostor();
        }

        public static void Postfix(Vent __instance,
            [HarmonyArgument(0)] GameData.PlayerInfo playerInfo,
            [HarmonyArgument(1)] ref bool canUse,
            [HarmonyArgument(2)] ref bool couldUse,
            ref float __result)
        {
            float num = float.MaxValue;
            PlayerControl playerControl = playerInfo.Object;
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) couldUse = CanVent(playerControl, playerInfo) && !playerControl.MustCleanVent(__instance.Id) && (!playerInfo.IsDead || playerControl.inVent) && (playerControl.CanMove || playerControl.inVent);
            else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek && playerControl.Data.IsImpostor()) couldUse = false;
            else couldUse = canUse;

            var ventitaltionSystem = ShipStatus.Instance.Systems[SystemTypes.Ventilation].Cast<VentilationSystem>();
            if (ventitaltionSystem != null && ventitaltionSystem.PlayersCleaningVents != null)
            {
                foreach (var item in ventitaltionSystem.PlayersCleaningVents.Values)
                {
                    if (item == __instance.Id)
                        couldUse = false;
                }
            }

            canUse = couldUse;

            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (Patches.SubmergedCompatibility.getInTransition())
                {
                    __result = float.MaxValue;
                    return;
                }
                switch (__instance.Id)
                {
                    case 9:  //Engine Room Exit Only Vent
                        if (PlayerControl.LocalPlayer.inVent) break;
                        __result = float.MaxValue;
                        return;
                    case 14: // Lower Central
                        __result = float.MaxValue;
                        if (canUse)
                        {
                            Vector3 center = playerControl.Collider.bounds.center;
                            Vector3 position = __instance.transform.position;
                            __result = Vector2.Distance(center, position);
                            canUse &= __result <= __instance.UsableDistance;
                        }
                        return;
                }
            }

            if (canUse)
            {
                Vector3 center = playerControl.Collider.bounds.center;
                Vector3 position = __instance.transform.position;
                num = Vector2.Distance((Vector2)center, (Vector2)position);
                canUse = ((canUse ? 1 : 0) & ((double)num > (double)__instance.UsableDistance ? 0 : (!PhysicsHelpers.AnythingBetween(playerControl.Collider, (Vector2)center, (Vector2)position, Constants.ShipOnlyMask, false) ? 1 : 0))) != 0;
            }

            __result = num;
        }
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.SetButtons))]
    public static class JesterEnterVent
    {
        public static bool Prefix(Vent __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent)
                return false;
            return true;
        }
    }
}