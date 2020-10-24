using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WolfLive.Api;
using WolfLive.Api.Commands;

namespace NsfwDetector.WolfLive.Filters
{
	public class ContainsLinkAttribute : FilterAttribute
	{
		public override Task<bool> Validate(IWolfClient client, CommandMessage message)
		{
			var urls = Urls(message.Message.Content);
			return Task.FromResult(urls.Length > 0);
		}

		public static string[] Urls(string message)
		{
			var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			return linkParser.Matches(message).Cast<Match>().Select(t => t.Value).ToArray();
		}
	}
}
