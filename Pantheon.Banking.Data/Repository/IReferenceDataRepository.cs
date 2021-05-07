using System;
using System.Linq;
using System.Linq.Expressions;

namespace Pantheon.Banking.Data.Repository
{
    public interface IReferenceDataRepository
    {
        IQueryable<T> GetRefData<T>(Expression<Func<T, bool>> filter) where T : AuditbleEntity;
    }
}
