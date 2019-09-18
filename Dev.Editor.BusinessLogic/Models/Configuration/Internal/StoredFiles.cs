using Dev.Editor.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Models.Configuration.Internal
{
    public class StoredFiles : BaseTypedItemCollection<BaseStoredFile>
    {
        internal const string NAME = "StoredFiles";

        public StoredFiles(BaseItemContainer parent)
            : base(NAME, parent)
        {
            ChildInfos = new List<BaseChildInfo>
            {
                new ChildInfo<TextStoredFile>(TextStoredFile.NAME, () => new TextStoredFile()),
                new ChildInfo<HexStoredFile>(HexStoredFile.NAME, () => new HexStoredFile()),
                new ChildInfo<BinStoredFile>(BinStoredFile.NAME, () => new BinStoredFile())
            };
        }

        protected override IEnumerable<BaseChildInfo> ChildInfos { get; }
    }
}
