namespace ApartmentManagementSystem.EF.Context.Base
{
    public interface IEntityBase<TKey>
    {
        TKey Id { get; set; }
    }
}
