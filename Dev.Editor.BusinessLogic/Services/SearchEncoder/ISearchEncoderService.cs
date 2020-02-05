namespace Dev.Editor.BusinessLogic.Services.SearchEncoder
{
    public interface ISearchEncoderService
    {
        Models.Search.SearchReplaceModel SearchDescriptionToModel(Models.Search.SearchReplaceDescription description);
    }
}