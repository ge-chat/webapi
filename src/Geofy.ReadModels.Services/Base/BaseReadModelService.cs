using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Geofy.ReadModels.Services.Base
{
    public abstract class BaseReadModelServicece<T, TFilter>
        where TFilter : BaseFilter, new()
        where T : new()
    {
        protected readonly IMongoCollection<T> _items;

        protected BaseReadModelServicece(IMongoCollection<T> items)
        {
            _items = items;
        }

        public virtual Task<T> GetByIdAsync(string id, TFilter filter = null)
        {
            if (string.IsNullOrEmpty(id))
                return Task.FromResult(default(T));

            if (filter == null)
                filter = new TFilter();
            filter.EntityId = id;

            return GetCursorByFilter(filter).FirstOrDefaultAsync();
        }

        public Task<List<T>> GetAllAsync()
        {
            return GetCursorByFilter(new TFilter()).ToListAsync();
        }

        public Task<long> CountAsync(TFilter filter)
        {
            return GetCursorByFilter(filter).CountAsync();
        }

        public Task<bool> IsExists(TFilter filter)
        {
            return GetCursorByFilter(filter).AnyAsync();
        }

        public async Task<IEnumerable<T>> GetByFilter(TFilter filter)
        {
            var cursor = GetCursorByFilter(filter);
            if (!filter.IsPagingEnabled) return await cursor.ToListAsync();

            var pagingInfo = filter.PagingInfo;
            pagingInfo.TotalCount = await cursor.CountAsync();
            var list = await cursor.ToListAsync();
            pagingInfo.ActualLoadedItemCount = list.Count;
            return list;
        }

        protected IFindFluent<T, T> GetCursorByFilter(TFilter filter)
        {
            var filterBuilder = Builders<T>.Filter;

            var queries = GetFilterQueries(filter).ToList();
            //if filter was not applied we not return all documents, we just return empty list
            if (!queries.Any() && filter.PagingInfo == null && filter.Ordering == null)
                return null;

            var resQuery = queries.Count > 0 ? filterBuilder.And(queries.ToArray()) : filterBuilder.Empty;

            var cursor = _items.Find(resQuery);
            var sortOrder = BuildSortExpression(filter);
            if (sortOrder.Any())
                cursor.Sort(Builders<T>.Sort.Combine(sortOrder));
            if (!filter.IsPagingEnabled) return cursor;
            var pagingInfo = filter.PagingInfo;
            if (pagingInfo == null) return cursor;
            cursor.Skip(pagingInfo.Skip);
            cursor.Limit(pagingInfo.Take);

            return cursor;
        } 

        public async Task<PagedViewsResult<T>> GetPagedResultByFilter(TFilter filter)
        {
            var items = await GetByFilter(filter);

            return new PagedViewsResult<T>()
            {
                Items = items,
                PagingInfo = filter.PagingInfo
            };
        }

        private List<FilterDefinition<T>> GetFilterQueries(TFilter filter)
        {
            var builder = Builders<T>.Filter;
            var queries = new List<FilterDefinition<T>>();

            if (!string.IsNullOrEmpty(filter.EntityId))
                queries.Add(builder.Eq("_id", filter.EntityId));

            queries.AddRange(BuildFilterQuery(filter));

            return queries;
        }

        protected virtual List<SortDefinition<T>> BuildSortExpression(TFilter filter)
        {
            var sortItems = new List<SortDefinition<T>>();
            var builder = Builders<T>.Sort;
            if (filter.Ordering != null && filter.Ordering.Any())
            {
                sortItems = filter.Ordering
                    .Select(order => order.Desc ? builder.Descending(order.Key) : builder.Ascending(order.Key))
                    .ToList();
            }
            return sortItems;
        }

        public abstract IEnumerable<FilterDefinition<T>> BuildFilterQuery(TFilter filter);
    }
}