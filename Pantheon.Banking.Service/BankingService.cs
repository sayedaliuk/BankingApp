using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantheon.Banking.Data;
using Pantheon.Banking.Data.Repository;
using Pantheon.Banking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using db = Pantheon.Banking.Data;
using dom = Pantheon.Banking.Domain;

namespace Pantheon.Banking.Service
{
    public class BankingService : IBankingService
    {
        protected readonly IRepository<db.BankAccount> _bankingRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<BankingService> _logger;
        private readonly IExchangeRateService _rateSvc;
        private readonly IReferenceDataRepository _refDataRepo;

        // ctor
        public BankingService(IRepository<db.BankAccount> bankingRepo,
            IReferenceDataRepository refDataRepository,
            IExchangeRateService exchangeRateService,
            IMapper mapper,
            ILogger<BankingService> logger)
        {           
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._rateSvc = exchangeRateService ?? throw new ArgumentNullException(nameof(exchangeRateService));
            this._bankingRepo = bankingRepo ?? throw new ArgumentNullException(nameof(bankingRepo));
            this._refDataRepo = refDataRepository ?? throw new ArgumentNullException(nameof(refDataRepository));
        }

        public Result<dom.BankAccount> GetAccountDetails(string accountNumber, string sortCode)
        {
            try
            {
                var accEnt = _bankingRepo.GetAll(x => x.AccountNo == accountNumber && x.SortCode == sortCode && x.IsActive)
                                        .Include(x => x.AccountType)
                                        .Include(x => x.BaseCurrency)
                                        .Include(x => x.Transactions)
                                           .ThenInclude(trn => trn.TransactionType)
                                        .FirstOrDefault();

                var balance = accEnt.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Credit).Sum(x => x.Value)
                                - accEnt.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Debit).Sum(x => x.Value);

                if (accEnt == null)                
                    throw new OperationCanceledException($"No account exists for accountNumber {accountNumber} and sortCode {sortCode}");


                var dto = _mapper.Map<db.BankAccount, dom.BankAccount>(accEnt);
                dto.Balance = balance;

