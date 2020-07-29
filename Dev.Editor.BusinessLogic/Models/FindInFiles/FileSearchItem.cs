using System.Collections;
using System.Collections.Generic;

namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{
    public class FileSearchItem : BaseSearchItem, IEnumerable<ResultSearchItem>
    {
        private readonly List<ResultSearchItem> results = new List<ResultSearchItem>();

        public FileSearchItem(string path)
            : base(path)
        {
        }

        public void Add(ResultSearchItem result) => results.Add(result);

        public IEnumerator<ResultSearchItem> GetEnumerator() => ((IEnumerable<ResultSearchItem>)results).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<ResultSearchItem>)results).GetEnumerator();
    }
}
