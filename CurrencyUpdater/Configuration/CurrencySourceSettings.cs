namespace CurrencyUpdater.Configuration
{
	public class CurrencySourceSettings
	{
		public const string SectionName = "CurrencySourceSettings";

		public string Url { get; set; } = string.Empty;
		public int TimeoutSeconds { get; set; } = 30;
	}
}
