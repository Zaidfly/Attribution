using FluentMigrator;

namespace Migration.Attribution.Migrations
{
    [Migration(20190725173400)]
    [Tags("Pre")]
    public class Migration20190725173400_UserActions_ActionType_AddForeignKey : ForwardOnlyMigration
    {
        private const string ActionTypesTableName = "action_types";
        private const string UserActionsTableName = "user_actions";
        private const string FkConstraintName = "fk_user_actions_action_type";
        private const string FkColumn = "action_type_id";
        private const string ReferenceColumn = "id";
		
        public override void Up()
        {
            Execute.Sql($@"
                DO
                $do$
                BEGIN
                if not exists 
					(SELECT FROM information_schema.table_constraints 
						WHERE table_name = '{UserActionsTableName}' AND constraint_name = '{FkConstraintName}'
					) then
						ALTER TABLE {UserActionsTableName} ADD CONSTRAINT {FkConstraintName} FOREIGN KEY ({FkColumn}) REFERENCES {ActionTypesTableName} ({ReferenceColumn});
                end if;
                end;
                $do$");
        }
    }
}