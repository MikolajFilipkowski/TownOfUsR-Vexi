using HarmonyLib;
using Reactor;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(SaveManager), nameof(SaveManager.BodyColor), MethodType.Setter)]
    public class SaveManagerPatch
    {
        public static void Prefix(byte value)
        {
            if (value > 17)
                value = (byte)SaveManager.colorConfig;
            Logger<TownOfUs>.Instance.LogError($"Setting shit.. {value}");

        }
    }
}
