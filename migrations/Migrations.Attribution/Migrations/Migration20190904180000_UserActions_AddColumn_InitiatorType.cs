using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190904180000)]
    [Tags("Pre")]
    public class Migration20190904180000_UserActions_AddColumn_InitiatorType : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        private const string ColumnName = "initiator_type";
        
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{ColumnName}') then
	                ALTER TABLE {TableName} ADD COLUMN {ColumnName} int4 null;
                end if;
                end;
                $do$");
        }
    }
}