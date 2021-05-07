using System.Threading.Tasks;

namespace Pantheon.Banking.Service
{
    public interface IApiService
    {
        Task<T> Get<T>(string endpoint);
    }
}
