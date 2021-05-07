using System;

namespace Pantheon.Banking.Domain
{
    public class BankAccount
    {
        public string BaseCurrency { get; set; }
        public string AccountNo { get; set; }
        public string SortCode { get; set; }
        public string AccountName { get; set; }
        public string AccountType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }
        public double? Balance { get; set; }
    }
}
