using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public class ListFieldDocumentMapperProfile : Profile
    {
        public ListFieldDocumentMapperProfile()
        {
            CreateMap<byte[], BitmapSource>().ConvertUsing<BytesToBitmapSourceTypeConverter>();
            CreateMap<BitmapSource, byte[]>().ConvertUsing<BitmapSourceToBytesTypeConverter>();

            CreateMap<ListFieldStepModel, ListFieldStepDto>()
                .ReverseMap()
                .ConstructUsingServiceLocator();

            CreateMap<ListFieldDocumentModel, ListFieldDocumentDto>()
                .IncludeBase<FieldDocumentModelBase, FieldDocumentDtoBase>()
                .ReverseMap()
                .ConstructUsingServiceLocator();
        }
    }
}
