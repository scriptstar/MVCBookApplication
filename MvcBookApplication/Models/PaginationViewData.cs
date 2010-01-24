using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcBookApplication.Models
{
    public class PaginationViewData
    {
        public int PageNumber { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int TotalItemCount { get; set; }
        public string PageActionLink { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }

        public bool HasPreviousPage
        {
            get
            {
                return (PageNumber > 1);
            }
        }
        public bool HasNextPage
        {
            get
            {
                return (PageNumber * PageSize) <= TotalItemCount;
            }
        }


    }
}
