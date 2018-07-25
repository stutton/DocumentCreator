using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Automations
{
    public class AutomationsMapperProfile : Profile
    {
        public AutomationsMapperProfile()
        {
            CreateMap<AutomationModelBase, AutomationDtoBase>().ReverseMap();
        }
    }
}
