namespace ApartmentManagementSystem.EF.Repositories.Interfaces.Base
{
    public interface IUnitOfWork
    {
        Task<int> CommitAsync();
    }
}
