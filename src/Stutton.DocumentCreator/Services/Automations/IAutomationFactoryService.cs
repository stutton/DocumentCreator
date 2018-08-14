using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Automations
{
    public interface IAutomationFactoryService
    {
        Task<IResponse<AutomationModelBase>> CreateAutomation(Type automationType);
        IResponse<Dictionary<string, Type>> GetAllAutomationKeys();
    }
}
