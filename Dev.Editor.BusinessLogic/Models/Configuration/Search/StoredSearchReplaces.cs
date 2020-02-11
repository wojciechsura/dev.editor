using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class StoredSearchReplaces : BaseTypedItemCollection<StoredSearchReplace>
    {
        public const string NAME = "StoredSearchReplaces";

        public StoredSearchReplaces(BaseItemContainer parent)
            : base(NAME, parent)
        {
            ChildInfos = new List<BaseChildInfo>
            {
                new ChildInfo<StoredSearchReplace>(StoredSearchReplace.NAME, () => new StoredSearchReplace())
            };
        }

        protected override IEnumerable<BaseChildInfo> ChildInfos { get; }
    }
}
