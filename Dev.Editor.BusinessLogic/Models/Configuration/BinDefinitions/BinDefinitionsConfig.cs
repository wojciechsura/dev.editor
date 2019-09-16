using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.BinDefinitions
{
    public class BinDefinitionsConfig : BaseTypedItemCollection<BinDefinition>
    {
        internal const string NAME = "StoredFiles";

        public BinDefinitionsConfig(BaseItemContainer parent)
            : base(NAME, parent)
        {
            ChildInfos = new List<BaseChildInfo>
            {
                new ChildInfo<BinDefinition>(BinDefinition.NAME, () => new BinDefinition())                
            };
        }

        protected override IEnumerable<BaseChildInfo> ChildInfos { get; }
    }
}
