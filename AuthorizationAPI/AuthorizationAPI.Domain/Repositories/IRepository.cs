﻿using System.Linq.Expressions;

namespace AuthorizationAPI.Domain.Repositories
{
    public interface IRepository<T>
    {

        /// <returns>true if the element exists, and false if not</returns>
        public Task<bool> IsItemExistAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);

        /// <param name="predicate">Special predicate for element search</param>
        /// <returns>The element, if it was found in the database or null</returns>
        public IQueryable<T> GetItemsByCondition(Expression<Func<T, bool>> predicate, bool trackChanges);

        /// <returns>queryable items from the database</returns>
        public IQueryable<T> GetItems(bool trackChanges);

        /// <summary>
        /// Create item in database
        /// </summary>
        /// <returns>ID of the added element</returns>
        public void Create(T item);

        /// <summary>
        /// Update item in database
        /// </summary>
        public void Update(T item);

        /// <summary>
        /// Delete item from database
        /// </summary>
        /// <returns>true if complite and false if not</returns>
        public void Delete(T item);
    }
}
