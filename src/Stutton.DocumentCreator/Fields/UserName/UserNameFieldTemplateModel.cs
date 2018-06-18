using System;
using System.Threading.Tasks;
using System.Windows.Input;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.UserName
{
    public class UserNameFieldTemplateModel : Observable, IFieldTemplate
    {
        public const string Key = "UserNameField";

        public string Description => $"Replace '{TextToReplace}' with the current user's name";
        public string TypeDisplayName => "Name";
        public string FieldKey => Key;
        private string _textToReplace;

        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public event EventHandler<IFieldTemplate> RequestDeleteMe;

        #region Delete Command

        private ICommand _deleteCommand;
        public ICommand DeleteCommand => _deleteCommand ?? (_deleteCommand = new RelayCommand(Delete));

        private void Delete()
        {
            RequestDeleteMe?.Invoke(this, this);
        }

        #endregion

        public async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem, IServiceResolver serviceResolver)
        {
            try
            {
                var serviceResponse = serviceResolver.Resolve<ITfsService>();
                if (!serviceResponse.Success)
                {
                    return Response.FromFailure(serviceResponse.Message);
                }

                var tfsService = serviceResponse.Value;
                var response = await tfsService.GetUserProfileAsync();
                if (!response.Success)
                {
                    return Response.FromFailure(response.Message);
                }

                var profile = response.Value;
                await Task.Run(() => TextReplacer.SearchAndReplace(document, TextToReplace, profile.Name, false));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException("Failed to repace text with username", ex);
            }
        }
    }
}
