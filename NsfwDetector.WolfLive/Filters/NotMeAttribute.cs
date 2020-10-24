using System.Threading.Tasks;
using WolfLive.Api;
using WolfLive.Api.Commands;

namespace NsfwDetector.WolfLive.Filters
{
	public class NotMeAttribute : FilterAttribute
	{
		public override Task<bool> Validate(IWolfClient client, CommandMessage message)
		{
			var (_, usr) = message;
			return Task.FromResult(usr.Id != client.CurrentUser().Id);
		}
	}
}
