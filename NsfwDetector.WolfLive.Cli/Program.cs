﻿using CardboardBox.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using WolfLive.Api;
using WolfLive.Api.Commands;

namespace NsfwDetector.WolfLive.Cli
{
	using Api;
	using Commands;
	using Db;
	using Filters;

	public class Program
	{
		public static async Task Main(string[] args)
		{
			var client = new WolfClient()
				.SetupCommands()
				.WithConfig("appsettings.json")
				.GetConfig(out SettingsRoot settings)
				.WithSerilog()
				.WithServices(c =>
				{
				   c.AddTransient<INsfwService, NsfwService>()
					/*.AddTransient<INsfwDataService, NsfwDataService>()
					.AddRedis(r =>
					{
						r.EndPoints.Add(settings.Redis.Host);
						r.Password = settings.Redis.Password;
					})*/;
				})
				.WithCommandSet(c =>
				{
				   c.AddCommands<NsfwCommands>()
					.AddFilters<NotMeAttribute>()
					.WithPrefix(settings.Account.Prefix);
				})
				.WithCommandSet(c =>
				{
				   c.AddCommands<PrefixlessCommands>()
					.AddFilters<NotMeAttribute>();
				})
				.Done();

			client.OnConnected += _client_OnConnected;
			client.OnReconnecting += Client_OnReconnected;

			var result = await client.Login(settings.Account.Email, settings.Account.Password);

			if (!result)
			{
				Console.WriteLine("Login failed!");
				return;
			}

			Console.WriteLine("Login success! Logged in as: " + client.CurrentUser().Nickname);

			await Task.Delay(-1);
		}

		private static void Client_OnReconnected(IWolfClient client, int reconnectionCount)
		{
			Console.WriteLine("Reconnect(ed/ing)... " + reconnectionCount);
		}

		private static void _client_OnConnected(IWolfClient client)
		{
			Console.WriteLine("Client connected");
		}
	}
}
