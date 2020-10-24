namespace NsfwDetector.Api
{
	public class NsfwResponse
	{
		public NsfwResult Results { get; set; }

		public double Porn => Results["Porn"];
		public double Sexy => Results["Sexy"];
		public double Hentai => Results["Hentai"];

		public bool Nsfw => !string.IsNullOrEmpty(Reason);

		public bool Worked => Results?.Worked ?? false;

		public string Reason { get; set; }
	}
}
