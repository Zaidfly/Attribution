using EnvironmentInitializer;
using FluentMigrator;

namespace Attribution.Api.Tests.Migrations
{
    [TestMigration(3)]
    public class Migration003_UrlReferrers_AddForDeletion : ForwardOnlyMigration
    {
        public const string Sometestdomen = "somedeletingdomain.test";
        
        private const string TableName = "url_referrers";

        public override void Up()
        {
            Insert.IntoTable(TableName).Row(new {host = Sometestdomen, is_deleted = false});
        }
    }
}