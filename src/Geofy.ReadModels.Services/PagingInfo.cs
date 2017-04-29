﻿namespace Geofy.ReadModels.Services
{
    public class PagingInfo
    {
        public PagingInfo()
        {

        }

        public PagingInfo(int page = 1)
        {
            CurrentPage = page;
        }

        private int _itemsPerPage = 20;
        private int _skip = 0;

        public int Skip
        {
            get
            {
                if (_skip == 0)
                    _skip = (CurrentPage - 1) * _itemsPerPage;
                return _skip;
            }
            set
            {
                _skip = value;
            }
        }

        public int Take
        {
            get
            {
                return _itemsPerPage;
            }
            set
            {
                _itemsPerPage = value;
            }
        }

        /// <summary>
        /// Total count of items
        /// </summary>
        public long TotalCount { get; set; }

        /// <summary>
        /// Actual loaded items per page 
        /// </summary>
        public int ActualLoadedItemCount { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int ItemsPerPage
        {
            get
            {
                return _itemsPerPage;
            }
            set
            {
                _itemsPerPage = value;
            }
        }

        public int TotalPagesCount
        {
            get
            {
                return (int)(TotalCount / ItemsPerPage + ((TotalCount % ItemsPerPage > 0) ? 1 : 0));
            }
        }

        public int IndexOfFirstItem
        {
            get
            {
                return (CurrentPage - 1) * ItemsPerPage + 1;
            }
        }

        public int IndexOfLastItem
        {
            get
            {
                return (CurrentPage - 1) * ItemsPerPage + ActualLoadedItemCount;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (CurrentPage > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (CurrentPage < TotalPagesCount);
            }
        }
    }
}