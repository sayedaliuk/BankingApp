using Pantheon.Banking.Domain;
using System.Threading.Tasks;

namespace Pantheon.Banking.Service
{
    public interface IExchangeRateService
    {
        Task<Result<double?>> GetExchangeRate(string baseCurr, string targetCurr);
    }
}
