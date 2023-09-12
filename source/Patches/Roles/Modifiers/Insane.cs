using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Patches.Roles.Modifiers
{
    public class Insane : Modifier
    {
        public static List<Insane> RunningCoroutines = new List<Insane>();
        public static bool MeetingInProgress = false;

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
                if (CustomGameOptions.InsaneGuardianAngel)
                    _rolesToReturn.Add(RoleEnum.GuardianAngel);

                return _rolesToReturn;
            } 
        }

        public Insane(PlayerControl player) : base(player)
        {
            Name = "Insane";
            SymbolName = "?";
            TaskText = () => "You can't trust yourself, nor your abilities";
            Color = Patches.Colors.Insane;
            ModifierType = ModifierEnum.Insane;
            IsHidden = true;

            if(Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(InsaneEvents());
                RunningCoroutines.Add(this);
            }
        }

        public IEnumerator RemoveBodyArrows(float delay, List<byte> ids)
        {
            yield return new WaitForSeconds(delay);

            if (Role.GetRole(Player).RoleType != RoleEnum.Mystic)
                yield return new WaitForEndOfFrame();

            Mystic mystic = Role.GetRole<Mystic>(Player);

            foreach (byte id in ids)
            {
                try
                {
                    if(mystic.BodyArrows.ContainsKey(id))
                    {
                        if (mystic.BodyArrows[id] != null)
                        {
                            mystic.BodyArrows[id].Destroy();
                            mystic.BodyArrows.Remove(id);
                        }
                    }
                }
                catch { }
            }
        }

        public IEnumerator InsaneEvents()
        {
            Logger<TownOfUs>.Info($"Insane coroutine started for {Player.PlayerId}");
            while (!Player.Data.IsDead && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(30, 60));
                Logger<TownOfUs>.Info($"Insane coroutine for {Player.PlayerId} looped. Selecting effects...");

                if (MeetingInProgress)
                    continue;

                if(Player.Is(RoleEnum.Mystic))
                {
                    var fakeBody = PlayerControl.AllPlayerControls.ToArray().Where(x => x != Player && !x.Data.IsDead).First();

                    if(CustomGameOptions.MysticArrowDuration > 0.1f)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = TownOfUs.Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        Role.GetRole<Mystic>(Player).BodyArrows.Add(fakeBody.PlayerId, arrow);
                        Role.GetRole<Mystic>(Player).BodyArrows.GetValueSafe(fakeBody.PlayerId).target = fakeBody.GetTruePosition();

                        Coroutines.Start(Utils.FlashCoroutine(Colors.Mystic));

                        Coroutines.Start(RemoveBodyArrows(CustomGameOptions.MysticArrowDuration, new List<byte>() { fakeBody.PlayerId }));
                    }
                }

                if(Player.Is(RoleEnum.Medic))
                {
                    Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
                }
            }
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

    public enum RevealsTo
    {
        Self = 0,
        Others = 1,
        Everyone = 2
    }
}
