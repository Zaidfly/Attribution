using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190829160910)]
    [Tags("Pre")]
    public class Migration20190829160910_UserActions_AddColumn_ChannelAttributesId : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        private const string ColumnName = "channel_attributes_id";
        
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{ColumnName}') then
	                ALTER TABLE {TableName} ADD COLUMN {ColumnName} int8 null REFERENCES channel_attributes(id);
                end if;
                end;
                $do$");
        }
    }
}