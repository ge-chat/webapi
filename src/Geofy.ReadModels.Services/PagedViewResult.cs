using System.Collections.Generic;

namespace Geofy.ReadModels.Services
{
    public class PagedViewsResult<T> where T : new()
    {
        public IEnumerable<T> Items { get; set; }

        public PagingInfo PagingInfo { get; set; }

    }
}