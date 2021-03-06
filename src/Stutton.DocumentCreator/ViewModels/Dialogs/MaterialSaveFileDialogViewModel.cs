﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Dialogs
{
    public class MaterialSaveFileDialogViewModel : Observable
    {
        private string _selectedFile;

        public string SelectedFile
        {
            get => _selectedFile;
            set => Set(ref _selectedFile, value);
        }
    }
}
