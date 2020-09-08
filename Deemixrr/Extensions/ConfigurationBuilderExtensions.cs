using System;

using Deemixrr.Configuration.Source;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deemixrr.Extensions
{
    public static class ConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddDbConfiguration(this IConfigurationBuilder builder, Action<DbContextOptionsBuilder> optionsAction)
        {
            return builder.Add(new DbConfigurationSource(optionsAction));
        }
    }
}