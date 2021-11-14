using System;
using System.Collections.Generic;
using System.Text;

namespace UnityEngine
{
    public static class TempExtentions
    {
        public static bool IsImpostor(this GameData.PlayerInfo playerinfo)
        {
            return playerinfo.Role.TeamType == RoleTeamTypes.Impostor;
        }

        public static void SetImpostor(this GameData.PlayerInfo playerinfo, bool impostor)
        {
            playerinfo.Role.TeamType = impostor ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
        }

        public static void SetHat(this HatParent hatParent, int hatId, int colorId)
        {
        }

        public static void SetSkin(this PlayerPhysics playerPhysics, int skinId)
        {
        }
        public static void RawSetPet(this PlayerControl playerPhysics, int petId, int colorId)
        {
        }
        public static uint GetIdFromHat(this HatManager hatManager,  HatBehaviour hat) => (uint)hatManager.AllHats.IndexOf(hat);

    }


}
