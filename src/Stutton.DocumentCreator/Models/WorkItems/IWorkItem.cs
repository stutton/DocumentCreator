using System.Collections.Generic;

namespace Stutton.DocumentCreator.Models.WorkItems
{
    public interface IWorkItem
    {
        int Id { get; }
        string Title { get; }
        string Description { get; }
        string Url { get; }
        string Area { get; }
        string Type { get; }
        string AssignedTo { get; }
        string State { get; }
        bool Selected { get; set; }
        int[] ChildWorkItems { get; }
    }
}
