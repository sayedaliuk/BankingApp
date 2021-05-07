using Pantheon.Banking.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Pantheon.Banking.Data.Repository
{
    public interface IRepository<T> 
        where T : EntityBase        
    {
        T GetByKey(int id);
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter);
        Result<T> Save(T entity);
        Result Delete(int id);
    }
}
