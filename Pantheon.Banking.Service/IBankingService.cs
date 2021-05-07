using Pantheon.Banking.Domain;
using System;
using System.Threading.Tasks;

namespace Pantheon.Banking.Service
{
    public interface IBankingService
    {
        Result<BankAccount> GetAccountDetails(string accountNumber, string sortCode);
        Task<Result<double?>> WithdrawFundsFromAccount(string accountNumber, string sortCode, double sumToWithdraw, string currency);
        Task<Result<double?>> DepositFundsInAccount(string accountNumber, string sortCode, double sumToDeposit, string currency);
    }
}
