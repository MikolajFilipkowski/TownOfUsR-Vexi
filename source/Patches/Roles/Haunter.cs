using UnityEngine;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Haunter : Role
    {
        public bool Caught;

        public bool CompletedTasks;

        public bool Faded;

        public List<ArrowBehaviour> ImpArrows = new List<ArrowBehaviour>();

        public List<PlayerControl> HaunterTargets = new List<PlayerControl>();

        public List<ArrowBehaviour> HaunterArrows = new List<ArrowBehaviour>();

        public int TasksLeft = int.MaxValue;

        public Haunter(PlayerControl player) : base(player)
        {
            Name = "Haunter";
            ImpostorText = () => "";
            TaskText = () => "Complete all your tasks to reveal impostors!";
            Color = new Color(0.83f, 0.83f, 0.83f, 1f);
            RoleType = RoleEnum.Haunter;
        }

        public void Fade()
        {
            Faded = true;
            var color = new Color(1f, 1f, 1f, 0f);


            var maxDistance = ShipStatus.Instance.MaxLightRadius * PlayerControl.GameOptions.CrewLightMod;

            if (PlayerControl.LocalPlayer == null)
                return;

            var distance = (PlayerControl.LocalPlayer.GetTruePosition() - Player.GetTruePosition()).magnitude;

            var distPercent = distance / maxDistance;
            distPercent = Mathf.Max(0, distPercent - 1);

            var velocity = Player.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude;
            color.a = 0.07f + velocity / Player.MyPhysics.TrueGhostSpeed * 0.13f;
            color.a = Mathf.Lerp(color.a, 0, distPercent);

            Player.MyRend.color = color;

            Player.HatRenderer.SetHat(0, 0);
            Player.nameText.text = "";
            if (Player.MyPhysics.Skin.skin.ProdId != DestroyableSingleton<HatManager>.Instance
                .AllSkins.ToArray()[0].ProdId)
                Player.MyPhysics.SetSkin(0);
            if (Player.CurrentPet != null) Object.Destroy(Player.CurrentPet.gameObject);
            Player.CurrentPet =
                Object.Instantiate(
                    DestroyableSingleton<HatManager>.Instance.AllPets.ToArray()[0]);
            Player.CurrentPet.transform.position = Player.transform.position;
            Player.CurrentPet.Source = Player;
            Player.CurrentPet.Visible = Player.Visible;
        }
    }
}