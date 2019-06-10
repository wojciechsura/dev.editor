using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Configuration
{
    public class ConfigCollection<T> : BaseTypedItemCollection<T> where T : BaseCollectionItem, new()
    {
        private readonly List<ChildInfo<T>> childInfos;

        protected override IEnumerable<BaseChildInfo> ChildInfos
        {
            get
            {
                return childInfos;
            }
        }

        public ConfigCollection(string xmlName, BaseItemContainer parent, string childName)
            : base(xmlName, parent)
        {
            childInfos = new List<ChildInfo<T>>
            {
                new ChildInfo<T>(childName, () => new T())
            };
        }
    }
}
