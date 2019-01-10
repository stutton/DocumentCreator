using AutoMapper;
using System.Windows.Media.Imaging;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public class ListFieldDocumentMapperProfile : Profile
    {
        public ListFieldDocumentMapperProfile()
        {
            CreateMap<byte[], BitmapSource>().ConvertUsing<BytesToBitmapSourceTypeConverter>();
            CreateMap<BitmapSource, byte[]>().ConvertUsing<BitmapSourceToBytesTypeConverter>();

            CreateMap<ListFieldStepModel, ListFieldStepDto>()
                .ConstructUsingServiceLocator();

            CreateMap<ListFieldStepDto, ListFieldStepModel>()
                .ConstructUsingServiceLocator();

            CreateMap<ListFieldDocumentModel, ListFieldDocumentDto>()
                .IncludeBase<FieldDocumentModelBase, FieldDocumentDtoBase>()
                .ConstructUsingServiceLocator();

            CreateMap<ListFieldDocumentDto, ListFieldDocumentModel>()
                .ForMember(p => p.IsExpanded, opt => opt.Ignore())
                .IncludeBase<FieldDocumentDtoBase, FieldDocumentModelBase>()
                .ConstructUsingServiceLocator();
        }
    }
}