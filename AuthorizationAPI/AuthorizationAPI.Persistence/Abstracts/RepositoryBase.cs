using AuthorizationAPI.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuthorizationAPI.Persistence.Abstracts
{
    public abstract class RepositoryBase<T> : IRepository<T>
        where T : class
    {
        protected AuthorizationContext Context;
        public RepositoryBase(AuthorizationContext context)
        {
            Context = context;
        }

        public virtual void Create(T item)
        {
            Context.Set<T>().Add(item);
        }
        public virtual void Update(T item)
        {
            Context.Set<T>().Update(item);
        }

        public virtual void Delete(T item)
        {
            Context.Set<T>().Remove(item);
        }

        public Task<bool> IsItemExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return Context.Set<T>().AnyAsync(predicate, cancellationToken);
        }

        public IQueryable<T> GetItemsByCondition(Expression<Func<T, bool>> predicate, bool trackChanges)
        {
            return GetItems(trackChanges).Where(predicate);
        }

        public abstract IQueryable<T> GetItems(bool trackChanges);
    }
}
