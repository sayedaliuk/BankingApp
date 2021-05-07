using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pantheon.Banking.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Pantheon.Banking.Data.Repository
{
    public class Repository<T,K> : IRepository<T>
        where T : EntityBase
        where K : DbContext
    {
        protected readonly ILogger<Repository<T, K>> _logger;      
        protected readonly K Context;
        protected DbSet<T> Set => Context.Set<T>();

        // ctor
        public Repository(K context, ILogger<Repository<T,K>> logger)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            try
            {
                if (filter == null)
                    filter = (x) => true;

                return Set.Where(filter).AsQueryable();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetAll)}");
                return (new T[0]).AsQueryable();
            }
        }

        public virtual T GetByKey(int id)
        {
            try
            {
                return Set.Find(id);                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(GetByKey)}");
                return default;
            }
        }

        public virtual Result<T> Save(T dto)
        {
            var ent = GetByKey(dto.Id);

            try
            {
                if (ent == default)
                {
                    try
                    {
                        Set.Add(ent);
                        Context.SaveChanges();
                    }
                    catch(Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to add record with Id = {dto?.Id}");
                        throw new OperationCanceledException();
                    }                    
                }
                else
                {
                    try
                    {
                        Context.Entry<T>(ent).CurrentValues.SetValues(dto);
                        Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to update record with Id = {dto?.Id}");
                        throw new OperationCanceledException();
                    }
                }

                return new Result<T>(true, ent);
            }
            catch (OperationCanceledException ex)
            {                
                return new Result<T>(false, default, new[] { ex.Message, ex.InnerException?.Message });                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(Save)}");
                return new Result<T>(false, default, new[] { ex.Message, ex.InnerException?.Message });                
            }
        }

        public virtual Result Delete(int id)
        {
            var ent = GetByKey(id);

            try
            {
                if (ent == default)
                    throw new OperationCanceledException($"Record does not exist for Id : {id}");

                Set.Remove(ent);
                Context.SaveChanges();

                return new Result<T>(true, ent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in {nameof(Delete)} while attempting to remove record with Id = {id}");
                return new Result<T>(false, default, new[] { ex.Message, ex.InnerException?.Message });
            }
        }
    }
}
