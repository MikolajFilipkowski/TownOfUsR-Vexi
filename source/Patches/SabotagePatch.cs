using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.RpcRepairSystem))]
    public static class SabotagePatch
    {
        public static void Postfix(SystemTypes systemType, int amount) {
            if (systemType==SystemTypes.Sabotage || systemType==SystemTypes.Comms)
            {
                Utils.Rpc(CustomRPC.SabotageOngoing);
            }
        }
    }
}
