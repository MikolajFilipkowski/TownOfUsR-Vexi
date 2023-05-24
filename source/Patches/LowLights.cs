using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player,
            ref float __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (GameOptionsManager.Instance.currentHideNSeekGameOptions.useFlashlight)
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorFlashlightSize;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewmateFlashlightSize;
                }
                else
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorLightMod;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewLightMod;
                }
                return false;
            }

            if (player == null || player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }

            var switchSystem = __instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            if (player.IsImpostor() || player._object.Is(RoleEnum.Glitch) ||
                player._object.Is(RoleEnum.Juggernaut) || player._object.Is(RoleEnum.Pestilence) ||
                (player._object.Is(RoleEnum.Jester) && CustomGameOptions.JesterImpVision) ||
                (player._object.Is(RoleEnum.Vulture) && CustomGameOptions.VultureImpVision) ||
                (player._object.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoImpVision))
            {
                __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                return false;
            }
            else if (player._object.Is(RoleEnum.Werewolf))
            {
                var role = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                if (role.Rampaged)
                {
                    __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod;
                    return false;
                }
            }

            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (player._object.Is(ModifierEnum.Torch)) __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
                return false;
            }

            var t = switchSystem.Value / 255f;

            if (player._object.Is(ModifierEnum.Torch)) t = 1;
            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod;
            return false;
        }
    }
}