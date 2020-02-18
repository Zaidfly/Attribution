using EnvironmentInitializer;
using FluentMigrator;

namespace Attribution.Api.Tests.Migrations
{
    [TestMigration(2)]
    public class Migration002_UrlReferrers_AddDeleted : ForwardOnlyMigration
    {
        public const string Sometestdomen = "somedeleteddomain.test";
        
        private const string TableName = "url_referrers";

        public override void Up()
        {
            Insert.IntoTable(TableName).Row(new {host = Sometestdomen, is_deleted = true});
        }
    }
}