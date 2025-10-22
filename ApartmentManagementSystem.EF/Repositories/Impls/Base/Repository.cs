using ApartmentManagementSystem.EF.Context.Base;
using ApartmentManagementSystem.EF.Repositories.Interfaces.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ApartmentManagementSystem.EF.Repositories.Impls.Base
{
    internal class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbFactory _dbFactory;
        private readonly UserAudit _userAudit;
        private DbSet<T> _dbSet;
        protected DbSet<T> DbSet
        {
            get => _dbSet ?? (_dbSet = _dbFactory.DbContext.Set<T>());
        }
        public Repository(DbFactory dbFactory, UserAudit userAudit)
        {
            _dbFactory = dbFactory;
            _userAudit = userAudit;
        }
        public async Task<T> Add(T entity)
        {
            if (typeof(IAuditEntity).IsAssignableFrom(typeof(T)))
            {
                if (typeof(IEntityBase<Guid>).IsAssignableFrom(typeof(T)))
                {
                    ((IEntityBase<Guid>)entity).Id = Guid.NewGuid();
                }
                ((IAuditEntity)entity).CreatedDate = DateTime.Now;
                ((IAuditEntity)entity).CreatedBy = _userAudit.UserId;
                ((IAuditEntity)entity).CreatedByUserName = _userAudit.UserName;
            }
            var newEntityEntry = await DbSet.AddAsync(entity);
            return newEntityEntry.Entity;
        }

        public async Task Add(IEnumerable<T> entities)
        {
            if (typeof(IAuditEntity).IsAssignableFrom(typeof(T)))
            {
                foreach (var entity in entities)
                {
                    if (typeof(IEntityBase<Guid>).IsAssignableFrom(typeof(T)))
                    {
                        ((IEntityBase<Guid>)entity).Id = Guid.NewGuid();
                    }
                    ((IAuditEntity)entity).CreatedDate = DateTime.Now;
                    ((IAuditEntity)entity).CreatedBy = _userAudit.UserId;
                    ((IAuditEntity)entity).CreatedByUserName = _userAudit.UserName;
                }
            }
            await DbSet.AddRangeAsync(entities);
        }

        public void Delete(T entity)
        {
            DbSet.Remove(entity);
        }
        public void Delete(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }

        public IQueryable<T> List(Expression<Func<T, bool>>? expression = null)
        {
            if (expression == null) return DbSet.AsQueryable();
            return DbSet.Where(expression);
        }

        public T Update(T entity)
        {
            if (typeof(IAuditEntity).IsAssignableFrom(typeof(T)))
            {
                ((IAuditEntity)entity).UpdatedDate = DateTime.Now;
                ((IAuditEntity)entity).UpdatedBy = _userAudit.UserId;
                ((IAuditEntity)entity).UpdatedByUserName = _userAudit.UserName;
            }
            var updateEntityEntry = DbSet.Update(entity);
            return updateEntityEntry.Entity;
        }
        public void Update(IEnumerable<T> entities)
        {
            if (typeof(IAuditEntity).IsAssignableFrom(typeof(T)))
            {
                foreach (var entity in entities)
                {
                    ((IAuditEntity)entity).UpdatedDate = DateTime.Now;
                    ((IAuditEntity)entity).UpdatedBy = _userAudit.UserId;
                    ((IAuditEntity)entity).UpdatedByUserName = _userAudit.UserName;
                }
            }
            DbSet.UpdateRange(entities);
        }

        public IQueryable<T> ListSplitQuery(Expression<Func<T, bool>>? expression = null)
        {
            if (expression == null) return DbSet.AsQueryable();
            return DbSet.AsSplitQuery().Where(expression);
        }
        public IQueryable<T> List()
        {
            return DbSet.AsQueryable();
        }
    }
}
