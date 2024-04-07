using System;
using System.ComponentModel.DataAnnotations;

namespace Share.Domain.Entities
{
    public abstract class AuditableEntity : CreatableEntry
    {
        [ConcurrencyCheck]
        public Guid? LastModifiedBy { get; set; }
        [ConcurrencyCheck]
        public DateTime? LastModifiedAt { get; set; }
    }
}
