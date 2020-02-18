using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190724191300)]
    [Tags("Pre")]
    public class Migration20190724191300_UserActions_RenameColumns : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        
        public override void Up()
        {
            RenameColumn("action_date", "action_datetime_utc", "TIMESTAMP");
            RenameColumn("action_type", "action_type_id", "int4");
            
            Execute.Sql(@"-- noinspection SqlNoDataSourceInspectionForFile

				CREATE UNIQUE INDEX IF NOT EXISTS 
					uix_user_actions_user_id_action_type_tmp 
					ON user_actions (user_id, action_type, object_type, object_id);");
        }

        private void RenameColumn(string oldColumnName, string newColumnName, string pgsqlType)
        {
            Execute.Sql($@"

                DO
                $do$
                BEGIN
                if exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{oldColumnName}') 
                    and not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{newColumnName}') then
					    
                        ALTER TABLE {TableName} RENAME {oldColumnName} TO {newColumnName};
                end if;
                end;
                $do$");

            Execute.Sql($@"

                DO
                $do$
                BEGIN
                if not exists (SELECT FROM information_schema.columns WHERE table_name = '{TableName}' AND column_name = '{oldColumnName}') then				 
					ALTER TABLE {TableName} ADD COLUMN {oldColumnName} {pgsqlType} null;
                end if;
                end;
                $do$");

            Execute.Sql($@"

                CREATE OR REPLACE FUNCTION {TableName}_insert_{oldColumnName}_tmp()
                  RETURNS trigger AS
                $BODY$
                BEGIN
                    new.{newColumnName} := new.{oldColumnName};
                    new.{oldColumnName} := null;
                    RETURN new;
                END;
                $BODY$
                LANGUAGE plpgsql;");

            Execute.Sql($@"

                DO
                $do$
                BEGIN
                if not exists (SELECT * FROM information_schema.triggers WHERE event_object_table = '{TableName}' AND trigger_name = '{TableName}_insert_{oldColumnName}_tmp') then

                        CREATE TRIGGER {TableName}_insert_{oldColumnName}_tmp
                        BEFORE INSERT
                        ON {TableName}
                        FOR EACH ROW
                        EXECUTE PROCEDURE {TableName}_insert_{oldColumnName}_tmp();

                end if;
                end;
                $do$");
        }
    }
}