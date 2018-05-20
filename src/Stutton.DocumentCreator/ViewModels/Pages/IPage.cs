﻿using System.Threading.Tasks;
using Stutton.DocumentCreator.ViewModels.Toolbar;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public interface IPage
    {
        string PageKey { get; }
        int PageOrder { get; }
        string Title { get; }
        bool IsOnDemandPage { get; }
        bool IsInEditMode { get; }
        bool IsFullscreen { get; }
        bool IsBusy { get; }
        ToolbarOptions ToolBar { get; }
        Task NavigatedTo(object parameter);
    }
}