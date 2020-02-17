using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.ViewModels.Tools.Explorer
{
    internal static class FolderListHelper
    {
        public static void UpdateFolderList(IList<FolderItemViewModel> current, IList<FolderItemViewModel> updated)
        {
            int currentIndex = 0;
            int updatedIndex = 0;
            while (currentIndex < current.Count || updatedIndex < updated.Count)
            {
                int compared;
                if (currentIndex < current.Count && updatedIndex < updated.Count)
                {
                    // Elements on both sides exist, compare their paths
                    compared = current[currentIndex].Path.CompareTo(updated[updatedIndex].Path);
                }
                else if (currentIndex < current.Count)
                {
                    // Current element exists, updated doesn't - remove current
                    compared = -1;
                }
                else
                {
                    // Update element exists, current doesn't - add updated
                    compared = 1;
                }

                if (compared == 0)
                {
                    currentIndex++;
                    updatedIndex++;
                }
                else if (compared < 0)
                {
                    // Folder was removed
                    current.RemoveAt(currentIndex);
                }
                else
                {
                    // Folder was added
                    current.Insert(currentIndex, updated[updatedIndex]);
                }
            }
        }
    }
}
