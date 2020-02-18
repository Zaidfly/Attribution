using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190607174119)]
    [Tags("Pre")]
    public class Migration20190607174119_CreateTables : ForwardOnlyMigration
    {
        public override void Up()
        {
	        Execute.Sql(@"-- noinspection SqlNoDataSourceInspectionForFile

				CREATE TABLE IF NOT EXISTS user_actions (
					id bigserial primary key,
					user_id int8 not null,
					action_date TIMESTAMP not null,
					action_type int4 not null,
					object_type int4 not null,
					object_id int8 not null,
					partner_id varchar(50) not null,
					campaign_id varchar(50) not null)");
	        
	        Execute.Sql(@"-- noinspection SqlNoDataSourceInspectionForFile
				
				CREATE UNIQUE INDEX IF NOT EXISTS 
					uix_user_actions_user_id_action_type 
					ON user_actions (user_id, action_type, object_type, object_id);");
        }
    }
}