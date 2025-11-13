namespace ApartmentManagementSystem.EF.Context.Base
{
    public interface ISoftDelete
    {
        public bool IsDeleted {get;set;}
    }
}