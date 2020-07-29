namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{
    public class RootSearchItem : BaseSearchContainerItem
    {
        public RootSearchItem(string path, string searchPattern)
            : base(path)
        {
            SearchPattern = searchPattern;
        }

        public string SearchPattern { get; }
    }
}
