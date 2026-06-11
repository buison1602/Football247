using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Common.Models.Paging
{
    public class PagingItemsModel<T>
    {
        public IList<T>? Items { get; set; }

        public PagingInfoModel? PagingInfo { get; set; }

        public PagingItemsModel()
        {
        }

        public PagingItemsModel(IList<T>? items, PagingInfoModel? pagingInfo)
        {
            Items = items;
            PagingInfo = pagingInfo;
        }

        public PagingItemsModel(IList<T>? items, BaseQueryModel? request, long totalItem)
        {
            Items = items;
            if (request != null)
            {
                PagingInfo = new PagingInfoModel(request, totalItem);
            }
        }
    }
}
