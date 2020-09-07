using System;
using System.Collections.Generic;
using System.Linq;

using Deemix.AutoLoader.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deemix.AutoLoader.Configuration.Provider
{
    public class DbConfigurationProvider : ConfigurationProvider
    {

        public DbConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        Action<DbContextOptionsBuilder> OptionsAction { get; }

        // Load config data from EF DB.
        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            OptionsAction(builder);

            using (var dbContext = new AppDbContext(builder.Options))
            {
                dbContext.Database.EnsureCreated();

                Data = !dbContext.ConfigValues.Any() ? CreateAndSaveDefaultValues(dbContext) : dbContext.ConfigValues.ToDictionary(c => c.Id, c => c.Value);
            }
        }


        public override void Set(string key, string value)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();

            OptionsAction(builder);

            using (var dbContext = new AppDbContext(builder.Options))
            {
                var configValue = dbContext.ConfigValues.Find(key);
                configValue.Value = value;

                dbContext.SaveChanges();
            }

            base.Set(key, value);
        }

        private static IDictionary<string, string> CreateAndSaveDefaultValues(AppDbContext dbContext)
        {
            var configValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "LastChangeId", "0" }
            };

            dbContext.ConfigValues.AddRange(configValues.Select(kvp => new ConfigValue()
            {
                Id = kvp.Key,
                Value = kvp.Value
            }).ToArray());

            dbContext.SaveChanges();

            return configValues;
        }

    }
}