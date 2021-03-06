﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    public class WorkItemModel : Observable, IWorkItem
    {
        private int _id;
        private string _name;
        private string _description;
        private string _area;

        public int Id
        {
            get => _id;
            set => Set(ref _id, value);
        }

        public string Title
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string Description
        {
            get => _description;
            set => Set(ref _description, value);
        }

        public string Area
        {
            get => _area;
            set => Set(ref _area, value);
        }

        private string _type;

        public string Type
        {
            get => _type;
            set => Set(ref _type, value);
        }

        private string _assignedTo;

        public string AssignedTo
        {
            get => _assignedTo;
            set => Set(ref _assignedTo, value);
        }

        private string _state;

        public string State
        {
            get => _state;
            set => Set(ref _state, value);
        }

        private bool _selected;

        public bool Selected
        {
            get => _selected;
            set => Set(ref _selected, value);
        }

        private string _team;

        public string Team
        {
            get => _team;
            set => Set(ref _team, value);
        }

        private int[] _childWorkItems;

        public int[] ChildWorkItems
        {
            get => _childWorkItems;
            set => Set(ref _childWorkItems, value);
        }
    }
}
