using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Pantheon.Banking.Service;
using Pantheon.Banking.Web.UI.Dto;

namespace Pantheon.Banking.Web.UI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly ILogger<BankAccountController> _logger;
        private readonly IBankingService _bankingSvc;

        // ctor
        public BankAccountController(IBankingService bankingService, 
            ILogger<BankAccountController> logger)
        {
            _logger = logger;
            _bankingSvc = bankingService;
        }

        [HttpGet]
        public IActionResult Get(string accountNumber, string sortCode)
        {
            try
            {
                var res = _bankingSvc.GetAccountDetails(accountNumber, sortCode);
                
                if(res.Success && res.Data != null)
                {
                    var dto = new BankAccountDetailResponse
                    {
                        AccountNumber =  accountNumber,
                        SortCode = sortCode,
                        Balance = res.Data.Balance.Value
                    };

                    return Ok(dto);
                }

                if (res.Errors.Any(x => x.Contains("Not found", StringComparison.InvariantCultureIgnoreCase)))
                    return StatusCode((int)HttpStatusCode.NotFound, string.Join(". ", res.Errors));

                else
                    return StatusCode((int)HttpStatusCode.InternalServerError, string.Join(". ", res.Errors));

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Exception in getting bank account details");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> DepositAmount([FromBody]AccountTransactionRequest dto)
        {
            try
            {
                if(dto == null || string.IsNullOrWhiteSpace(dto?.AccountNumber) || string.IsNullOrWhiteSpace(dto?.SortCode)
                    || dto.Amount <= 0)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Invalid input. Account Number, Sort Code and Amount are required");
                }

                var res = await _bankingSvc.DepositFundsInAccount(dto.AccountNumber, dto.SortCode, dto.Amount, dto.Currency);

                if (res.Success && res.Data != null)
                {
                    return Ok(res.Data);
                }

                if (res.Errors.Any(x => x.Contains("Not found", StringComparison.InvariantCultureIgnoreCase)))
                    return StatusCode((int)HttpStatusCode.NotFound, string.Join(". ", res.Errors));

                else
                    return StatusCode((int)HttpStatusCode.NotAcceptable, string.Join(". ", res.Errors));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in crediting the account");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> WithdrawAmount([FromBody]AccountTransactionRequest dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto?.AccountNumber) || string.IsNullOrWhiteSpace(dto?.SortCode)
                    || dto.Amount <= 0)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "Invalid input. Account Number, Sort Code and Amount are required");
                }

                var res = await _bankingSvc.WithdrawFundsFromAccount(dto.AccountNumber, dto.SortCode, dto.Amount, dto.Currency);

                if (res.Success && res.Data != null)
                {
                    return Ok(res.Data);
                }

                var errors = res?.Errors.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
                if (errors.Any(x => x.Contains("Not found", StringComparison.InvariantCultureIgnoreCase)))
                    return StatusCode((int)HttpStatusCode.NotFound, string.Join(". ", res.Errors));

                else if (errors.Any(x => x.Contains("Insufficient funds", StringComparison.InvariantCultureIgnoreCase)))
                    return StatusCode((int)HttpStatusCode.NotAcceptable, string.Join(". ", res.Errors));

                else
                    return StatusCode((int)HttpStatusCode.InternalServerError, string.Join(". ", res.Errors));

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in withdrawing from the account");
                return StatusCode((int)HttpStatusCode.InternalServerError, $"{ex.Message}");
            }
        }
    }
}
