using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190910131500)]
    [Tags("Pre")]
    public class Migration20190910131500_UserActions_AlterColumns_PartnerIdAndCampaignId : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        private const string PartnerIdName = "partner_id";
        private const string CampaignIdName = "campaign_id";
		
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{PartnerIdName}')
                     then
						ALTER TABLE {TableName} ALTER COLUMN {PartnerIdName} DROP NOT NULL;
                end if;
                end;
                $do$");
            
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{CampaignIdName}')
                     then
						ALTER TABLE {TableName} ALTER COLUMN {CampaignIdName} DROP NOT NULL;
                end if;
                end;
                $do$");
        }
    }
}