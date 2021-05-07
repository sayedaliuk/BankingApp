using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pantheon.Banking.Domain;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace Pantheon.Banking.Service
{
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly ILogger<ExchangeRateService> _logger;
        private readonly IApiService _apiService;
        private readonly IConfiguration _config;

        // ctor
        public ExchangeRateService(IApiService apiService, IConfiguration config, ILogger<ExchangeRateService> logger)
        {
            _apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _config = config ?? throw new ArgumentNullException(nameof(config));            
        }

        public async Task<Result<double?>> GetExchangeRate(string baseCurr, string targetCurr)
        {
            try
            {                
                var endpoint = _config["RatesApiEndpoint"]?.ToString();

                var exchangeRateEndpoint = $"{endpoint}/latest?base={baseCurr}&symbols={targetCurr}";
                var apiRes = await _apiService.Get<ExchangeRate>(exchangeRateEndpoint);

                var rate = apiRes.Rates
                            .Select(x => new { Currency = x.Key, Rate = x.Value })
                            .FirstOrDefault(x => string.Equals(targetCurr, x.Currency, StringComparison.InvariantCultureIgnoreCase))?.Rate;

                if (rate == null)
                    throw new OperationCanceledException("Unknown Rate. External API failed to return the exchange rate");

                return new Result<double?>(true, rate);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error in { nameof(GetExchangeRate) }");
                return new Result<double?>(false, default, new[] { "Failed to get rate from external API", ex.Message });
            }
        }
    }
}
