using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Vsts;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.UserName.Document
{
    [DataContract(Name = "UserNameField")]
    public class UserNameDocumentModel : FieldDocumentModelBase
    {
        
        public const string Key = "UserNameField";

        private readonly IVstsService _vstsService;

        public override string Description => $"Replace '{TextToReplace}' with the current user's name";
        public string TypeDisplayName => "Name";
        public override string FieldKey => Key;

        private string _textToReplace;
        [DataMember]
        public string TextToReplace
        {
            get => _textToReplace;
            set => Set(ref _textToReplace, value);
        }

        public UserNameDocumentModel(IVstsService vstsService)
        {
            _vstsService = vstsService;
        }

        public override async Task<IResponse> ModifyDocument(WordprocessingDocument document, IWorkItem workItem)
        {
            try
            {
                var response = await _vstsService.GetUserProfileAsync();
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
                return Response.FromException("Failed to add username to document", ex);
            }
        }
    }
}
