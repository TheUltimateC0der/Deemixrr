
using System;
using System.Threading.Tasks;

using Deemixrr.Data;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Deemixrr.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly AppDbContext _appDbContext;


        public ConfigurationService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<T> Get<T>(string key)
        {
            var result = await _appDbContext.ConfigValues.FirstOrDefaultAsync(x => x.Id == key);

            return result != null ? JsonConvert.DeserializeObject<T>(result.Value) : default(T);
        }

        public async Task Set(string key, object value)
        {
            var existingConfig = await _appDbContext.ConfigValues.FirstOrDefaultAsync(x => x.Id == key);

            if (existingConfig != null)
            {
                existingConfig.Value = JsonConvert.SerializeObject(value);
            }
            else
            {
                await _appDbContext.ConfigValues.AddAsync(new ConfigValue
                {
                    Id = key,
                    Value = JsonConvert.SerializeObject(value)
                });
            }

            await _appDbContext.SaveChangesAsync();
        }
    }
}