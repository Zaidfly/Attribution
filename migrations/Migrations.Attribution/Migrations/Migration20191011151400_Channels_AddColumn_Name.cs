using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20191011151400)]
    [Tags("Pre")]
    public class Migration20191011151400_Channels_AddColumn_Name : ForwardOnlyMigration
    {
        private const string TableName = "channels";
        private const string ColumnName = "name";
        
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{ColumnName}') then
	                ALTER TABLE {TableName} ADD COLUMN {ColumnName} varchar(100) null;
                end if;
                end;
                $do$");
        }
    }
}