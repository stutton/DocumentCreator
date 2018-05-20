namespace Stutton.DocumentCreator.Models.Documents.Fields
{
    public interface IField
    {
        string Name { get; set; }
        string TypeDisplayName { get; }
        string FieldKey { get; }
        string TextToReplace { get; set; }
    }
}