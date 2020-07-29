namespace Dev.Editor.BusinessLogic.Models.FindInFiles
{ 
    public abstract class BaseSearchItem
    {
        public BaseSearchItem(string name)
        {
            Path = name;
        }

        public string Path { get; }
    }
}
