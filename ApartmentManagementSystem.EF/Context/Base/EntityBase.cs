using System.ComponentModel.DataAnnotations;

namespace ApartmentManagementSystem.EF.Context.Base
{
    public abstract class EntityBase<TKey> : IEntityBase<TKey>
    {
        [Key]
        public virtual TKey Id { get; set; }
    }
}
