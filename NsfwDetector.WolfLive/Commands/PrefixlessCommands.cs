using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WolfLive.Api.Commands;

namespace NsfwDetector.WolfLive.Commands
{
	using Api;
	using Filters;

	public class PrefixlessCommands : WolfContext
	{
		private readonly INsfwService _nsfw;
		private readonly ILogger _logger;

		public PrefixlessCommands(INsfwService nsfw, ILogger<PrefixlessCommands> logger)
		{
			_nsfw = nsfw;
			_logger = logger;
		}

		[ContainsLink, GroupOnly]
		public async Task UrlReceived()
		{
			try
			{
				var urls = ContainsLinkAttribute.Urls(Message.Content);

				foreach (var url in urls)
				{
					var nsfw = await _nsfw.DetermineNsfw(url);
					if (!nsfw.Worked || !nsfw.Nsfw)
						continue;

					await this.NsfwDelete(nsfw, _logger);
					return;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred");
			}
		}

		[ImageOnly, GroupOnly]
		public async Task ImageRecieved()
		{
			try
			{
				string url = Message.Content;
				_logger.LogDebug($"Evaluating image: {User.Nickname} ({User.Id}) >> {Group.Name} ({Group.Id}): {url}");

				var nsfw = await _nsfw.DetermineNsfw(url);

				if (!nsfw.Worked || !nsfw.Nsfw)
					return;

				await this.NsfwDelete(nsfw, _logger);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred");
			}
		}
	}
}
