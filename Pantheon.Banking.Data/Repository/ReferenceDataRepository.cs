using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Pantheon.Banking.Data.Repository
{
    public class ReferenceDataRepository : IReferenceDataRepository
    {
        private readonly BankingDbContext _ctx;
        private readonly ILogger<ReferenceDataRepository> _logger;

        public ReferenceDataRepository(BankingDbContext context, 
            ILogger<ReferenceDataRepository> logger)            
        {
            _ctx = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IQueryable<T> GetRefData<T>(Expression<Func<T, bool>> filter = null) where T: AuditbleEntity
        {
            try
            {
                if (filter == null)
                    filter = (x) => x.IsActive;                

                return _ctx.Set<T>().Where(filter).ToArray().AsQueryable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in { nameof(GetRefData) } for { typeof(T).Name }");
                return (new T[0]).AsQueryable();
            }
        }
    }
}
