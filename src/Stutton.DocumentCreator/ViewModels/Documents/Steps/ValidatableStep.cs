using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignExtensions.Model;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Documents.Steps
{
    public class ValidatableStep : Step
    {
        public override void Validate()
        {
            if (!(Content is IValidatable validatableContent))
            {
                HasValidationErrors = false;
                return;
            }

            if (validatableContent.Validate())
            {
                HasValidationErrors = false;
                return;
            }

            HasValidationErrors = true;
        }
    }
}
