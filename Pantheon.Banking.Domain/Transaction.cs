using System;

namespace Pantheon.Banking.Domain
{
    public class Transaction
    {
        public int? Id { get; set; }
        public string TransactionNumber { get; set; }       
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public string TransactionType { get; set; }
    }
}
