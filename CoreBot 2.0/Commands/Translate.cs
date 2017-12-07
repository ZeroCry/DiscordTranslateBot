using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace CoreBot.Commands
{
    public class Translate : ModuleBase
    {
        private readonly CommandService _service;
        public Translate(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("help")]
        [Remarks("List of commands/info")]
        public async Task Help()
        {
            var embed = new EmbedBuilder();
            embed.Title = $"Translate Commands || `{Config.Load().Prefix} <command>`";
            foreach (var module in _service.Modules)
            {
                var modulelist = "";
                foreach (var command in module.Commands)
                {
                    modulelist += $"`{Config.Load().Prefix}{command.Summary}` - {command.Remarks}\n";
                }
                embed.AddField($"{module.Name}", $"{modulelist}");
            }
            embed.AddField("Designed By PassiveModding", $"{Config.Load().HomeInvite}\n" +
                                                         $"https://passivenation.com/");
            await ReplyAsync("", false, embed.Build());
        }

        [Command("translate")]
        [Summary("translate <language-code> <message>")]
        [Remarks("Translate from one language to another")]
        public async Task TranslateCmd(string language, [Remainder] string message)
        {
            var languages = new List<string> { "fr", "en", "es", "tl", "pt" };
            if (!languages.Contains(language))
            {
                await ReplyAsync("Unsupported Language");
                var embed2 = new EmbedBuilder();
                embed2.AddField("Language Codes",
                    "`fr` - french(français)\n" +
                    "`en` - english\n" +
                    "`es` - spanish(Español)\n" +
                    "`tl` - filipino\n" +
                    "`pt` - portugese (português)");
                await ReplyAsync("", false, embed2.Build());
                return;
            }
            var url = "https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=" + language +
                      "&dt=t&q=" + Uri.EscapeDataString(message);
            var embed = new EmbedBuilder();

            using (var client = new WebClient())
            {
                client.Encoding = Encoding.Unicode;
                client.DownloadFile($"{url}", $"{Context.Message.Id}.txt");
            }

            dynamic file =
                JsonConvert.DeserializeObject(File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                    $"{Context.Message.Id}.txt")));

            embed.AddField($"Original [{file[2]}]", $"{message}");
            embed.AddField($"Final [{language}]", $"{file[0][0][0]}");


            await ReplyAsync("", false, embed.Build());
            File.Delete(Path.Combine(AppContext.BaseDirectory, $"{Context.Message.Id}.txt"));
        }

        [Command("Languages")]
        [Summary("Languages")]
        [Remarks("Lists laguages supported by the bot")]
        public async Task Languages()
        {
                var embed2 = new EmbedBuilder();
                embed2.AddField("Languages",
                    "`fr` - french(français)\n" +
                    "`en` - english\n" +
                    "`es` - spanish(Español)\n" +
                    "`tl` - filipino\n" +
                    "`pt` - portugese (português)");
                await ReplyAsync("", false, embed2.Build());
        }
    }
}