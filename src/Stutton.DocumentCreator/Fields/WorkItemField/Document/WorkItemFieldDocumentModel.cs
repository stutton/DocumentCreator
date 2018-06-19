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
        public string Name { get; }
        public string Description { get; }
        public string TypeDisplayName { get; }
        public string FieldKey { get; }
        public string TextToReplace { get; }
        public string SelectedField { get; }


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
