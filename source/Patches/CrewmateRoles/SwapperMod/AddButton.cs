using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using UnityEngine.UI;
using static Rewired.Utils.Classes.Utility.ObjectInstanceTracker;
using Object = UnityEngine.Object;

namespace TownOfUs.CrewmateRoles.SwapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButton
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfUs.SwapperSwitch;
        public static Sprite DisabledSprite => TownOfUs.SwapperSwitchDisabled;

        public static void GenButton(Swapper role, int index, bool isDead)
        {
            if (isDead)
            {
                role.Buttons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;


            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.Buttons.Add(newButton);
            role.ListOfActives.Add(false);
        }


        private static Action SetActive(Swapper role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 2 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite) return;

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index] ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = !role.ListOfActives[index];

                _mostRecentId = index;

                SwapVotes.Swap1 = null;
                SwapVotes.Swap2 = null;
                var toSet1 = true;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i]) continue;

                    if (toSet1)
                    {
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates[i];
                        toSet1 = false;
                    }
                    else
                    {
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates[i];
                    }
                }

                if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null)
                {
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    return;
                }

                if (role.Player.Is(ModifierEnum.Insane))
                {
                    byte fake1 = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && x != role.Player).Random().PlayerId;
                    byte fake2 = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && x != role.Player && x.PlayerId != fake1).Random().PlayerId;

                    try
                    {
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates.Where(x => x.TargetPlayerId == fake1).First();
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates.Where(x => x.TargetPlayerId == fake2).First();
                    }
                    catch { }

                    Utils.Rpc(CustomRPC.SetSwaps, SwapVotes.Swap1.TargetPlayerId, SwapVotes.Swap2.TargetPlayerId);
                }
                else
                    Utils.Rpc(CustomRPC.SetSwaps, SwapVotes.Swap1.TargetPlayerId, SwapVotes.Swap2.TargetPlayerId);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Swapper))
            {
                var swapper = (Swapper) role;
                swapper.ListOfActives.Clear();
                swapper.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swapper)) return;
            var swapperrole = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
                GenButton(swapperrole, i, __instance.playerStates[i].AmDead);
        }
    }
}