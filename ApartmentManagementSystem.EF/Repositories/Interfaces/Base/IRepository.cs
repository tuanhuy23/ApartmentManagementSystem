using System.Linq.Expressions;

namespace ApartmentManagementSystem.EF.Repositories.Interfaces.Base
{
    public interface IRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task Add(IEnumerable<T> entities);
        T Update(T entity);
        void Update(IEnumerable<T> entities);
        void Delete(T entity);
        void Delete(IEnumerable<T> entities);
        IQueryable<T> List(Expression<Func<T, bool>>? expression = null);
        IQueryable<T> ListSplitQuery(Expression<Func<T, bool>>? expression = null);
        IQueryable<T> List();
    }
}
