using System.Linq;

namespace NsfwDetector.Api
{
	public class NsfwResult
	{
		public bool Worked { get; set; }
		public string Error { get; set; }
		public Classification[] Classifications { get; set; }

		public double this[string name] => (Classifications?.FirstOrDefault(t => t.Name == name)?.Probability ?? 0) * 100;
	}
}
