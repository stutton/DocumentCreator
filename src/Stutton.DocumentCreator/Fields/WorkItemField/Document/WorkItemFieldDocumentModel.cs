using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.WorkItemField.Document
{
    public class WorkItemFieldDocumentModel : IFieldDocument
    {
        private readonly ITfsService _tfsService;
        public const string Key = "WorkItemField";

        public string Name { get; set; }

        public string Description =>
            $"Replace '{TextToReplace}' with the value of '{SelectedField}' from the selected work item";
        public string TypeDisplayName => "Work Item Field";
        public string FieldKey => Key;
        public string TextToReplace { get; set; }
        public string SelectedField { get; set; }


        public WorkItemFieldDocumentModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }
        public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
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
