using System;
using System.ComponentModel.DataAnnotations;

namespace Pantheon.Banking.Data
{
    public class AuditbleEntity : EntityBase
    {
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
