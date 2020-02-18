using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190904190000)]
    [Tags("Pre")]
    public class Migration20190904190000_UserActions_AddColumn_ObjectGuid : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        private const string ColumnName = "object_guid";
        
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{ColumnName}') then
	                ALTER TABLE {TableName} ADD COLUMN {ColumnName} uuid null;
                end if;
                end;
                $do$");
        }
    }
}