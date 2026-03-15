namespace CurrencyUpdater.Configuration
{
	public class UpdateSettings
	{
		public const string SectionName = "UpdateSettings";

		public int IntervalHours { get; set; } = 24;
		public int RetryCount { get; set; } = 3;
		public int RetryDelaySeconds { get; set; } = 60;

		public TimeSpan Interval => TimeSpan.FromHours(IntervalHours);
		public TimeSpan RetryDelay => TimeSpan.FromSeconds(RetryDelaySeconds);
	}
}
