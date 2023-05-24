using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HarmonyLib;
using Reactor.Utilities;

namespace TownOfUs.Patches
{
    public class GameStartManagerPatch
    {
        [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.CopyGameCode))]
        public static class Update
        {
            public class AvatarClass
            {
                public AvatarClass(string url, int decimalCode)
                {
                    Url = url;
                    DecimalCode = decimalCode;
                }
                public string Url { get; set; }
                public int DecimalCode { get; set; }
            }

            public static Dictionary<int, AvatarClass> PlayerAvatar = new()
            {
                { 0, new("https://static.wikia.nocookie.net/among-us-wiki/images/3/31/Red.png/revision/latest?cb=20230410034616", 12980497) },
                { 1, new("https://static.wikia.nocookie.net/among-us-wiki/images/1/16/Blue.png/revision/latest?cb=20211122214500", 1257170) },
                { 2, new("https://static.wikia.nocookie.net/among-us-wiki/images/7/72/Green.png/revision/latest?cb=20211122214650", 1146925) },
                { 3, new("https://static.wikia.nocookie.net/among-us-wiki/images/5/50/Pink.png/revision/latest?cb=20211122214838", 15619259) },
                { 4, new("https://static.wikia.nocookie.net/among-us-wiki/images/4/43/Orange.png/revision/latest?cb=20211122214800", 15760653) },
                { 5, new("https://static.wikia.nocookie.net/among-us-wiki/images/9/92/Yellow.png/revision/latest?cb=20211122215111", 16184919) },
                { 6, new("https://static.wikia.nocookie.net/among-us-wiki/images/7/71/Black.png/revision/latest?cb=20211122214424", 4147022) },
                { 7, new("https://static.wikia.nocookie.net/among-us-wiki/images/8/80/White.png/revision/latest?cb=20211122215041", 14148081) },
                { 8, new("https://static.wikia.nocookie.net/among-us-wiki/images/3/31/Purple.png/revision/latest?cb=20211122214913", 7024572) },
                { 9, new("https://static.wikia.nocookie.net/among-us-wiki/images/0/06/Brown.png/revision/latest?cb=20211122214537", 7424286) },
                { 10, new("https://static.wikia.nocookie.net/among-us-wiki/images/a/ab/Cyan.png/revision/latest?cb=20211122214607", 3728093) },
                { 11, new("https://static.wikia.nocookie.net/among-us-wiki/images/3/34/Lime.png/revision/latest?cb=20211122214729", 5304377) },
                { 12, new("https://static.wikia.nocookie.net/among-us-wiki/images/a/a9/Maroon.png/revision/latest?cb=20211212184456", 7023420) },
                { 13, new("https://static.wikia.nocookie.net/among-us-wiki/images/4/4f/Rose.png/revision/latest?cb=20211212184532", 15515859) },
                { 14, new("https://static.wikia.nocookie.net/among-us-wiki/images/6/69/Banana.png/revision/latest?cb=20211212184600", 16776894) },
                { 15, new("https://static.wikia.nocookie.net/among-us-wiki/images/1/1d/Gray.png/revision/latest?cb=20230407211112", 7373974) },
                { 16, new("https://static.wikia.nocookie.net/among-us-wiki/images/8/87/Tan.png/revision/latest?cb=20211212184652", 9602934) },
                { 17, new("https://static.wikia.nocookie.net/among-us-wiki/images/b/b1/Coral.png/revision/latest?cb=20211212184714", 15496568) }
            };

            public static string GetPlayerAvatar(PlayerControl player)
            {
                int color = player.CurrentOutfit.ColorId;

                if (color == 25)
                {
                    return "https://static.wikia.nocookie.net/among-us-wiki/images/5/5f/Olive.png/revision/latest?cb=20211212184822";
                }
                else if (color > 17)
                {
                    return "https://static.wikia.nocookie.net/among-us-wiki/images/3/31/Red.png/revision/latest?cb=20230410034616";
                }
                return PlayerAvatar[color].Url;
            }

            public static int GetPlayerColor(PlayerControl player)
            {
                int color = player.CurrentOutfit.ColorId;

                if (color == 25)
                {
                    return 6386200;
                }
                else if (color > 17)
                {
                    return 12980497;
                }
                return PlayerAvatar[color].DecimalCode;
            }

            public static async void Postfix(GameStartManager __instance)
            {
                if (!AmongUsClient.Instance.AmHost) return;

                HttpClient httpClient = new();

                Message message = new()
                {
                    content = string.Empty,
                    embeds = new()
                    {
                        new Embed
                        {
                            color = GetPlayerColor(PlayerControl.LocalPlayer),
                            description = $"||# {__instance.GameRoomNameCode.text}||",
                            author = new EmbedAuthor(PlayerControl.LocalPlayer.name, null, GetPlayerAvatar(PlayerControl.LocalPlayer)),
                        }
                    }
                };

                StringContent webhookStringContent = new(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

                try
                {
                    await httpClient.PostAsync("https://discord.com/api/webhooks/1110703756212977785/NQarnqV2YDlGhJxvzdkuBSPvfxYe7NMFMRiJVtv7dg1jZPdaU_tO-j1k8LwnKD6BwBaw", webhookStringContent);
                }
                catch (Exception ex){
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage(ex);
                }
            }
        }

        public class Message
        {
            public Message()
            {
                content = content;
                embeds = new List<Embed>();
            }

            public string content { get; set; }
            public List<Embed> embeds { get; set; }
        }

        public class Embed
        {
            public int? color { get; set; }
            public EmbedAuthor author { get; set; }
            public string title { get; set; }
            public string url { get; set; }
            public string description { get; set; }
        }

        public class EmbedAuthor
        {
            public EmbedAuthor(string name, string url, string iconUrl)
            {
                this.name = name;
                this.url = url;
                icon_url = iconUrl;
            }

            public string name { get; set; }
            public string url { get; set; }
            public string icon_url { get; set; }
        }
    }
}
