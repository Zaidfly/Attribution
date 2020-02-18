using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    /// <summary>
    /// Колонка с короткоживущим значением хэша channel_attributes (40 минут на момент создания колонки)
    /// </summary>
    [Migration(20191017124700)]
    [Tags("Pre")]
    public class Migration20191017124700_UserActions_AddColumn_ActualHash : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        private const string ColumnName = "actual_hash";
        
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{ColumnName}') then
	                ALTER TABLE {TableName} ADD COLUMN {ColumnName} int8 null;
                end if;
                end;
                $do$");
        }
    }
}