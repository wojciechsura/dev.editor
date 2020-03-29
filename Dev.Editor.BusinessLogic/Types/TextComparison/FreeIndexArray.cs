using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Types.TextComparison
{
    internal class FreeIndexArray<T>
    {
        private readonly int startIndex;
        private readonly int endIndex;
        private readonly T[] buffer;

        public FreeIndexArray(int startIndex, int endIndex)
        {
            if (endIndex < startIndex)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            this.startIndex = startIndex;
            this.endIndex = endIndex;
            buffer = new T[endIndex - startIndex + 1];
        }

        public T this[int index]
        {
            get
            {
                if (index < startIndex || index > endIndex)
                    throw new ArgumentOutOfRangeException(nameof(index));

                return buffer[index - startIndex];
            }
            set
            {
                if (index < startIndex || index > endIndex)
                    throw new ArgumentOutOfRangeException(nameof(index));

                buffer[index - startIndex] = value;
            }
        }
    }
}
