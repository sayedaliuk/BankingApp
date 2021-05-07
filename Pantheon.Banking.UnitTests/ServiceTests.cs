using System;
using Xunit;
using NSubstitute;
using Pantheon.Banking.Data.Repository;
using Pantheon.Banking.Data;
using Pantheon.Banking.Service;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using db = Pantheon.Banking.Data;
using dom = Pantheon.Banking.Domain;
using Shouldly;
using Pantheon.Banking.Domain;

namespace Pantheon.Banking.UnitTests
{
    public class ServiceTests
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

        [Fact]
        public void Test_GetBankAccount_WorksCorrectly()
        {
            // Arrange
            var fakeRepository = Substitute.For<IRepository<db.BankAccount>>();
            var fakeRateSvc = Substitute.For<IExchangeRateService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakeRefDatarepo = Substitute.For<IReferenceDataRepository>();
            var fakeLogger = Substitute.For<ILogger<BankingService>>();

            var data = new List<db.BankAccount>()
            {
                new db.BankAccount()
                {
                    SortCode = "22",
                    AccountNo = "12",
                    AccountName = "Sayed",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,

                    AccountType = new AccountType
                    {
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "Sayed",
                        LastModified = DateTime.UtcNow,
                    },
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Id = 1,
                    BaseCurrency = new Currency
                    {
                        Symbol = "GBP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "British Pounds",
                        LastModified = DateTime.UtcNow
                    },

                    Transactions = new List<db.Transaction>()
                    {
                        new db.Transaction()
                        {
                            Value = 500,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 1,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123e1231",
                            Id =1,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 1,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Credit",
                            },
                        },
                         new db.Transaction()
                        {
                            Value = 200,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 2,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123",
                            Id = 2,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 2,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Debit",
                            },
                        }
                    }
                }
            }.AsQueryable();

            fakeRepository.GetAll(Arg.Any<Expression<Func<db.BankAccount, bool>>>()).Returns(data);
            fakeMapper.Map<db.BankAccount, dom.BankAccount>(Arg.Any<db.BankAccount>()).Returns(GetBankAccount(data.FirstOrDefault()));

            var bankingSvc = new BankingService(fakeRepository,
                fakeRefDatarepo,
                fakeRateSvc,
                fakeMapper,
                fakeLogger);

            // Act
            var bankDetails = bankingSvc.GetAccountDetails("12", "22");

            // Assert
            bankDetails.Data.Balance.ShouldBe(300);
            bankDetails.Data.AccountName.ShouldBe("Sayed");
            bankDetails.Data.SortCode.ShouldBe("22");
            bankDetails.Data.AccountNo.ShouldBe("12");
        }

        [Fact]
        public void Test_WithdrawFunds_When_Withdrawal_Amount_Is_LessThan_Balance()
        {
            // Arrange
            var fakeRepository = Substitute.For<IRepository<db.BankAccount>>();
            var fakeRateSvc = Substitute.For<IExchangeRateService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakeRefDatarepo = Substitute.For<IReferenceDataRepository>();
            var fakeLogger = Substitute.For<ILogger<BankingService>>();

            var data = new List<db.BankAccount>()
            {
                new db.BankAccount()
                {
                    SortCode = "22",
                    AccountNo = "12",
                    AccountName = "Sayed",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,

                    AccountType = new AccountType
                    {
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "Sayed",
                        LastModified = DateTime.UtcNow,
                    },
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Id = 1,
                    BaseCurrency = new Currency
                    {
                        Symbol = "GBP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "British Pounds",
                        LastModified = DateTime.UtcNow
                    },

                    Transactions = new List<db.Transaction>()
                    {
                        new db.Transaction()
                        {
                            Value = 500,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 1,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123e1231",
                            Id =1,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 1,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Credit",
                            },
                        },
                         new db.Transaction()
                        {
                            Value = 200,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 2,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123",
                            Id = 2,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 2,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Debit",
                            },
                        }
                    }
                }
            }.AsQueryable();

            var tType = new List<TransactionType>()
            {
                new TransactionType()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Debit",
                }

            }.AsQueryable();

            fakeRepository.GetAll(Arg.Any<Expression<Func<db.BankAccount, bool>>>()).Returns(data);
            fakeRepository.Save(Arg.Any<db.BankAccount>()).Returns(new Result<db.BankAccount>(true, new db.BankAccount(), null));
            fakeMapper.Map<db.BankAccount, dom.BankAccount>(Arg.Any<db.BankAccount>()).Returns(GetBankAccount(data.FirstOrDefault()));
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.TransactionType, bool>>>()).Returns(tType);

            var bankingSvc = new BankingService(fakeRepository,
                fakeRefDatarepo,
                fakeRateSvc,
                fakeMapper,
                fakeLogger);

            // Act
            var bankDetails = bankingSvc.WithdrawFundsFromAccount("11", "22", 100, "GBP").Result;

            // Assert
            bankDetails.Data.Value.ShouldBe(200);
        }

        [Fact]
        public void Test_WithdrawFunds_When_Withdrawal_Amount_Is_Greater_Balance()
        {
            // Arrange
            var fakeRepository = Substitute.For<IRepository<db.BankAccount>>();
            var fakeRateSvc = Substitute.For<IExchangeRateService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakeRefDatarepo = Substitute.For<IReferenceDataRepository>();
            var fakeLogger = Substitute.For<ILogger<BankingService>>();

            var data = new List<db.BankAccount>()
            {
                new db.BankAccount()
                {
                    SortCode = "22",
                    AccountNo = "12",
                    AccountName = "Sayed",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,

                    AccountType = new AccountType
                    {
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "Sayed",
                        LastModified = DateTime.UtcNow,
                    },
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Id = 1,
                    BaseCurrency = new Currency
                    {
                        Symbol = "GBP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "British Pounds",
                        LastModified = DateTime.UtcNow
                    },

                    Transactions = new List<db.Transaction>()
                    {
                        new db.Transaction()
                        {
                            Value = 500,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 1,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123e1231",
                            Id =1,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 1,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Credit",
                            },
                        },
                         new db.Transaction()
                        {
                            Value = 200,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 2,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123",
                            Id = 2,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 2,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Debit",
                            },
                        }
                    }
                }
            }.AsQueryable();

            var tType = new List<TransactionType>()
            {
                new TransactionType()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Debit",
                }

            }.AsQueryable();

            fakeRepository.GetAll(Arg.Any<Expression<Func<db.BankAccount, bool>>>()).Returns(data);
            fakeRepository.Save(Arg.Any<db.BankAccount>()).Returns(new Result<db.BankAccount>(true, new db.BankAccount(), null));
            fakeMapper.Map<db.BankAccount, dom.BankAccount>(Arg.Any<db.BankAccount>()).Returns(GetBankAccount(data.FirstOrDefault()));
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.TransactionType, bool>>>()).Returns(tType);

            var bankingSvc = new BankingService(fakeRepository,
                fakeRefDatarepo,
                fakeRateSvc,
                fakeMapper,
                fakeLogger);

            // Act
            var bankDetails = bankingSvc.WithdrawFundsFromAccount("11", "22", 900, "GBP").Result;

            // Assert
            bankDetails.Errors.FirstOrDefault().ShouldContain("Insufficient funds in the account");
        }

        [Fact]
        public void Test_WithdrawFunds_When_Withdrawal_Currency_Is_Not_BaseCurrency()
        {
            // Arrange
            var fakeRepository = Substitute.For<IRepository<db.BankAccount>>();
            var fakeRateSvc = Substitute.For<IExchangeRateService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakeRefDatarepo = Substitute.For<IReferenceDataRepository>();
            var fakeLogger = Substitute.For<ILogger<BankingService>>();

            var data = new List<db.BankAccount>()
            {
                new db.BankAccount()
                {
                    SortCode = "22",
                    AccountNo = "12",
                    AccountName = "Sayed",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,

                    AccountType = new AccountType
                    {
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "Sayed",
                        LastModified = DateTime.UtcNow,
                    },
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Id = 1,
                    BaseCurrency = new Currency
                    {
                        Symbol = "GBP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "British Pounds",
                        LastModified = DateTime.UtcNow
                    },

                    Transactions = new List<db.Transaction>()
                    {
                        new db.Transaction()
                        {
                            Value = 500,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 1,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123e1231",
                            Id =1,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 1,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Credit",
                            },
                        },
                         new db.Transaction()
                        {
                            Value = 200,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 2,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123",
                            Id = 2,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 2,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Debit",
                            },
                        }
                    }
                }
            }.AsQueryable();

            var tType = new List<TransactionType>()
            {
                new TransactionType()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Debit",
                }

            }.AsQueryable();

            var curr = new List<Currency>()
            {
                new Currency()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Swiss Franc",
                    Symbol = "CHF"
                }

            }.AsQueryable();

            fakeRepository.GetAll(Arg.Any<Expression<Func<db.BankAccount, bool>>>()).Returns(data);
            fakeRepository.Save(Arg.Any<db.BankAccount>()).Returns(new Result<db.BankAccount>(true, new db.BankAccount(), null));
            fakeMapper.Map<db.BankAccount, dom.BankAccount>(Arg.Any<db.BankAccount>()).Returns(GetBankAccount(data.FirstOrDefault()));
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.TransactionType, bool>>>()).Returns(tType);
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.Currency, bool>>>()).Returns(curr);
            fakeRateSvc.GetExchangeRate(Arg.Any<string>(), Arg.Any<string>()).Returns(new Result<double?>(true, .09, null));

            var bankingSvc = new BankingService(fakeRepository,
                fakeRefDatarepo,
                fakeRateSvc,
                fakeMapper,
                fakeLogger);

            // Act
            var bankDetails = bankingSvc.WithdrawFundsFromAccount("11", "22", 100, "CHF").Result;

            // Assert
            bankDetails.Data.ShouldBe(300 - (100 * .09));
        }


        [Fact]
        public void Test_DepositFunds_When_Base_Currency_Is_Same_As_Deposit_Currency()
        {
            // Arrange
            var fakeRepository = Substitute.For<IRepository<db.BankAccount>>();
            var fakeRateSvc = Substitute.For<IExchangeRateService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakeRefDatarepo = Substitute.For<IReferenceDataRepository>();
            var fakeLogger = Substitute.For<ILogger<BankingService>>();

            var data = new List<db.BankAccount>()
            {
                new db.BankAccount()
                {
                    SortCode = "22",
                    AccountNo = "12",
                    AccountName = "Sayed",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,

                    AccountType = new AccountType
                    {
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "Sayed",
                        LastModified = DateTime.UtcNow,
                    },
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Id = 1,
                    BaseCurrency = new Currency
                    {
                        Symbol = "GBP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "British Pounds",
                        LastModified = DateTime.UtcNow
                    },

                    Transactions = new List<db.Transaction>()
                    {
                        new db.Transaction()
                        {
                            Value = 500,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 1,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123e1231",
                            Id =1,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 1,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Credit",
                            },
                        },
                         new db.Transaction()
                        {
                            Value = 200,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 2,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123",
                            Id = 2,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 2,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Debit",
                            },
                        }
                    }
                }
            }.AsQueryable();

            var tType = new List<TransactionType>()
            {
                new TransactionType()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Credit",
                }

            }.AsQueryable();

            fakeRepository.GetAll(Arg.Any<Expression<Func<db.BankAccount, bool>>>()).Returns(data);
            fakeRepository.Save(Arg.Any<db.BankAccount>()).Returns(new Result<db.BankAccount>(true, new db.BankAccount(), null));
            fakeMapper.Map<db.BankAccount, dom.BankAccount>(Arg.Any<db.BankAccount>()).Returns(GetBankAccount(data.FirstOrDefault()));
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.TransactionType, bool>>>()).Returns(tType);

            var bankingSvc = new BankingService(fakeRepository,
                fakeRefDatarepo,
                fakeRateSvc,
                fakeMapper,
                fakeLogger);

            // Act
            var bankDetails = bankingSvc.DepositFundsInAccount("11", "22", 100, "GBP").Result;

            // Assert 
            bankDetails.Data.ShouldBe(400);
        }

        [Fact]
        public void Test_DepositFunds_When_Base_Currency_Is_Not_Same_As_Deposit_Currency()
        {
            // Arrange
            var fakeRepository = Substitute.For<IRepository<db.BankAccount>>();
            var fakeRateSvc = Substitute.For<IExchangeRateService>();
            var fakeMapper = Substitute.For<IMapper>();
            var fakeRefDatarepo = Substitute.For<IReferenceDataRepository>();
            var fakeLogger = Substitute.For<ILogger<BankingService>>();

            var data = new List<db.BankAccount>()
            {
                new db.BankAccount()
                {
                    SortCode = "22",
                    AccountNo = "12",
                    AccountName = "Sayed",
                    IsActive = true,
                    Created = DateTime.UtcNow,
                    LastModified = DateTime.UtcNow,

                    AccountType = new AccountType
                    {
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "Sayed",
                        LastModified = DateTime.UtcNow,
                    },
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow,
                    Id = 1,
                    BaseCurrency = new Currency
                    {
                        Symbol = "GBP",
                        Created = DateTime.UtcNow,
                        IsActive = true,
                        Id = 1,
                        Name = "British Pounds",
                        LastModified = DateTime.UtcNow
                    },

                    Transactions = new List<db.Transaction>()
                    {
                        new db.Transaction()
                        {
                            Value = 500,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 1,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123e1231",
                            Id =1,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 1,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Credit",
                            },
                        },
                         new db.Transaction()
                        {
                            Value = 200,
                            Created = DateTime.UtcNow,
                            IsActive = true,
                            TargetCurrency = new Currency
                            {
                                IsActive = true,
                                Created = DateTime.UtcNow,
                                LastModified = DateTime.UtcNow,
                                Id = 1,
                                Name = "GBP",
                                Symbol = "GBP",
                            },

                            BaseCurrency = new Currency
                            {
                                Created = DateTime.UtcNow,
                                Symbol = "GBP",
                                Id = 2,
                                IsActive = true,
                                Name = "GBP",
                                LastModified = DateTime.Now
                            } ,
                            TransactionNumber = "123",
                            Id = 2,
                            Timestamp = DateTime.UtcNow,
                            LastModified = DateTime.UtcNow,
                            TransactionType = new TransactionType
                            {
                                Created = DateTime.UtcNow,
                                Id = 2,
                                IsActive = true,
                                LastModified = DateTime.UtcNow,
                                Name = "Debit",
                            },
                        }
                    }
                }
            }.AsQueryable();

            var tType = new List<TransactionType>()
            {
                new TransactionType()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Credit",
                }

            }.AsQueryable();

            var curr = new List<Currency>()
            {
                new Currency()
                {
                    Created = DateTime.UtcNow,
                    Id = 2,
                    IsActive = true,
                    LastModified = DateTime.UtcNow,
                    Name = "Swiss Franc",
                    Symbol = "CHF"
                }

            }.AsQueryable();

            fakeRepository.GetAll(Arg.Any<Expression<Func<db.BankAccount, bool>>>()).Returns(data);
            fakeRepository.Save(Arg.Any<db.BankAccount>()).Returns(new Result<db.BankAccount>(true, new db.BankAccount(), null));
            fakeMapper.Map<db.BankAccount, dom.BankAccount>(Arg.Any<db.BankAccount>()).Returns(GetBankAccount(data.FirstOrDefault()));
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.TransactionType, bool>>>()).Returns(tType);
            fakeRefDatarepo.GetRefData(Arg.Any<Expression<Func<db.Currency, bool>>>()).Returns(curr);
            fakeRateSvc.GetExchangeRate(Arg.Any<string>(), Arg.Any<string>()).Returns(new Result<double?>(true, .09, null));

            var bankingSvc = new BankingService(fakeRepository,
                fakeRefDatarepo,
                fakeRateSvc,
                fakeMapper,
                fakeLogger);

            // Act
            var bankDetails = bankingSvc.DepositFundsInAccount("11", "22", 100, "CHF").Result;

            // Assert
            bankDetails.Data.ShouldBe(300 + (100 * .09));
        }
    }
}
