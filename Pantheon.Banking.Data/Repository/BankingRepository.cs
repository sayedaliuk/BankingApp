using Microsoft.Extensions.Logging;

namespace Pantheon.Banking.Data.Repository
{
    public class BankingRepository : Repository<BankAccount, BankingDbContext>
    {
        public BankingRepository(BankingDbContext context, ILogger<BankingRepository> logger)
            : base(context, logger)
        {
            
        }         
    }
}
