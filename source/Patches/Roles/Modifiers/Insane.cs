using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Patches.Roles.Modifiers
{
    public class Insane : Modifier
    {
        // Finish list based on custom game options
        /*
        public static List<RoleEnum> InsaneRoles 
        { 
            get 
            {
                List<RoleEnum> _rolesToReturn = new List<RoleEnum>();
                

            } 
        }
        */

        public Insane(PlayerControl player) : base(player)
        {
            Name = "Insane";
            TaskText = () => "";
            Color = Patches.Colors.Insane;
            ModifierType = ModifierEnum.Insane;
            IsHidden = true;
        }
    }
}
