using System.Threading.Tasks;
using WolfLive.Api;
using WolfLive.Api.Commands;

namespace NsfwDetector.WolfLive.Filters
{
	public class AlecOnlyAttribute : FilterAttribute
	{
		public override Task<bool> Validate(IWolfClient client, CommandMessage message)
		{
			var (_, user) = message;

			return Task.FromResult(user.Id == "43681734");
		}
	}
}
