using System;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models.DocumentTemplates.Automations
{
    public interface IAutomation
    {
        string Name { get; }

        string Description { get; set; }

        Task<IResponse> Execute();
    }
}
