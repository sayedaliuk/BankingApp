using System;
using System.ComponentModel.DataAnnotations;

namespace Pantheon.Banking.Data
{
    public class Transaction : AuditbleEntity
    {
        [MaxLength(500)]
        [Required]
        public string TransactionNumber { get; set; }
        
        [Required]
        public DateTime? Timestamp { get; set; }

        [Required]
        public double Value { get; set; }
                
        public virtual Currency BaseCurrency { get; set; }        
        public virtual Currency TargetCurrency { get; set; }
        
        [Required]
        public virtual TransactionType TransactionType { get; set; }
    }
}