                return new Result<dom.BankAccount>(true, dto);                                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get account details");
                return new Result<dom.BankAccount>(false, default, new[] { ex.Message, ex.InnerException?.Message });
            }
        }

        public async Task<Result<double?>> DepositFundsInAccount(string accountNumber, string sortCode, double sumToDeposit, string currency)
        {
            try
            {                
                var acc = _bankingRepo.GetAll(x => x.AccountNo == accountNumber && x.SortCode == sortCode && x.IsActive)
                                        .Include(x => x.Transactions).ThenInclude(x => x.TransactionType)
                                        .Include(x => x.AccountType)
                                        .Include(x => x.BaseCurrency)
                                        .FirstOrDefault();                                       

                if (acc == null)
                    throw new OperationCanceledException($"No active account record found with Account no = {accountNumber} and Sort code = {sortCode} ");
                
                var targetCurr = acc.BaseCurrency;        
                var baseCurr = (_refDataRepo.GetRefData<Currency>(x => x.Symbol == currency.Trim()).FirstOrDefault()) ?? targetCurr;

                double? rate = 1;
                if (!string.Equals(baseCurr.Symbol, targetCurr.Symbol, StringComparison.InvariantCultureIgnoreCase))
                {
                    var rateFromApiRes =  await _rateSvc.GetExchangeRate(baseCurr.Symbol, targetCurr.Symbol);
                    if (rateFromApiRes.Success && rateFromApiRes.Data.HasValue)
                    {
                        rate = rateFromApiRes.Data;                        
                    }
                    else
                    {
                        throw new OperationCanceledException(string.Join(". ", rateFromApiRes.Errors));
                    }                    
                }

                var tranType = _refDataRepo.GetRefData<db.TransactionType>(x => x.IsActive && x.Name == Constants.TranasctionType.Credit)
                                            .FirstOrDefault();
                var now = DateTime.UtcNow;

                var trn = new db.Transaction
                {                    
                    BaseCurrency = baseCurr,
                    TargetCurrency = targetCurr,
                    IsActive = true,
                    Timestamp = now,
                    Created = now,
                    TransactionType = tranType,
                    TransactionNumber = Guid.NewGuid().ToString(),
                    Value = sumToDeposit * rate.Value 
                };


                acc.Transactions.Add(trn);
                var saveRes = _bankingRepo.Save(acc);

                if (saveRes.Success)
                {                    
                    var updatedAcc = _bankingRepo
                                       .GetAll(x => x.AccountNo == accountNumber && x.SortCode == sortCode && x.IsActive)
                                       .Include(x => x.Transactions).ThenInclude(x => x.TransactionType)   
                                       .FirstOrDefault();

                    var balance = updatedAcc.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Credit).Sum(x => x.Value)
                                    - updatedAcc.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Debit).Sum(x => x.Value);

                    return new Result<double?>(true, balance);
                }
                else
                {
                    return new Result<double?>(false, null, saveRes.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deposit funds in account");
                return new Result<double?>(false, default, new[] { ex.Message, ex.InnerException?.Message });
            }
        }

        public async Task<Result<double?>> WithdrawFundsFromAccount(string accountNumber, string sortCode, double sumToWithdraw, string currency)
        {
            try
            {
                var acc = _bankingRepo.GetAll(x => x.AccountNo == accountNumber && x.SortCode == sortCode && x.IsActive)
                                        .Include(x => x.Transactions).ThenInclude(x => x.TransactionType)
                                        .Include(x => x.AccountType)
                                        .Include(x => x.BaseCurrency)
                                        .FirstOrDefault();

                if (acc == null)
                    throw new OperationCanceledException($"No active account record found with Account no = {accountNumber} and Sort code = {sortCode} ");

                var targetCurr = acc.BaseCurrency;
                var baseCurr = (_refDataRepo.GetRefData<Currency>(x => x.Symbol == currency.Trim()).FirstOrDefault()) ?? targetCurr;

                double? rate = 1;
                if (!string.Equals(baseCurr.Symbol, targetCurr.Symbol, StringComparison.InvariantCultureIgnoreCase))
                {
                    var rateFromApiRes = await _rateSvc.GetExchangeRate(baseCurr.Symbol, targetCurr.Symbol);
                    if (rateFromApiRes.Success && rateFromApiRes.Data.HasValue)
                    {
                        rate = rateFromApiRes.Data;
                    }
                    else
                    {
                        throw new OperationCanceledException(string.Join(". ", rateFromApiRes.Errors));
                    }
                }


                var balance = acc.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Credit).Sum(x => x.Value)
                                    - acc.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Debit).Sum(x => x.Value);

                var withdrawalAmountInBaseCurrency = sumToWithdraw * rate.Value;
                if (withdrawalAmountInBaseCurrency > balance)
                {
                    throw new InvalidOperationException($"Insufficient funds in the account. Withdrawal transaction cancelled. " +
                        $"Current Account Balance = {balance:N2} {acc.BaseCurrency.Symbol}. Withdrawal amount requested = {sumToWithdraw} {currency}. " +
                        $" Value of requested withdrawal amount in account base currency = {withdrawalAmountInBaseCurrency:N2} {acc.BaseCurrency.Symbol}");
                }



                var tranType = _refDataRepo.GetRefData<db.TransactionType>(x => x.IsActive && x.Name == Constants.TranasctionType.Debit)
                                            .FirstOrDefault();
                var now = DateTime.UtcNow;

                var trn = new db.Transaction
                {
                    BaseCurrency = baseCurr,
                    TargetCurrency = targetCurr,
                    IsActive = true,
                    Timestamp = now,
                    Created = now,
                    TransactionType = tranType,
                    TransactionNumber = Guid.NewGuid().ToString(),
                    Value = withdrawalAmountInBaseCurrency
                };


                acc.Transactions.Add(trn);
                var saveRes = _bankingRepo.Save(acc);

                if (saveRes.Success)
                {
                    var updatedAcc = _bankingRepo
                                       .GetAll(x => x.AccountNo == accountNumber && x.SortCode == sortCode && x.IsActive)
                                        .Include(x => x.Transactions).ThenInclude(x => x.TransactionType)
                                        .FirstOrDefault();                                          

                    var updatedBalance = updatedAcc.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Credit).Sum(x => x.Value)
                                            - updatedAcc.Transactions.Where(x => x.TransactionType.Name == Constants.TranasctionType.Debit).Sum(x => x.Value);

                    return new Result<double?>(true, updatedBalance);
                }
                else
                {
                    return new Result<double?>(false, null, saveRes.Errors);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to withdraw funds from the account");
                return new Result<double?>(false, default, new[] { ex.Message, ex.InnerException?.Message });
            }
        }
    }
}
