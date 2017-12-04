﻿using System;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CoreBot
{
    public class Program
    {
        private CommandHandler _handler;
        public DiscordShardedClient Client;

        public static void Main(string[] args)
        {
            new Program().Start().GetAwaiter().GetResult();
        }

        public async Task Start()
        {
            Console.Title = "Discord Bot Base By PassiveModding";


            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "setup/")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "setup/"));

            Config.CheckExistence();
            var token = Config.Load().Token;
            Console.Title = Config.Load().BotName;

            Client = new DiscordShardedClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            try
            {
                await Client.LoginAsync(TokenType.Bot, token);
                await Client.StartAsync();
            }
            catch (Exception e)
            {
                Log.Information("------------------------------------\n" +
                                $"{e}\n" +
                                "------------------------------------\n" +
                                "Token was rejected by Discord (Invalid Token or Connection Error)\n" +
                                "------------------------------------");
            }


            var serviceProvider = ConfigureServices();
            _handler = new CommandHandler(serviceProvider);
            await _handler.ConfigureAsync();

            Client.Log += Client_Log;
            await Task.Delay(-1);
        }

        private static Task Client_Log(LogMessage arg)
        {
            Logger.LogInfo(arg.Message);
            return Task.CompletedTask;
        }

        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(new CommandService(
                    new CommandServiceConfig { CaseSensitiveCommands = false, ThrowOnError = false }));

            return services.BuildServiceProvider();
        }
    }
}
