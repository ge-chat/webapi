using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Geofy.ReadModels.Services
{
    public class BaseFilter
    {
        public string EntityId { get; set; }

        public PagingInfo PagingInfo { get; set; }

        public BaseFilter()
        {
        }

        public List<FilterOrder> Ordering { get; set; } = new List<FilterOrder>();

        public bool IsPagingEnabled => PagingInfo != null;

        public List<string> ExcludeFields { get; set; }

        /// <summary>
        /// Load only included fields from database
        /// </summary>
        public List<string> IncludeFields { get; set; }

        public void AddOrder(string key, bool desc = false)
        {
            Ordering.Add(new FilterOrder() { Key = key, Desc = desc });
        }

        public void AddOrder<T, TResult>(Expression<Func<T, TResult>> sortKeySelector, bool desc = false)
        {
            var s = sortKeySelector.Body.ToString();
            var key = s.Substring(sortKeySelector.Parameters[0].Name.Length + 1);
            AddOrder(key, desc);
        }
    }

    public class FilterOrder
    {
        public string Key { get; set; }
        public bool Desc { get; set; }
    }
}