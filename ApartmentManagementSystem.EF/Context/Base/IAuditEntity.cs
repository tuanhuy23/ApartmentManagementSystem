namespace ApartmentManagementSystem.EF.Context.Base
{
    public interface IAuditEntity
    {
        DateTime CreatedDate { get; set; }
        string CreatedBy { get; set; }
        string CreatedByUserName { get; set; }
        DateTime? UpdatedDate { get; set; }
        string? UpdatedBy { get; set; }
        string? UpdatedByUserName { get; set; }
    }
    public interface  IAuditEntity<TKey> : IAuditEntity, IEntityBase<TKey>
    {

    }
}
