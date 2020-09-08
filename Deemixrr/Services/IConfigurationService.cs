using System.Threading.Tasks;

namespace Deemixrr.Services
{
    public interface IConfigurationService
    {

        Task<T> Get<T>(string key);
        Task Set(string key, object value);

    }
}