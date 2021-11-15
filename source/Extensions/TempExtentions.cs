using System;
using System.Collections.Generic;
using System.Text;
using TownOfUs.Extensions;

namespace UnityEngine
{
    public static class TempExtentions
    {
        public static bool IsImpostor(this GameData.PlayerInfo playerinfo)
        {
            return playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;
        }

        public static void SetImpostor(this GameData.PlayerInfo playerinfo, bool impostor)
        {
            if (playerinfo.Role != null)
                playerinfo.Role.TeamType = impostor ? RoleTeamTypes.Impostor : RoleTeamTypes.Crewmate;
        }


        public static string GetHatIdByNr(this HatManager hatManager, uint id)
        {
            return id < hatManager.AllHats.Count ? hatManager.AllHats[(int)id].ProdId : hatManager.AllHats[0].ProdId;
        }
        public static string GetSkinIdByNr(this HatManager hatManager, uint id)
        {
            return id < hatManager.AllSkins.Count ? hatManager.AllSkins[(int)id].ProdId : hatManager.AllSkins[0].ProdId;
        }
        public static string GetPetIdByNr(this HatManager hatManager, uint id)
        {
            return id < hatManager.AllPets.Count ? hatManager.AllPets[(int)id].ProdId : hatManager.AllPets[0].ProdId;
        }

        public static void SetHat(this HatParent hatParent, uint hatId, int colorId)
        {
            hatParent.SetHat(HatManager.Instance.GetHatIdByNr(hatId), colorId);
        }

        public static void SetSkin(this PlayerPhysics playerPhysics, uint skinId)
        {
            playerPhysics.SetSkin(HatManager.Instance.GetSkinIdByNr(skinId));

        }
        public static void RawSetPet(this PlayerControl playerPhysics, uint petId, int colorId)
        {
            playerPhysics.RawSetPet(HatManager.Instance.GetPetIdByNr(petId), colorId);
        }
        public static uint GetIdFromHat(this HatManager hatManager, HatBehaviour hat) => (uint)hatManager.AllHats.IndexOf(hat);


        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl)
        {
            return playerControl.Data.DefaultOutfit;
        }

   
        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType, GameData.PlayerOutfit outfit)
        {
            playerControl.Data.SetOutfit((PlayerOutfitType)CustomOutfitType, outfit);
            playerControl.SetOutfit(CustomOutfitType);
        }
        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            var outfitType = (PlayerOutfitType)CustomOutfitType;
            if (!playerControl.Data.Outfits.ContainsKey(outfitType))
            {
                return;
            }
            var newOutfit = playerControl.Data.Outfits[outfitType];
            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId);
            playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            if(playerControl?.MyPhysics?.Skin?.skin?.ProdId != newOutfit.SkinId)
            playerControl.RawSetSkin(newOutfit.SkinId);

        }


        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl)
        {
            return (CustomPlayerOutfitType)playerControl.CurrentOutfitType;
        }

    }


}
