using System;

namespace Attribution.Configuration
{
    public class DbConnectionString
    {
        public DbConnectionString(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }
    }
}