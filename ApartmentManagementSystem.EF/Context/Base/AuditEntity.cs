namespace ApartmentManagementSystem.EF.Context.Base
{
    public abstract class AuditEntity<TKey> : EntityBase<TKey>, IAuditEntity<TKey>
    {
        public DateTime CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedByUserName { get; set; }
        
        public string? CreatedByUserDisplayName { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public string? UpdatedByUserName { get; set; }
        public string? UpdatedByUserDisplayName { get; set; }
    }
}
