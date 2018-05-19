using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents.Fields;

namespace Stutton.DocumentCreator.ViewModels.Documents.DocumentTemplateSteps
{
    public class FieldsStepViewModel
    {
        public ObservableCollection<IField> Fields { get; }

        public FieldsStepViewModel(ObservableCollection<IField> fields)
        {
            Fields = fields;
        }
    }
}
