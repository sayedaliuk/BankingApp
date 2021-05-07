using System.ComponentModel.DataAnnotations;

namespace Pantheon.Banking.Data
{
    public class TransactionType : AuditbleEntity
    {
        [MaxLength(20)]
        [Required]
        public string Name { get; set; }
    }
}
