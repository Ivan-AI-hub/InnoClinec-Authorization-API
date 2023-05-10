using AuthorizationAPI.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Transactions;

namespace AuthorizationAPI.DAL.Abstracts
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
            var t = new CommittableTransaction(new TransactionOptions { IsolationLevel = IsolationLevel.Serializable });
            Context.Database.EnlistTransaction(t);
            return Context.Set<T>().AnyAsync(predicate, cancellationToken);
        }

        public IQueryable<T> GetItemsByCondition(Expression<Func<T, bool>> predicate, bool trackChanges)
        {
            return GetItems(trackChanges).Where(predicate);
        }

        public abstract IQueryable<T> GetItems(bool trackChanges);
    }
}
