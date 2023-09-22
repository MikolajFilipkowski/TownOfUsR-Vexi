using Hazel;
using InnerNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.CrewmateRoles.MedicMod;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using UnityEngine.UI;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.ImpostorRoles.BlackmailerMod;
using TownOfUs.Patches;
using TownOfUs.NeutralRoles.DoomsayerMod;

namespace TownOfUs.Roles
{
    public class Pelican : Role
    {
        public static Sprite HackSprite = TownOfUs.EatSprite;
        public static Sprite LockSprite = TownOfUs.LockSprite;

        public bool lastMouse;

        public Pelican(PlayerControl owner) : base(owner)
        {
            Name = "Pelican";
            Color = Patches.Colors.Pelican;
            LastHack = DateTime.UtcNow;
            HackButton = null;
            HackTarget = null;
            RoleType = RoleEnum.Pelican;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "Devour them all alive to win!";
            TaskText = () => "Devour them all alive to win.\nMeeting will kill everybody you devoured!\nFake Tasks:";
            Faction = Faction.NeutralKilling;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastHack { get; set; }
        public KillButton HackButton { get; set; }
        public PlayerControl HackTarget { get; set; }

        public bool PelicanWins { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.IsDevoured()) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.IsDevoured() &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling))) == 1)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PelicanWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Glitch Edition");
            PelicanWins = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var pelicanTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            pelicanTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = pelicanTeam;
        }

        public void Update(HudManager __instance)
        {
            if (!Player.Data.IsDead)
            {
                Utils.SetClosestPlayer(ref ClosestPlayer);
            }

            Player.nameText().color = Color;

            if (MeetingHud.Instance != null)
                foreach (var player in MeetingHud.Instance.playerStates)
                    if (player.NameText != null && Player.PlayerId == player.TargetPlayerId)
                        player.NameText.color = Color;

            if (HudManager.Instance != null && HudManager.Instance.Chat != null)
                foreach (var bubble in HudManager.Instance.Chat.chatBubPool.activeChildren)
                    if (bubble.Cast<ChatBubble>().NameText != null &&
                        Player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                        bubble.Cast<ChatBubble>().NameText.color = Color;

            FixedUpdate(__instance);
        }

        public void FixedUpdate(HudManager __instance)
        {

            HackButtonHandler.HackButtonUpdate(this, __instance);

            if (HackButton != null && Player.Data.IsDead)
                HackButton.SetTarget(null);
        }

        public bool UseAbility(KillButton __instance)
        {
            if (__instance == HackButton)
                HackButtonHandler.HackButtonPress(this, __instance);

            return false;
        }

        public void RpcSetHacked(PlayerControl hacked)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.SetDevoured, SendOption.Reliable, -1);
            writer.Write(hacked.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            SetHacked(hacked);
        }

        public void SetHacked(PlayerControl hacked)
        {
            LastHack = DateTime.UtcNow;
            SetInv(hacked);
            Coroutines.Start(AbilityCoroutine.Hack(this, hacked));
        }

        public static void SetInv(PlayerControl hacked)
        {
            GetRole(hacked).Devoured = true;
            hacked.SetOutfit(CustomPlayerOutfitType.Swooper, new GameData.PlayerOutfit()
            {
                ColorId = hacked.CurrentOutfit.ColorId,
                HatId = "",
                SkinId = "",
                VisorId = "",
                PlayerName = " "
            });
            hacked.myRend().color = Color.clear;
            hacked.nameText().color = Color.clear;
            hacked.cosmetics.colorBlindText.color = Color.clear;
            hacked.Collider.enabled = false;
        }

        public static void UnInv(PlayerControl hacked)
        {
            hacked.Collider.enabled = true;
            Utils.Unmorph(hacked);
            hacked.myRend().color = Color.white;
            GetRole(hacked).Devoured = false;
        }

        public static void RpcMurderPlayer(PlayerControl player, PlayerControl assassin)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(voteArea, player, assassin);
        }
        public static void RpcMurderPlayer(PlayerVoteArea voteArea, PlayerControl player, PlayerControl assassin)
        {
            MurderPlayer(voteArea, player);
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.PelicanKill, SendOption.Reliable, -1);
            writer.Write(player.PlayerId);
            writer.Write(assassin.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl player, bool checkLover = true)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(voteArea, player, checkLover);
        }

        public static void MurderPlayer(
            PlayerVoteArea voteArea,
            PlayerControl player,
            bool checkLover = true
        )
        {

            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                Utils.ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                    swapper.ListOfActives.Clear();
                    swapper.Buttons.Clear();
                    SwapVotes.Swap1 = null;
                    SwapVotes.Swap2 = null;
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    var buttons = Role.GetRole<Swapper>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Imitator))
                {
                    var imitator = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
                    imitator.ListOfActives.Clear();
                    imitator.Buttons.Clear();
                    SetImitate.Imitate = null;
                    var buttons = Role.GetRole<Imitator>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }

                if (player.Is(RoleEnum.Vigilante))
                {
                    var retributionist = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                    ShowHideButtonsVigi.HideButtonsVigi(retributionist);
                }

                if (player.Is(AbilityEnum.Assassin))
                {
                    var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                    ShowHideButtons.HideButtons(assassin);
                }

                if (player.Is(RoleEnum.Doomsayer))
                {
                    var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                    ShowHideButtonsDoom.HideButtonsDoom(doomsayer);
                }

                if (player.Is(RoleEnum.Mayor))
                {
                    var mayor = Role.GetRole<Mayor>(PlayerControl.LocalPlayer);
                    mayor.RevealButton.Destroy();
                }
            }
            player.Die(DeathReason.Kill, false);
            if (checkLover && player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var otherLover = Modifier.GetModifier<Lover>(player).OtherLover.Player;
                if (!otherLover.Is(RoleEnum.Pestilence)) MurderPlayer(otherLover, false);
            }

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);
            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();
            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = MeetingHud.Instance;
            if (amOwner)
            {
                meetingHud.SetForegroundForDead();
            }

            var blackmailers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();
            foreach (var role in blackmailers)
            {
                if (role.Blackmailed != null && voteArea.TargetPlayerId == role.Blackmailed.PlayerId)
                {
                    if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                    {
                        voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                        voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                        voteArea.XMark.transform.localPosition = new Vector3(
                            voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                            voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset,
                            voteArea.XMark.transform.localPosition.z);
                    }
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var vigi = Role.GetRole<Vigilante>(PlayerControl.LocalPlayer);
                ShowHideButtonsVigi.HideTarget(vigi, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var assassin = Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer);
                ShowHideButtons.HideTarget(assassin, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var doom = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                ShowHideButtonsDoom.HideTarget(doom, voteArea.TargetPlayerId);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                var swapper = Role.GetRole<Swapper>(PlayerControl.LocalPlayer);
                var button = swapper.Buttons[voteArea.TargetPlayerId];
                if (button.GetComponent<SpriteRenderer>().sprite == TownOfUs.SwapperSwitch)
                {
                    swapper.ListOfActives[voteArea.TargetPlayerId] = false;
                    if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                    if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                }
                button.SetActive(false);
                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                swapper.Buttons[voteArea.TargetPlayerId] = null;
            }

            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (playerVoteArea.VotedFor != player.PlayerId) continue;
                playerVoteArea.UnsetVote();
                var voteAreaPlayer = Utils.PlayerById(playerVoteArea.TargetPlayerId);
                if (!voteAreaPlayer.AmOwner) continue;
                meetingHud.ClearVote();
            }

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }

        public static void Drag(PlayerControl __instance, PlayerControl playerDragged)
        {
            if (!__instance.Is(RoleEnum.Pelican)) return;
            if (playerDragged == null) return;
            if (__instance.Data.IsDead)
            {
                return;
            }
            var currentPosition = __instance.transform.position;
            Vector3 newPos = ((Vector2)__instance.transform.position);
            newPos.z = currentPosition.z;

            //WHY ARE THERE DIFFERENT LOCAL Z INDEXS FOR DIFFERENT DECALS ON DIFFERENT LEVELS?!?!?!
            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (newPos.y > -7f)
                {
                    newPos.z = 0.0208f;
                }
                else
                {
                    newPos.z = -0.0273f;
                }
            }
            playerDragged.transform.position = newPos;
        }

        public static class AbilityCoroutine
        {
            public static Dictionary<byte, DateTime> tickDictionary = new Dictionary<byte, DateTime>();

            public static IEnumerator Hack(Pelican __instance, PlayerControl hackPlayer)
            {
                GameObject[] lockImg = { null, null, null, null };

                if (tickDictionary.ContainsKey(hackPlayer.PlayerId))
                {
                    tickDictionary[hackPlayer.PlayerId] = DateTime.UtcNow;
                    yield break;
                }

                while (true)
                {
                    Drag(__instance.Player, hackPlayer);
                    if (PlayerControl.LocalPlayer == hackPlayer)
                    {
                        if (HudManager.Instance.KillButton != null)
                        {
                            if (lockImg[0] == null)
                            {
                                lockImg[0] = new GameObject();
                                var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[0].layer = 5;
                            lockImg[0].transform.position =
                                new Vector3(HudManager.Instance.KillButton.transform.position.x,
                                    HudManager.Instance.KillButton.transform.position.y, -50f);
                            HudManager.Instance.KillButton.enabled = false;
                            HudManager.Instance.KillButton.graphic.color = Palette.DisabledClear;
                            HudManager.Instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                        }

                        if (HudManager.Instance.UseButton != null || HudManager.Instance.PetButton != null)
                        {
                            if (lockImg[1] == null)
                            {
                                lockImg[1] = new GameObject();
                                var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }
                            if (HudManager.Instance.UseButton != null)
                            {
                                lockImg[1].transform.position =
                                new Vector3(HudManager.Instance.UseButton.transform.position.x,
                                    HudManager.Instance.UseButton.transform.position.y, -50f);
                                lockImg[1].layer = 5;
                                HudManager.Instance.UseButton.enabled = false;
                                HudManager.Instance.UseButton.graphic.color = Palette.DisabledClear;
                                HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 1f);
                            }
                            else
                            {
                                lockImg[1].transform.position =
                                    new Vector3(HudManager.Instance.PetButton.transform.position.x,
                                    HudManager.Instance.PetButton.transform.position.y, -50f);
                                lockImg[1].layer = 5;
                                HudManager.Instance.PetButton.enabled = false;
                                HudManager.Instance.PetButton.graphic.color = Palette.DisabledClear;
                                HudManager.Instance.PetButton.graphic.material.SetFloat("_Desat", 1f);
                            }
                        }

                        if (HudManager.Instance.ReportButton != null)
                        {
                            if (lockImg[2] == null)
                            {
                                lockImg[2] = new GameObject();
                                var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[2].transform.position =
                                new Vector3(HudManager.Instance.ReportButton.transform.position.x,
                                    HudManager.Instance.ReportButton.transform.position.y, -50f);
                            lockImg[2].layer = 5;
                            HudManager.Instance.ReportButton.enabled = false;
                            HudManager.Instance.ReportButton.SetActive(false);
                        }

                        var role = GetRole(PlayerControl.LocalPlayer);
                        if (role != null)
                            if (role.ExtraButtons.Count > 0)
                            {
                                if (lockImg[3] == null)
                                {
                                    lockImg[3] = new GameObject();
                                    var lockImgR = lockImg[3].AddComponent<SpriteRenderer>();
                                    lockImgR.sprite = LockSprite;
                                }

                                lockImg[3].transform.position = new Vector3(
                                    role.ExtraButtons[0].transform.position.x,
                                    role.ExtraButtons[0].transform.position.y, -50f);
                                lockImg[3].layer = 5;
                                role.ExtraButtons[0].enabled = false;
                                role.ExtraButtons[0].graphic.color = Palette.DisabledClear;
                                role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 1f);
                            }

                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }

                    if (MeetingHud.Instance || __instance.Player.Data.IsDead || __instance.Player == null || hackPlayer == null || hackPlayer.Data.IsDead)
                    {

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.UnDevour, SendOption.Reliable, -1);
                        writer.Write(hackPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (MeetingHud.Instance && !(hackPlayer == null || hackPlayer.Data.IsDead || __instance.Player.Data.IsDead || __instance.Player == null))
                        {
                            RpcMurderPlayer(hackPlayer, __instance.Player);
                        }

                        foreach (var obj in lockImg)
                            if (obj != null)
                                obj.SetActive(false);

                        if (PlayerControl.LocalPlayer == hackPlayer)
                        {
                            if (HudManager.Instance.UseButton != null)
                            {
                                HudManager.Instance.UseButton.enabled = true;
                                HudManager.Instance.UseButton.graphic.color = Palette.EnabledColor;
                                HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 0f);
                            }
                            else
                            {
                                HudManager.Instance.PetButton.enabled = true;
                                HudManager.Instance.PetButton.graphic.color = Palette.EnabledColor;
                                HudManager.Instance.PetButton.graphic.material.SetFloat("_Desat", 0f);
                            }
                            HudManager.Instance.ReportButton.enabled = true;
                            HudManager.Instance.KillButton.enabled = true;
                            var role = GetRole(PlayerControl.LocalPlayer);
                            
                            if (role != null)
                                if (role.ExtraButtons.Count > 0)
                                {
                                    role.ExtraButtons[0].enabled = true;
                                    role.ExtraButtons[0].graphic.color = Palette.EnabledColor;
                                    role.ExtraButtons[0].graphic.material.SetFloat("_Desat", 0f);
                                }
                        }

                        tickDictionary.Remove(hackPlayer.PlayerId);
                        yield break;
                    }

                    yield return null;
                }
            }
        }

        public static class HackButtonHandler
        {
            public static void HackButtonUpdate(Pelican __gInstance, HudManager __instance)
            {
                if (__gInstance.HackButton == null)
                {
                    __gInstance.HackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.HackButton.gameObject.SetActive(true);
                    __gInstance.HackButton.graphic.enabled = true;
                }

                __gInstance.HackButton.graphic.sprite = HackSprite;

                __gInstance.HackButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !__gInstance.Player.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started);
                __gInstance.HackButton.transform.position = new Vector3(__gInstance.HackButton.transform.position.x,
                    __gInstance.HackButton.transform.position.y, __instance.ReportButton.transform.position.z);
                __gInstance.HackButton.SetCoolDown(
                    CustomGameOptions.DevourCd - (float)(DateTime.UtcNow - __gInstance.LastHack).TotalSeconds,
                    CustomGameOptions.DevourCd);

                __gInstance.HackButton.SetTarget(null);
                __gInstance.HackTarget = null;

                if (__gInstance.HackButton.isActiveAndEnabled)
                {
                    PlayerControl closestPlayer = null;
                    Utils.SetTarget(
                        ref closestPlayer,
                        __gInstance.HackButton,
                        GameOptionsData.KillDistances[CustomGameOptions.GlitchHackDistance]
                    );
                    __gInstance.HackTarget = closestPlayer;
                }

                if (__gInstance.HackTarget != null)
                    __gInstance.HackTarget.myRend().material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void HackButtonPress(Pelican __gInstance, KillButton __instance)
            {
                // Bug: Hacking someone with a pet doesn't disable the ability to pet the pet
                // Bug: Hacking someone doing fuel breaks all their buttons/abilities including the use and report buttons
                if (__gInstance.HackTarget != null)
                {
                    var interact = Utils.Interact(__gInstance.Player, __gInstance.HackTarget);
                    if (interact[4] == true)
                    {
                        __gInstance.RpcSetHacked(__gInstance.HackTarget);
                    }
                    if (interact[0] == true)
                    {
                        __gInstance.LastHack = DateTime.UtcNow;
                        return;
                    }
                    else if (interact[1] == true)
                    {
                        __gInstance.LastHack = DateTime.UtcNow;
                        __gInstance.LastHack.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DevourCd);
                        return;
                    }
                    else if (interact[3] == true) return;
                    return;
                }
            }
        }
    }
}