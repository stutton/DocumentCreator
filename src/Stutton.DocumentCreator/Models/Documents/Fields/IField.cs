namespace Stutton.DocumentCreator.Models.Documents.Fields
{
    public interface IField
    {
        string Description { get; }
        string TypeDisplayName { get; }
        string FieldKey { get; }
        string TextToReplace { get; set; }
    }
}