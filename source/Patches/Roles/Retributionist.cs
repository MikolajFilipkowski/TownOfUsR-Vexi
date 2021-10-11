using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Retributionist : Role
    {
        public Dictionary<byte, (GameObject, GameObject, TMP_Text)> Buttons = new Dictionary<byte, (GameObject, GameObject, TMP_Text)>();


        public Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>
        {
            { "Impostor", Color.red },
            { "Janitor", Color.red },
            { "Morphling", Color.red },
            { "Camouflager", Color.red },
            { "Miner", Color.red },
            { "Swooper", Color.red },
            { "Undertaker", Color.red },
            { "Assassin", Color.red },
            { "Underdog", Color.red },
            { "Grenadier", Color.red },
            { "Loving Impostor", new Color(1f, 0.4f, 0.8f, 1f) }
        };

        public Dictionary<byte, string> Guesses = new Dictionary<byte, string>();


        public Retributionist(PlayerControl player) : base(player)
        {
            Name = "Retributionist";
            ImpostorText = () => "Kill impostors during meetings if you can guess their roles";
            TaskText = () => "Guess the roles of impostors mid-meeting to kill them!";
            Color = new Color(0.8f, 1f, 0f, 1f);
            RoleType = RoleEnum.Retributionist;

            RemainingKills = CustomGameOptions.RetributionistKills;

            if (CustomGameOptions.RetributionistGuessNeutrals)
            {
                ColorMapping.Add("Jester", new Color(1f, 0.75f, 0.8f, 1f));
                ColorMapping.Add("Shifter", new Color(0.6f, 0.6f, 0.6f, 1f));
                ColorMapping.Add("Executioner", new Color(0.55f, 0.25f, 0.02f, 1f));
                ColorMapping.Add("The Glitch", Color.green);
                ColorMapping.Add("Arsonist", new Color(1f, 0.3f, 0f));
            }
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => ColorMapping.Keys.ToList();
    }
}
