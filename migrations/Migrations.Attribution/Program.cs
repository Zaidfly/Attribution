using FluentMigratorCore.Consolisator;

namespace Migration.Attribution
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            return RunMigrator.WithParameters(
                DbKind.Postges,
                "Host=ghost.local.com;Database=Attribution;Username=youdo;Password=youdo", 
                typeof(Program).Assembly, 
                args);
        }
    }
}