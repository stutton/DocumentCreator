using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.WorkItemField
{
    public class WorkItemFieldTemplateModel : Observable, IFieldTemplate, IRequiresInitialization
    {
        public const string Key = "WorkItemField";

        public string Description => $"Replace '{TextToReplace}' with the value of '{SelectedField}' from the selected work item";
        public string TypeDisplayName => "Work Item Field";
        public string FieldKey => Key;

        private ObservableCollection<string> _workItemFields;

        public ObservableCollection<string> WorkItemFields
        {
            get => _workItemFields;
            set => Set(ref _workItemFields, value);
        }

        private string _textToReplace;
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public event EventHandler<IFieldTemplate> RequestDeleteMe;

        private string _selectedField;
        public string SelectedField
        {
            get => _selectedField;
            set => Set(ref _selectedField, value);
        }

        #region Delete Command

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, this);
        }

        #endregion

        public async Task<IResponse> Initialize(IServiceResolver serviceResolver)
        {
            var serviceResolverResponse = serviceResolver.Resolve<ITfsService>();
            if (!serviceResolverResponse.Success)
            {
                return serviceResolverResponse;
            }

            var tfsService = serviceResolverResponse.Value;

            var tfsServiceResponse = await tfsService.GetWorkItemFields();
            if (!tfsServiceResponse.Success)
            {
                return tfsServiceResponse;
            }

            WorkItemFields = new ObservableCollection<string>(tfsServiceResponse.Value);
            return Response.FromSuccess();
        }

        public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
        {
            try
            {
                var serviceResolverResponse = serviceResolver.Resolve<ITfsService>();
                if (!serviceResolverResponse.Success)
                {
                    return serviceResolverResponse;
                }
                var tfsService = serviceResolverResponse.Value;

                var tfsServiceResponse = await tfsService.GetWorkItemFieldValue(workItem.Id, SelectedField);
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
