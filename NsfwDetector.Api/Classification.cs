using Newtonsoft.Json;

namespace NsfwDetector.Api
{
	public class Classification
	{
		[JsonProperty("className")]
		public string Name { get; set; }
		public double Probability { get; set; }
	}
}
