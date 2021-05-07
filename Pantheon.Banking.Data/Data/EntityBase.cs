using System.ComponentModel.DataAnnotations;

namespace Pantheon.Banking.Data
{
    public class EntityBase : IEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
