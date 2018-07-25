using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Automations.AttachToWorkItem
{
    public class AttachToWorkItemAutomationMapperProfile : Profile
    {
        public AttachToWorkItemAutomationMapperProfile()
        {
            CreateMap<AttachToWorkItemAutomationModel, AttachToWorkItemAutomationDto>()
                .IncludeBase<AutomationModelBase, AutomationDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
