using CurrencyService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CurrencyService.Infrastructure
{
	public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite> 
	{
		public void Configure(EntityTypeBuilder<Favorite> builder)
		{
			builder.ToTable("FAVORITES"); 
			builder.HasKey(f => new { f.UserId, f.CurrencyId });

			builder.Property(f => f.UserId)
				.HasColumnName("USER_ID");

			builder.Property(f => f.CurrencyId)
				.HasColumnName("CURRENCY_ID");
		}
	}
}
