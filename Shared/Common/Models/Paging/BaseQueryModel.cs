using Refit;

namespace Shared.Common.Models.Paging
{
    public class BaseQueryModel
    {

        private string? _keyword;

        [AliasAs("keyword")]
        public string? Keyword
        {
            get
            {
                if (!string.IsNullOrEmpty(_keyword))
                {
                    return _keyword.Trim();
                }

                return _keyword;
            }
            set
            {
                _keyword = value;
            }
        }

        [AliasAs("page")]
        public int Page { get; set; } = 1;


        [AliasAs("pageSize")]
        public int PageSize { get; set; } = 20;

        public bool IsQueryAll { get; private set; }

        public BaseQueryModel()
        {
        }

        public BaseQueryModel(bool isQueryAll)
        {
            IsQueryAll = isQueryAll;
        }

        public void SetIsQueryAll(bool value)
        {
            IsQueryAll = value;
        }
    }
}
