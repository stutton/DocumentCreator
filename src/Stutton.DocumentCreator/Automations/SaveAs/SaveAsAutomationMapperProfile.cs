using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Automations.SaveAs
{
    public class SaveAsAutomationMapperProfile : Profile
    {
        public SaveAsAutomationMapperProfile()
        {
            CreateMap<SaveAsAutomationModel, SaveAsAutomationDto>()
                .IncludeBase<AutomationModelBase, AutomationDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
