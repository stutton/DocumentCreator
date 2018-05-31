using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Models.Documents.Fields;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Documents.Steps
{
    public class FieldsStepViewModel : Observable
    {
        private ObservableCollection<IField> _fields;

        public ObservableCollection<IField> Fields
        {
            get => _fields;
            set => Set(ref _fields, value);
        }

        public FieldsStepViewModel(IEnumerable<IField> fields)
        {
            _fields = new ObservableCollection<IField>(fields);
        }
    }
}
