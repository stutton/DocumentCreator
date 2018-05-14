using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.WorkItems
{
    public class WorkItemViewModel : Observable
    {
        private int _id;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public string Url { get; }
        public string Area { get; }
        public ICommand Command { get; }
        public ICommand OpenUrlCommand { get; }
    }
}
