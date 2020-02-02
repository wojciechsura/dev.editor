using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Search
{
    public class LastReplaceTexts : BaseTypedItemCollection<ReplaceText>
    {
        public const string NAME = "LastReplaceTexts";

        public LastReplaceTexts(BaseItemContainer parent) : base(NAME, parent)
        {
            ChildInfos = new List<BaseChildInfo>
            {
                new ChildInfo<ReplaceText>(ReplaceText.NAME, () => new ReplaceText())
            };
        }

        protected override IEnumerable<BaseChildInfo> ChildInfos { get; }
    }
}
