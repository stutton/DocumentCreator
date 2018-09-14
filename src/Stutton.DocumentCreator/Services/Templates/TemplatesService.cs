using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using Stutton.DocumentCreator.Models.Template;
using Stutton.DocumentCreator.Shared;
using System.IO.Compression;

namespace Stutton.DocumentCreator.Services.Templates
{
    public class TemplatesService : ITemplatesService
    {
        private static readonly string DocumentTemplatesDirectoryName =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DocumentCreator\\Templates";
        private static readonly string DocumentTemplateFileExtension = "template";
        private static readonly string TemplateDocsDirectoryName = DocumentTemplatesDirectoryName + "\\Docs";
        private static readonly string FirstRunTemplatesDirectory = DocumentTemplatesDirectoryName + "\\FirstRun";

        private readonly IMapper _mapper;

        public TemplatesService(IMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IResponse<IEnumerable<DocumentTemplateModel>>> GetDocuments()
        {
            try
            {
                if (!Directory.Exists(DocumentTemplatesDirectoryName))
                {
                    return Response<IEnumerable<DocumentTemplateModel>>.FromSuccess(new List<DocumentTemplateModel>());
                }

                var templateFiles = await Task.Run(() =>
                    Directory.GetFiles(DocumentTemplatesDirectoryName, $"*.{DocumentTemplateFileExtension}"));

                if (!templateFiles.Any())
                {
                    return Response<IEnumerable<DocumentTemplateModel>>.FromSuccess(new List<DocumentTemplateModel>());
                }

                var templates = new List<DocumentTemplateModel>();
                foreach (var templateFile in templateFiles)
                {
                    var template = await OpenTemplate(templateFile);
                    await template.Initialize();
                    templates.Add(template);
                }

                return Response<IEnumerable<DocumentTemplateModel>>.FromSuccess(templates);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<DocumentTemplateModel>>.FromException("Failed to load document templates", ex);
            }
        }

        public async Task<IResponse> SaveDocumentTemplate(DocumentTemplateModel document)
        {
            try
            {
                await SaveTemplate(document);
                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to save template {document.TemplateDetails.Name}", ex);
            }
        }

        public async Task<IResponse> DeleteDocumentTemplate(DocumentTemplateModel document)
        {
            try
            {
                var filePath = $"{DocumentTemplatesDirectoryName}\\{document.Id}.{DocumentTemplateFileExtension}";
                if (File.Exists(filePath))
                {
                    await Task.Run(() => File.Delete(filePath));
                }

                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to delete template {document.TemplateDetails.Name}", ex);
            }
        }

        public async Task<IResponse> ShareDocumentTemplate(DocumentTemplateModel document, string fileName)
        {
            try
            {
                if (!fileName.EndsWith(".zip"))
                {
                    fileName += ".zip";
                }
                var rootFolder = Path.GetDirectoryName(fileName);
                if (rootFolder == null)
                {
                    return Response.FromFailure("Failed to determine directory to save file", ResponseCode.FileNotFound);
                }

                var zipDirectory = Path.Combine(rootFolder, "zip");

                var sourceTemplateFile = $"{DocumentTemplatesDirectoryName}\\{document.Id}.{DocumentTemplateFileExtension}";
                var destTemplateFile = $"{zipDirectory}\\{document.Id}.{DocumentTemplateFileExtension}";
                var sourceDocFile = document.TemplateDetails.TemplateFilePath;
                var destDocFile = $"{zipDirectory}\\{document.Id}.docx";

                await Task.Run(() =>
                {
                    try
                    {
                        Directory.CreateDirectory(zipDirectory);

                        File.Copy(sourceTemplateFile, destTemplateFile);
                        File.Copy(sourceDocFile, destDocFile);

                        ZipFile.CreateFromDirectory(zipDirectory, fileName);
                    }
                    finally
                    {
                        if (Directory.Exists(zipDirectory))
                        {
                            if (File.Exists(destTemplateFile))
                            {
                                File.Delete(destTemplateFile);
                            }

                            if (File.Exists(destDocFile))
                            {
                                File.Delete(destDocFile);
                            }
                            Directory.Delete(zipDirectory);
                        }
                    }
                });

                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to share template {document.TemplateDetails.Name}", ex);
            }
        }

        public async Task<IResponse> ImportDocumentTemplate(string fileName)
        {
            try
            {
                // Add .zip if needed
                if (!fileName.EndsWith(".zip"))
                {
                    fileName += ".zip";
                }

                // Verify file exists
                if (!File.Exists(fileName))
                {
                    return Response.FromFailure($"File '{fileName}' does not exist.", ResponseCode.FileNotFound);
                }

                string zipDirectory = null, templateFile = null, docFile = null;
                try
                {
                    // Unzip to temporary location
                    zipDirectory = $"{DocumentTemplatesDirectoryName}\\zip";
                    Directory.CreateDirectory($"{DocumentTemplatesDirectoryName}\\zip");
                    ZipFile.ExtractToDirectory(fileName, zipDirectory);

                    // Try to get the .template and .docx files from the extracted zip
                    templateFile = Directory.GetFiles(zipDirectory, $"*.{DocumentTemplateFileExtension}").FirstOrDefault();
                    docFile = Directory.GetFiles(zipDirectory, "*.docx").FirstOrDefault();

                    // Verify the files were found
                    if (templateFile == null || docFile == null)
                    {
                        return Response.FromFailure("Zip archive did not contain the correct files", ResponseCode.FileNotFound);
                    }

                    // Create the docs folder if needed
                    if (!Directory.Exists(TemplateDocsDirectoryName))
                    {
                        Directory.CreateDirectory(TemplateDocsDirectoryName);
                    }

                    // Copy the .docx to the docs folder
                    var docFileDest = Path.Combine(TemplateDocsDirectoryName, Path.GetFileName(docFile));
                    File.Copy(docFile, docFileDest);

                    // Load the template and update the TemplateFilePath
                    var template = await OpenTemplate(templateFile);
                    template.TemplateDetails.TemplateFilePath = docFileDest;

                    // Save the modified template
                    await SaveTemplate(template);
                }
                finally
                {
                    // Cleanup
                    if (!string.IsNullOrEmpty(zipDirectory) && File.Exists(zipDirectory))
                    {
                        if (!string.IsNullOrEmpty(templateFile) && File.Exists(templateFile))
                        {
                            File.Delete(templateFile);
                        }

                        if (!string.IsNullOrEmpty(docFile) && File.Exists(docFile))
                        {
                            File.Delete(docFile);
                        }

                        Directory.Delete(zipDirectory);
                    }
                }

                return Response.FromSuccess();
            }
            catch (Exception ex)
            {
                return Response.FromException($"Failed to import the template from '{fileName}'", ex);
            }
        }

        public Task<IResponse> ImportFirstRunTemplates()
        {
            return Task.Run<IResponse>(async () =>
            {
                // Check for first run templates
                if (!Directory.Exists(FirstRunTemplatesDirectory))
                {
                    return Response.FromSuccess();
                }

                var files = Directory.GetFiles(FirstRunTemplatesDirectory);
                if (!files.Any())
                {
                    return Response.FromSuccess();
                }

                foreach (var file in files)
                {
                    var response = await ImportDocumentTemplate(file);
                    if (!response.Success)
                    {
                        return response;
                    }
                    File.Delete(file);
                }

                Directory.Delete(FirstRunTemplatesDirectory);

                return Response.FromSuccess();
            });
        }

        private Task<DocumentTemplateModel> OpenTemplate(string templateFile) => Task.Run(() =>
        {
            var templateJson = File.ReadAllText(templateFile);
            var templateDto = JsonConvert.DeserializeObject<DocumentTemplateDto>(templateJson,
                                                                                 new JsonSerializerSettings
                                                                                 {
                                                                                     TypeNameHandling = TypeNameHandling.Objects
                                                                                 });
            var template = _mapper.Map<DocumentTemplateModel>(templateDto);
            return template;
        });

        private Task SaveTemplate(DocumentTemplateModel template) => Task.Run(() =>
        {
            if (!Directory.Exists(DocumentTemplatesDirectoryName))
            {
                Directory.CreateDirectory(DocumentTemplatesDirectoryName);
            }

            var templateDto = _mapper.Map<DocumentTemplateDto>(template);

            var documentJson = JsonConvert.SerializeObject(templateDto,
                                                           Formatting.Indented,
                                                           new JsonSerializerSettings
                                                           {
                                                               TypeNameHandling = TypeNameHandling.Objects,
                                                               TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                                                           });
            File.WriteAllText($"{DocumentTemplatesDirectoryName}\\{template.Id}.{DocumentTemplateFileExtension}", documentJson);
        });
    }
}
