namespace Dev.Editor.BusinessLogic.Services.Registry
{
    public interface IRegistryService
    {
        bool IsFileContextMenuEntryRegistered(string fileType, string shellKeyName);
        void RegisterFileContextMenuEntry(string fileType, string shellKeyName, string menuText, string menuCommand, string iconPath = null);
        void UnregisterFileContextMenuEntry(string fileType, string shellKeyName);
    }
}