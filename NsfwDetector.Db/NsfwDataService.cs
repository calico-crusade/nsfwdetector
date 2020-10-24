using CardboardBox.Extensions;
using System.Threading.Tasks;

namespace NsfwDetector.Db
{

	public interface INsfwDataService
	{
		Task<string> Fetch(string key);
		Task<bool> Set(string key, string value);
	}

	public class NsfwDataService : INsfwDataService
	{
		private readonly IRedisService _redis;

		public NsfwDataService(IRedisService redis)
		{
			_redis = redis;
		}

		public Task<string> Fetch(string key)
		{
			return _redis.Get<string>(key);
		}

		public Task<bool> Set(string key, string value)
		{
			return _redis.Set<string>(key, value);
		}

	}
}
