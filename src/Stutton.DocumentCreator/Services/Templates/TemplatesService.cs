using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Stutton.DocumentCreator.Fields;
using Stutton.DocumentCreator.Fields.List.Template;
using Stutton.DocumentCreator.Fields.Text.Template;
using Stutton.DocumentCreator.Fields.UserName.Template;
using Stutton.DocumentCreator.Fields.WorkItemField.Template;
using Stutton.DocumentCreator.Models.Documents;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Services.Telemetry;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Templates
{
    public class TemplatesService : ITemplatesService
    {
        private readonly IServiceResolver _serviceResolver;
        private readonly ITelemetryService _telemetryService;
        private readonly IMapper _mapper;

        private readonly string _documentTemplatesDirectoryName =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator\\Templates";
        private readonly string _documentTemplateFileExtension = "template";

        public TemplatesService(IServiceResolver serviceResolver, ITelemetryService telemetryService)
        {
            _serviceResolver = serviceResolver ?? throw new ArgumentNullException(nameof(serviceResolver));
            _telemetryService = telemetryService ?? throw new ArgumentNullException(nameof(telemetryService));

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ListFieldTemplateModel, ListFieldTemplateDto>();
                cfg.CreateMap<TextFieldTemplateModel, TextFieldTemplateDto>();
                cfg.CreateMap<UserNameFieldTemplateModel, UserNameFieldTemplateDto>();
                cfg.CreateMap<WorkItemFieldTemplateModel, WorkItemFieldTemplateDto>();
                
                cfg.CreateMap<WorkItemFieldTemplateDto, WorkItemFieldTemplateModel>();

                cfg.CreateMap<DocumentTemplateModel, DocumentTemplateDto>();
            });
            _mapper = new Mapper(mapperConfig, Resolver);
        }

        public async Task<IResponse<IEnumerable<DocumentTemplateModel>>> GetDocuments()
        {
            try
            {
                if (!Directory.Exists(_documentTemplatesDirectoryName))
                {
                    return Response<IEnumerable<DocumentTemplateModel>>.FromSuccess(new List<DocumentTemplateModel>());
                }

                var templateFiles = await Task.Run(() =>
                    Directory.GetFiles(_documentTemplatesDirectoryName, $"*.{_documentTemplateFileExtension}"));

                if (!templateFiles.Any())
                {
                    return Response<IEnumerable<DocumentTemplateModel>>.FromSuccess(new List<DocumentTemplateModel>());
                }

                var templates = new List<DocumentTemplateModel>();
                foreach (var templateFile in templateFiles)
                {
                    var templateJson = await Task.Run(() => File.ReadAllText(templateFile));
                    var templateDto = await Task.Run(() => JsonConvert.DeserializeObject<DocumentTemplateDto>(templateJson, new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.Objects
                    }));
                    var template = _mapper.Map<DocumentTemplateModel>(templateDto);
                    templates.Add(template);
                }

                return Response<IEnumerable<DocumentTemplateModel>>.FromSuccess(templates);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<DocumentTemplateModel>>.FromException("Failed to get saved documents", ex);
            }
        }

        public async Task<IResponse> SaveDocumentTemplate(DocumentTemplateModel document)
        {
            try
            {
                if (!Directory.Exists(_documentTemplatesDirectoryName))
                {
                    Directory.CreateDirectory(_documentTemplatesDirectoryName);
                }

                var templateDto = _mapper.Map<DocumentTemplateDto>(document);

                var documentJson = await Task.Run(() => JsonConvert.SerializeObject(templateDto, Formatting.Indented, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                }));
                await Task.Run(() => File.WriteAllText($"{_documentTemplatesDirectoryName}\\{document.Id}.{_documentTemplateFileExtension}", documentJson));
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save document {document.TemplateDetails.Name}", ex);
            }
        }

        private object Resolver(Type type)
        {
            var response = _serviceResolver.Resolve(type);
            if (!response.Success)
            {
                _telemetryService.TrackFailedResponse(response);
                return null;
            }

            return response.Value;
        }
    }
}
