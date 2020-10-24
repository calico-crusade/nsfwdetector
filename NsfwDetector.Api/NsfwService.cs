using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace NsfwDetector.Api
{
	public interface INsfwService
	{
		Task<NsfwResult> GetClassifications(string url);
		Task<NsfwResponse> DetermineNsfw(string imageUrl, double pornPercent = 50, double sexyPercent = 50, double hentaiPercent = 50);
	}

	public class NsfwService : INsfwService
	{
		private const string URL = "http://localhost:8080/nsfw/";

		private readonly ILogger _logger;

		public NsfwService(ILogger<NsfwService> logger)
		{
			_logger = logger;
		}

		public async Task<T> Get<T>(string url)
		{
			using (var wc = new WebClient())
			{
				var data = await wc.DownloadStringTaskAsync(url);
				return JsonConvert.DeserializeObject<T>(data);
			}
		}

		public Task<NsfwResult> GetClassifications(string url)
		{
			var encodedUrl = WebUtility.UrlEncode(url);
			var fullUrl = $"{URL}{encodedUrl}";

			return Get<NsfwResult>(fullUrl);
		}

		public async Task<NsfwResponse> DetermineNsfw(string imageUrl, double pornPercent = 50, double sexyPercent = 50, double hentaiPercent = 50)
		{
			try
			{
				var results = await GetClassifications(imageUrl);
				if (!results.Worked)
					throw new Exception("Error occurred in NSFW response: " + results.Error);

				var response = new NsfwResponse { Results = results };

				if (response.Porn >= pornPercent)
				{
					response.Reason = $"Pornography ({response.Porn:00.00}%)";
					return response;
				}

				if (response.Sexy >= sexyPercent)
				{
					response.Reason = $"Sexual Content ({response.Sexy:00.00}%)";
					return response;
				}

				if (response.Hentai >= hentaiPercent)
				{
					response.Reason = $"Hentai (Anime Pornography) ({response.Hentai:00.00}%)";
					return response;
				}

				return response;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while attempting to resolve NSFW content");
				return new NsfwResponse();
			}
		}
	}
}
