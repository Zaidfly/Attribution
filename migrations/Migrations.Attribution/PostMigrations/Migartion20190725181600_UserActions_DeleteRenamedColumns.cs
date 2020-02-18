using FluentMigrator;
// ReSharper disable InconsistentNaming

namespace DatabaseMigrations.PostMigrations
{
    [Migration(20190725181600)]
    [Tags("Post")]
    public class Migartion20190725181600_UserActions_DeleteRenamedColumns : ForwardOnlyMigration
    {
        private const string TableName = "user_actions";
        
        public override void Up()
        {
            DropRenamedColumn("action_date");
            DropRenamedColumn("action_type");
            
            Execute.Sql($@"drop index if exists uix_user_actions_user_id_action_type_tmp;");
        }

        private void DropRenamedColumn(string oldColumnName)
        {
            Execute.Sql($@"DROP trigger IF EXISTS {TableName}_insert_{oldColumnName}_tmp on user_actions");
            Execute.Sql($@"DROP function IF EXISTS {TableName}_insert_{oldColumnName}_tmp");
            Execute.Sql($@"ALTER TABLE {TableName} DROP COLUMN IF EXISTS {oldColumnName};");
        }
    }
}