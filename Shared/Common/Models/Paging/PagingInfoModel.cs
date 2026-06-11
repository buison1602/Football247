using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Models.Paging
{
    public class PagingInfoModel
    {
        public int PageSize { get; set; }

        public int Page { get; set; }

        public long TotalItems { get; set; }

        public PagingInfoModel()
        {
        }

        public PagingInfoModel(BaseQueryModel request, long totalItem)
        {
            TotalItems = totalItem;
            if (request != null)
            {
                Page = request.Page;
                PageSize = request.PageSize;
            }
        }
    }
}
