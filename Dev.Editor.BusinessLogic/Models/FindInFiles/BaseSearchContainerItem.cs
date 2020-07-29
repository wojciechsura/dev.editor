using System.Collections;
using System.Collections.Generic;

namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{
    public abstract class BaseSearchContainerItem : BaseSearchItem, IEnumerable<BaseSearchItem>
    {
        protected readonly List<BaseSearchItem> items = new List<BaseSearchItem>();

        public BaseSearchContainerItem(string path) 
            : base(path)
        {
        }

        public void Add(BaseSearchItem item) => items.Add(item);

        public IEnumerator<BaseSearchItem> GetEnumerator() => ((IEnumerable<BaseSearchItem>)items).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<BaseSearchItem>)items).GetEnumerator();
    }
}
