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
        public static List<RoleEnum> InsaneRoles 
        { 
            get 
            {
                List<RoleEnum> _rolesToReturn = new List<RoleEnum>();

                if (CustomGameOptions.InsaneDetective)
                    _rolesToReturn.Add(RoleEnum.Detective);
                if (CustomGameOptions.InsaneSeer)
                    _rolesToReturn.Add(RoleEnum.Seer);
                if (CustomGameOptions.InsaneSnitch)
                    _rolesToReturn.Add(RoleEnum.Snitch);
                if (CustomGameOptions.InsaneTrapper)
                    _rolesToReturn.Add(RoleEnum.Trapper);
                if (CustomGameOptions.InsaneMystic)
                    _rolesToReturn.Add(RoleEnum.Mystic);
                if (CustomGameOptions.InsaneAurial)
                    _rolesToReturn.Add(RoleEnum.Aurial);
                if (CustomGameOptions.InsaneOracle)
                    _rolesToReturn.Add(RoleEnum.Oracle);
                if (CustomGameOptions.InsaneMedic)
                    _rolesToReturn.Add(RoleEnum.Medic);
                if (CustomGameOptions.InsaneAltruist)
                    _rolesToReturn.Add(RoleEnum.Altruist);
                if (CustomGameOptions.InsaneSwapper)
                    _rolesToReturn.Add(RoleEnum.Swapper);
                if (CustomGameOptions.InsaneTransporter)
                    _rolesToReturn.Add(RoleEnum.Transporter);
                if (CustomGameOptions.InsaneProsecutor)
                    _rolesToReturn.Add(RoleEnum.Prosecutor);
                if (CustomGameOptions.InsaneGuardianAngel)
                    _rolesToReturn.Add(RoleEnum.GuardianAngel);
                if (CustomGameOptions.InsaneJester)
                    _rolesToReturn.Add(RoleEnum.Jester);

                return _rolesToReturn;
            } 
        }

        public Insane(PlayerControl player) : base(player)
        {
            Name = "Insane";
            TaskText = () => "You can't trust yourself, nor your abilities";
            Color = Patches.Colors.Insane;
            ModifierType = ModifierEnum.Insane;
            IsHidden = true;
        }
    }

    public enum SeerSees
    {
        Random = 0,
        Opposite = 1
    }

    public enum AltruistRevive
    {
        DiesAndReport = 0,
        Dies = 1,
        Report = 2
    }
}
