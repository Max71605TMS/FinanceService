using System.Globalization;
using System.Xml.Serialization;

namespace CurrencyUpdater.Models
{
	[XmlRoot("ValCurs")]
	public class ValCurs
	{
		[XmlElement("Valute")]
		public List<Valute> Valutes { get; set; } = new();
	}

	public class Valute
	{
		[XmlElement("NumCode")]
		public string NumCode { get; set; } = string.Empty;

		[XmlElement("CharCode")]
		public string CharCode { get; set; } = string.Empty;

		[XmlElement("Nominal")]
		public int Nominal { get; set; }

		[XmlElement("Name")]
		public string Name { get; set; } = string.Empty;

		[XmlElement("Value")]
		public string Value { get; set; } = string.Empty;

		public decimal RateValue
		{
			get
			{				
				var valueStr = Value.Replace(',', '.');				
				return decimal.Parse(valueStr, CultureInfo.InvariantCulture);
			}
		}
	}
}
