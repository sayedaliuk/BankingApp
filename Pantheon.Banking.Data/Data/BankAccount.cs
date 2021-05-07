using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pantheon.Banking.Data
{
    public class BankAccount : AuditbleEntity
    {   
        [MaxLength(50)]
        [Required]
        public string AccountNo { get; set; }

        [MaxLength(20)]
        [Required]
        public string SortCode { get; set; }

        [MaxLength(500)]
        [Required]
        public string AccountName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
        public virtual AccountType AccountType { get; set; }
        public virtual Currency BaseCurrency { get; set; }
    }
}
