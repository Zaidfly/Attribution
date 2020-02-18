using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    /// <summary>
    /// Колонка с долгоживущим значением куки с хэшем для channel_attributes (90 дней на момент создания колонки)
    /// </summary>
    [Migration(20191017123200)]
    [Tags("Pre")]
    public class Migration20191017123200_UserActions_AddColumn_LingeringHash : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        private const string ColumnName = "lingering_hash";
        
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