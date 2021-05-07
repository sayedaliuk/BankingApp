using System.ComponentModel.DataAnnotations;

namespace Pantheon.Banking.Data
{
    public class Currency : AuditbleEntity
    {
        [MaxLength(100)]
        [Required]
        public string Name { get; set; }

        [MaxLength(20)]
        [Required]
        public string Symbol { get; set; }
    }
}
