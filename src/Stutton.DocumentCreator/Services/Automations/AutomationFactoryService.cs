using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stutton.DocumentCreator.Automations;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Automations
{
    public class AutomationFactoryService : IAutomationFactoryService
    {
        private readonly Func<Type, AutomationModelBase> _automationResolver;

        private Dictionary<string, Type> _automationTypes;

        public AutomationFactoryService(Func<Type, AutomationModelBase> automationResolver)
        {
            _automationResolver = automationResolver;
        }

        public async Task<IResponse<AutomationModelBase>> CreateAutomation(Type automationType)
        {
            try
            {
                if (!typeof(AutomationModelBase).IsAssignableFrom(automationType))
                {
                    return Response<AutomationModelBase>.FromFailure(
                        $"Type '{automationType.Name}' does not inherit from AutomationModelBase");
                }

                var automation = _automationResolver(automationType);
                if (automation == null)
                {
                    return Response<AutomationModelBase>.FromFailure($"Failed to create field of type '{automationType.Name}'");
                }

                if (automation is IRequiresInitialization initializeMe)
                {
                    await initializeMe.Initialize();
                }

                return Response<AutomationModelBase>.FromSuccess(automation);
            }
            catch (Exception ex)
            {
                return Response<AutomationModelBase>.FromException("Failed to create field for unknown reason", ex);
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
                var automationTypes = typeof(AutomationModelBase).Assembly.GetInheritingTypes<AutomationModelBase>();
                foreach (var automationType in automationTypes)
                {
                    if (_automationResolver(automationType) is AutomationModelBase automation)
                    {
                        _automationTypes.Add(automation.TypeDisplayName, automationType);
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
