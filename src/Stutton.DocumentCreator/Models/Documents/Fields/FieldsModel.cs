using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.Documents.Fields
{
    public class FieldsModel : Observable
    {
        
        public ObservableCollection<IField> Fields { get; } = new ObservableCollection<IField>();

        public FieldsModel()
        {
            
        }
    }
}
