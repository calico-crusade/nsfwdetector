namespace NsfwDetector.WolfLive.Cli
{
	using Settings;

	public class SettingsRoot
	{
		public Account Account { get; set; }
		public Redis Redis { get; set; }
	}
}
