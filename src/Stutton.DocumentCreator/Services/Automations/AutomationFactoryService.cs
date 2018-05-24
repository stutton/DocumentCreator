using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Stutton.DocumentCreator.Models.Documents.Automations;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Automations
{
    public class AutomationFactoryService : IAutomationFactoryService
    {
        private readonly Func<Type, IAutomation> _automationResolver;

        private Dictionary<string, Type> _automationTypes;

        public AutomationFactoryService(Func<Type, IAutomation> automationResolver)
        {
            _automationResolver = automationResolver;
        }

        public IResponse<IAutomation> CreateAutomation(Type automationType)
        {
            try
            {
                if (!typeof(IAutomation).IsAssignableFrom(automationType))
                {
                    return Response<IAutomation>.FromFailure(
                        $"Type '{automationType.Name}' does not inherit from IAutomation");
                }

                var automation = _automationResolver(automationType);
                if (automation == null)
                {
                    return Response<IAutomation>.FromFailure($"Failed to create field of type '{automationType.Name}'");
                }

                return Response<IAutomation>.FromSuccess(automation);
            }
            catch (Exception ex)
            {
                return Response<IAutomation>.FromException("Failed to create field for unknown reason", ex);
            }
        }

        public IResponse<Dictionary<string, Type>> GetAllAutomationKeys()
        {
            try
            {
                if (_automationTypes != null)
                {
                    return Response<Dictionary<string, Type>>.FromSuccess(_automationTypes);
                }

                _automationTypes = new Dictionary<string, Type>();
                var automationTypes = typeof(IAutomation).Assembly.GetInheritingTypes<IAutomation>();
                foreach (var automationType in automationTypes)
                {
                    if (_automationResolver(automationType) is IAutomation automation)
                    {
                        _automationTypes.Add(automation.Name, automationType);
                    }
                }

                return Response<Dictionary<string, Type>>.FromSuccess(_automationTypes);
            }
            catch (Exception ex)
            {
                return Response<Dictionary<string, Type>>.FromException("Failed to get all automation keys", ex);
            }
        }
    }
}
