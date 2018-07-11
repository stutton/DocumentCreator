using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.UserName.Document
{
    public class UserNameDocumentMapperProfile : Profile
    {
        public UserNameDocumentMapperProfile()
        {
            CreateMap<UserNameDocumentModel, UserNameDocumentDto>()
                .IncludeBase<FieldDocumentModelBase, FieldDocumentDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
