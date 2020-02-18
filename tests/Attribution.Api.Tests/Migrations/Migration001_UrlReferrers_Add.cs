using EnvironmentInitializer;
using FluentMigrator;
using FluentMigrator.SqlServer;

namespace Attribution.Api.Tests.Migrations
{
    [TestMigration(1)]
    public class Migration001_UrlReferrers_Add : ForwardOnlyMigration
    {
        public const string SomeTestHost = "somedomain.test";
        public const int Id = 5000;
        private const string TableName = "url_referrers";

        public override void Up()
        {
            Insert
                .IntoTable(TableName)
                .Row(
                    new
                    {
                        id = Id,
                        host = SomeTestHost,
                        is_deleted = false
                    })
                .WithIdentityInsert();
        }
    }
}