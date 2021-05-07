using System;
using System.Collections.Generic;
using dom = Pantheon.Banking.Domain;
using db = Pantheon.Banking.Data;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Pantheon.Banking.Service
{
    /// <summary>
    /// Source to target object mapping provider class. 
    /// Used for converting between any two objects of clases internal to Pantheon.Banking framework
    /// </summary>
    public class Mapper : IMapper
    {
        private readonly ILogger<Mapper> _logger;
        private Dictionary<MappingKey, Delegate> _registry;
        public string Key => "DefaultMapper";
        
        // ctor
        public Mapper(ILogger<Mapper> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            InitMappingRegistry();
        }

        /// <summary>
        /// Converts an object of type T to an object of type K
        /// </summary>
        public K Map<T, K>(T dto)
        {
            try
            {
                var key = GetKey<T, K>();

                if (!_registry.ContainsKey(key))
                    throw new NotImplementedException($"No mapper found from {typeof(T).Name} to {typeof(K).Name}");

                var func = _registry[key];
                var res = func.DynamicInvoke(dto);

                try
                {
                    var convertedRes = (K)res;
                    return convertedRes;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to convert output of registered mapper delegate to type of {typeof(K).Name}");
                    throw new OperationCanceledException();
                }
            }
            catch (OperationCanceledException ex)
            {
                // already handled and logged -  do nothing else                
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to map object of type {typeof(T).Name} to an object of type {typeof(K).Name}");
                return default;
            }
        }

        #region Private Methods

        private MappingKey GetKey<T, K>()
        {
            return new MappingKey(typeof(T), typeof(K));
        }

        /// <summary>
        /// Sets up the registry of mapper delegates for each pair of source and target data type
        /// </summary>
        private void InitMappingRegistry()
        {
            _registry = new Dictionary<MappingKey, Delegate>(new MappingKeyEqualityComparer())
            {
                {
                    GetKey<db.BankAccount, dom.BankAccount>() ,
                    (Func<db.BankAccount, dom.BankAccount>) EntityToDomainMappingsDelegates.GetBankAccount
                },
                {
                    GetKey<db.Transaction, dom.Transaction>() ,
                    (Func<db.Transaction, dom.Transaction>) EntityToDomainMappingsDelegates.GetTransaction
                }
            };
        }

        #endregion

        #region Delegates

        public class EntityToDomainMappingsDelegates
        {
            public static dom.BankAccount GetBankAccount(db.BankAccount dto)
            {
                if (dto == null)
                    return null;

                var retVal = new dom.BankAccount
                {
                    AccountName = dto.AccountName,
                    AccountNo = dto.AccountNo,
                    AccountType = dto.AccountType?.Name,
                    BaseCurrency = dto.BaseCurrency?.Symbol,
                    SortCode = dto.SortCode,
                    Balance = null,
                    EndDate = dto.EndDate,
                    StartDate = dto.StartDate
                };

                return retVal;
            }

            public static dom.Transaction GetTransaction(db.Transaction dto)
            {
                if (dto == null)
                    return null;

                var retVal = new dom.Transaction
                {
                    Id = dto.Id,
                    TransactionNumber = dto.TransactionNumber,
                    Timestamp = dto.Timestamp.Value,
                    BaseCurrency = dto.BaseCurrency?.Symbol,
                    TargetCurrency = dto.TargetCurrency?.Symbol,
                    TransactionType =  dto.TransactionType?.Name,
                    Value = dto.Value
                };

                return retVal;
            }
        }

        #endregion

        #region Private Classes

        private class MappingKeyEqualityComparer : IEqualityComparer<MappingKey>
        {
            public bool Equals([AllowNull] MappingKey x, [AllowNull] MappingKey y)
            {
                if (x == null || y == null)
                    return false;

                return (x.Source == y.Source) && (x.Target == y.Target);
            }

            public int GetHashCode([DisallowNull] MappingKey obj)
            {
                return obj.Source.GetHashCode() ^ obj.Target.GetHashCode();
            }
        }

        private class MappingKey
        {
            public Type Source { get; set; }
            public Type Target { get; set; }

            public MappingKey(Type source, Type target)
            {
                this.Source = source;
                this.Target = target;
            }
        }

        #endregion
    }
}
