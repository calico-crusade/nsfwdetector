using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WolfLive.Api;
using WolfLive.Api.Commands;
using WolfLive.Api.Exceptions;
using WolfLive.Api.Models;

namespace NsfwDetector.WolfLive.Commands
{
	using Api;
	using Db;
	using Filters;

	public class NsfwCommands : WolfContext
	{
		private readonly INsfwService _nsfw;
		private readonly ILogger _logger;
		//private readonly INsfwDataService _data;

		public NsfwCommands(INsfwService nsfw, ILogger<NsfwCommands> logger/*, INsfwDataService data*/)
		{
			_nsfw = nsfw;
			_logger = logger;
			//_data = data;
		}

		[Command("test")]
		public async Task Test()
		{
			await this.Reply("Hello world");
		}

		[Command("check")]
		public async Task IsNsfw(string url)
		{
			try
			{
				var msg = Message;
				if (string.IsNullOrEmpty(url))
				{
					await this.Reply("Either send me an image or an image URL");
					url = (msg = await this.Next()).Content;
				}

				var results = await _nsfw.DetermineNsfw(url);
				if (results == null)
				{
					_logger.LogError("Return results were null");
					return;
				}

				if (!(results?.Worked ?? false))
				{
					_logger.LogError(results.Results.Error);
					return;
				}
				var formated = string.Join("\r\n", results.Results.Classifications.Select(t => $"{t.Name}: {(t.Probability * 100):00.00}%"));

				if (!results.Nsfw)
				{
					await this.Reply($"That image has been deemed safe for work!\r\n{formated}");
					return;
				}

				await this.DeleteMessage(msg);
				await this.Reply($"Message has been deleted: {results.Reason}\r\n{formated}");
			}
			catch (WolfSocketPacketException ex)
			{
				if (ex.Code == 403)
				{
					await this.Reply("I couldn't delete your message!");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		[Command("join"), AlecOnly]
		public async Task JoinGroup(string remainder)
		{
			var worked = await Client.JoinGroup(remainder);
			await this.Reply("Joined group? " + worked);
		}

		[Command("priv")]
		public async Task PrivilegesCheck(string remainder)
		{
			var user = string.IsNullOrEmpty(remainder) ? User : await Client.GetUser(remainder);

			var bob = new StringBuilder();

			for(var i = 0; i < 31; i++)
			{
				var flag = 1 << i;
				var hasFlag = ((user?.PrivilegesFlag ?? 0) & flag) != 0;

				bob.AppendLine($"{i} - {flag} - {hasFlag} - {(PrivilegeType)flag}");
			}

			await this.Reply(bob.ToString());
		}

		//[Command("store")]
		//public async Task RedisCheck(string remainder)
		//{
		//	var parts = remainder.Split(' ');
		//	if (parts.Length <= 1)
		//	{
		//		var data = await _data.Fetch(remainder);
		//		await this.Reply($"Results: {(data ?? "null")}");
		//		return;
		//	}

		//	var results = await _data.Set(parts[0], string.Join(" ", parts.Skip(1)));
		//	await this.Reply($"Results: {results}");
		//}
	}
}
