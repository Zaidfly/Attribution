using System;
using System.Threading;
using FluentMigrator;

namespace EnvironmentInitializer
{
    public class TestMigrationAttribute : MigrationAttribute
    {
        public static int DefaultVersionCounter;

        public TestMigrationAttribute() : this(null)
        {
        }
        
        public TestMigrationAttribute(long version) : this((long?)version)
        {
        }

        private TestMigrationAttribute(long? version) : base(CalculateVersion(version))
        {
        }

        private static long CalculateVersion(long? version)
        {
            var editedVersion = version ?? Interlocked.Increment(ref DefaultVersionCounter);
            //If versin less then current datetime, then we have a possibility of collision with product migrations
            if (DateToVersion(DateTime.Now) > editedVersion)
            {
                var maxVersionOrder = (long)Math.Pow(10, Math.Min(12L, GetNumberOrder(editedVersion)));
                var newVersion = DateToVersion(DateTime.Now.AddYears(1000));
                editedVersion = newVersion / maxVersionOrder * maxVersionOrder + editedVersion % maxVersionOrder;		
            }

            return editedVersion;
        }

        private static double GetNumberOrder(long number) => Math.Floor(Math.Log10(number) + 1);
        
        private static long DateToVersion(DateTime dateTime)
        {
            var version = dateTime.Year * 10000000000 + 
                          dateTime.Month * 100000000 + 
                          dateTime.Day * 1000000 + 
                          dateTime.Hour * 10000 + 
                          dateTime.Minute * 100 + 
                          dateTime.Second;

            return version;
        }
    }
}