using System.Threading.Tasks;
using WolfLive.Api;
using WolfLive.Api.Commands;

namespace NsfwDetector.WolfLive.Filters
{
	public class ImageOnlyAttribute : FilterAttribute
	{
		public override Task<bool> Validate(IWolfClient client, CommandMessage message)
		{
			var (msg, _) = message;
			return Task.FromResult(msg.MimeType == "text/image_link");
		}
	}
}
