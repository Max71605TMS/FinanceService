using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class FillData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.InsertData(
				table: "USERS",
				columns: new[] { "ID", "NAME", "PASSWORD" },
				values: new object[,]
				{
					{ 1, "ivan_ivanov", "1qw2" },
                    { 2, "maria_sidorova", "qwerty" },
					{ 3, "alexey_smirnov", "pass789" }
				});
            
			migrationBuilder.Sql("SELECT setval('\"USERS_ID_seq\"', (SELECT MAX(\"ID\") FROM \"USERS\"));");

			migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM ""CURRENCIES"") THEN
                        INSERT INTO ""CURRENCIES"" (""NAME"", ""RATE"") VALUES
                            ('USD', 92.50),
                            ('EUR', 100.20),
                            ('GBR', 117.80),
                            ('AUD', 60.15);
                    END IF;
                END $$;
            ");

			migrationBuilder.Sql("SELECT setval('\"CURRENCIES_ID_seq\"', (SELECT MAX(\"ID\") FROM \"CURRENCIES\"));");

			migrationBuilder.InsertData(
				table: "FAVORITES",
				columns: new[] { "USER_ID", "CURRENCY_ID" },
				values: new object[,]
				{
                              // Иван Иванов 
                    { 1, 1 }, // Доллар США
                    { 1, 2 }, // Евро
                    
                              // Мария Сидорова
                    { 2, 2 }, // Евро
                    { 2, 3 }, // Фунт
                    { 2, 4 }, // Канадский доллар
                    
                              // Алексей Смирнов
                    { 3, 1 }, // Доллар
                    { 3, 2 }, // Евро
                    { 3, 4 }, // Канадский доллар
                });
		
		}

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
                DELETE FROM ""FAVORITES"" 
                WHERE ""USER_ID"" IN (1, 2, 3);
            ");
            			
			migrationBuilder.Sql(@"
                DELETE FROM ""USERS"" 
                WHERE ""ID"" IN (1, 2, 3);
            ");

			
migrationBuilder.Sql(@"
	DELETE FROM ""CURRENCIES"" 
	WHERE ""NAME"" IN ('USD', 'EUR', 'GBR', 'AUD');
");
}
}
}
