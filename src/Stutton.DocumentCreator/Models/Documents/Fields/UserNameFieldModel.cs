using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Services.Tfs;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Documents.Fields
{
    public class UserNameFieldModel : Observable, IField
    {
        private readonly ITfsService _tfsService;
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

        public UserNameFieldModel(ITfsService tfsService)
        {
            _tfsService = tfsService;
        }

        public async Task<IResponse<string>> GetReplaceWithText()
        {
            var response = await _tfsService.GetUserProfileAsync();
            if (!response.Success)
            {
                return Response<string>.FromFailure(response.Message);
            }

            var profile = response.Value;
            return Response<string>.FromSuccess(profile.Name);
        }
    }
}
