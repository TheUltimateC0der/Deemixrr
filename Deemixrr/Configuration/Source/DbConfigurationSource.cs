using System;

using Deemixrr.Configuration.Provider;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Deemixrr.Configuration.Source
{
    public class DbConfigurationSource : IConfigurationSource
    {

        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public DbConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DbConfigurationProvider(_optionsAction);
        }
    }
}