using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WolfLive.Api.Commands;
using WolfLive.Api.Exceptions;

namespace NsfwDetector.WolfLive
{
	using Api;

	public static class NsfwExtensions
	{
		public static async Task NsfwDelete(this WolfContext context, NsfwResponse results, ILogger _logger)
		{
			try
			{
				await context.DeleteMessage(context.Message);
				await context.Reply($"I have deleted your message because I determined it contained {results.Reason}!");
			}
			catch (WolfSocketPacketException ex)
			{
				if (ex.Code != 403)
				{
					_logger.LogError(ex, "Packet exception occurred: " + ex.Code);
					return;
				}

				await context.Reply("I deemed that your image was not safe for wolf, but I am unable to delete your message because I don't have the correct permissions!");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while executing nsfwdelete");
			}
		}
	}
}
