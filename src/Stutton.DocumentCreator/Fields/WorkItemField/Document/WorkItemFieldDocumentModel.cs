using System;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Document
{
    public sealed class WorkItemFieldDocumentModel : FieldDocumentModelBase
    {
        private readonly ITfsService _tfsService;
        public const string Key = "WorkItemField";
        public override string Description =>
            $"Replace '{TextToReplace}' with the value of '{SelectedField}' from the selected work item";
        public override string FieldKey => Key;

        private string _textToReplace;
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        private WorkItemFieldModel _selectedField;
        public WorkItemFieldModel SelectedField
        {
            get => _selectedField;
            set => Set(ref _selectedField, value);
        }

        public WorkItemFieldDocumentModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public override async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            try
            {
                var tfsServiceResponse = await _tfsService.GetWorkItemFieldValue(workItem.Id, SelectedField);
                if (!tfsServiceResponse.Success)
                {
                    return tfsServiceResponse;
                }
                var fieldValue = tfsServiceResponse.Value;

                await Task.Run(() => TextReplacer.SearchAndReplace(document, TextToReplace, fieldValue, false));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException(
                    $"Failed to replace '{TextToReplace}' with value of '{SelectedField}' field from work item '{workItem.Id}'",
                    ex);
            }
        }
    }
}
