using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Flurl;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenXmlPowerTools;
using Stutton.DocumentCreator.Models;
using Stutton.DocumentCreator.Models.WorkItems;
using Stutton.DocumentCreator.Services.Settings;
using Stutton.DocumentCreator.Services.Vsts.Dtos;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Vsts
{
    public class VstsAdalService : IVstsService
    {
        // ===== - Authentication Settings - ===== \\
        // This should be refactored into an IHttpProvider/Factory service that will return a configured HttpClient to use
        private const string _adalV3CacheFileName = "cacheAdalV3.bin";
        private const string _msalCacheFileName = "cacheMsal.bin";
        private FilesBasedTokenCache _tokenCache;
        private AuthenticationContext _context;
        private AuthenticationResult _authenticationResult;

        private const string _clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";
        private const string _replyUri = "urn:ietf:wg:oauth:2.0:oob";
        private const string _azureDevOpsResourceId = "499b84ac-1321-427f-aa17-267ca6975798";
        private readonly IPlatformParameters _defaultPromptBehavior = new PlatformParameters(PromptBehavior.Auto);
        private readonly ISettingsService _settingsService;
        private readonly IMapper _mapper;
        private HttpClient _client;

        public VstsAdalService(ISettingsService settingsService, IMapper mapper)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IResponse> AttachFileToWorkItemAsync(string filePath, int workItemId)
        {
            var fileName = Path.GetFileName(filePath);
            var fileBytes = File.ReadAllBytes(filePath);
            
            var uploadResponse = await SendRequestAsync(
                HttpMethod.Post,
                $"_apis/wit/attachments?fileName={fileName}&api-version=5.0",
                fileBytes);

            if (!uploadResponse.Success)
            {
                return Response.FromFailure(uploadResponse.Message);
            }

            var uploadResult = (JObject)JsonConvert.DeserializeObject(uploadResponse.Value);
            var uploadUrl = uploadResult.SelectToken("url")?.ToString();

            if (uploadUrl == null)
            {
                return Response.FromFailure("File upload did not return a file location url");
            }

            var patchDocument = new Object[]
            {
                new
                {
                    op = "test",
                    path = "/rev",
                    value = 3
                },
                new
                {
                    op = "add",
                    path = "/fields/System.History",
                    value = $"Attaching file {fileName}"
                },
                new
                {
                    op = "add",
                    path = "/relations/-",
                    value = new
                    {
                        rel = "AttachedFile",
                        url = uploadUrl,
                        attributes = new { comment = fileName }
                    }
                }
            };

            var body = JsonConvert.SerializeObject(patchDocument);

            var linkResponse = await SendRequestAsync(
                new HttpMethod("PATCH"),
                $"_apis/wit/workitems/{workItemId}?api-version=5.0",
                body);

            if (!linkResponse.Success)
            {
                return Response.FromFailure(linkResponse.Message);
            }

            return Response.FromSuccess();
        }

        public async Task<IResponse<ProfileModel>> GetUserProfileAsync()
        {
            var authResult = await AuthenticateAsync(_defaultPromptBehavior);
            if (!authResult.Success)
            {
                return Response<ProfileModel>.FromFailure(authResult.Message);
            }

            var model = new ProfileModel
            {
                Name = $"{authResult.Value.UserInfo.GivenName} {authResult.Value.UserInfo.FamilyName}"
            };
            return Response<ProfileModel>.FromSuccess(model);
        }

        public async Task<IResponse<IWorkItem>> GetWorkItemAsync(int id)
        {
            var response = await SendRequestAsync(HttpMethod.Get, $"_apis/wit/workitems/{id}?$expand=relations&api-version=5.0");
            var dto = JsonConvert.DeserializeObject<WorkItemApiResultDto>(response.Value);
            var result = _mapper.Map<WorkItemModel>(dto);

            return Response<IWorkItem>.FromSuccess(result);
        }

        public async Task<IResponse<IEnumerable<WorkItemFieldModel>>> GetWorkItemFields()
        {
            var response = await SendRequestAsync(HttpMethod.Get, $"_apis/wit/fields?api-version=5.0");
            if (!response.Success)
            {
                return Response<IEnumerable<WorkItemFieldModel>>.FromFailure(response.Message);
            }

            var dto = JsonConvert.DeserializeObject<VstsCollectionDto<WorkItemFieldDto>>(response.Value);

            var models = dto.Value.Select(t => new WorkItemFieldModel { Name = t.Name, ReferenceName = t.ReferenceName });

            return Response<IEnumerable<WorkItemFieldModel>>.FromSuccess(models);
        }

        public async Task<IResponse<string>> GetWorkItemFieldValue(int id, string field)
        {
            if(field.Equals("System.ID", StringComparison.InvariantCultureIgnoreCase))
            {
                return Response<string>.FromSuccess(id.ToString());
            }

            var response = await SendRequestAsync(HttpMethod.Get, $"_apis/wit/workitems/{id}?fields={field}&api-version=5.0");
            var dto = (JObject)JsonConvert.DeserializeObject(response.Value);

            var result = dto.SelectToken($"fields.['{field}']")?.ToString();

            if(result == null)
            {
                return Response<string>.FromFailure($"Field '{field}' not found on work item '{id}'");
            }

            return Response<string>.FromSuccess(result);
        }

        public async Task<IResponse<IEnumerable<IWorkItem>>> GetWorkItemsAsync(WorkItemQueryModel query)
        {
            try
            {
                var settingsResponse = await _settingsService.GetSettings();

                if (!settingsResponse.Success)
                {
                    return Response<IEnumerable<IWorkItem>>.FromFailure(settingsResponse.Message);
                }

                var settings = settingsResponse.Value;

                var sb = new StringBuilder();
                sb.Append($"SELECT [System.Id] FROM workitems WHERE [System.AssignedTo] contains '{settings.TfsUserName}'");
                foreach (var expression in query.Expressions.Where(p => p.Field != null))
                {
                    sb.Append(" AND ");

                    sb.Append($"[{expression.Field}] ");

                    sb.Append($"{VstsHelpers.GetExpressionOperatorString(expression.Operator)} ");

                    if (expression.Operator == WorkItemQueryExpressionOperator.In)
                    {
                        var valueSb = new StringBuilder();
                        valueSb.Append("(");

                        foreach (var value in expression.Values)
                        {
                            valueSb.Append($"'{value.Value}', ");
                        }

                        sb.Append($"{valueSb.ToString().TrimEnd(',', ' ')})");
                    }
                    else
                    {
                        sb.Append($"'{expression.Value}'");
                    }
                }

                var wiql = sb.ToString();
                var queryBody = JsonConvert.SerializeObject(new { query = wiql });

                var queryResponse = await SendRequestAsync(HttpMethod.Post, "_apis/wit/wiql?api-version=5.0", queryBody);
                if (!queryResponse.Success)
                {
                    return Response<IEnumerable<IWorkItem>>.FromFailure(queryResponse.Message);
                }
                var dto = JsonConvert.DeserializeObject<WiqlQueryResultDto>(queryResponse.Value);

                if(dto.WorkItems.Count == 0)
                {
                    return Response<IEnumerable<IWorkItem>>.FromSuccess(new List<IWorkItem>());
                }

                if(dto.WorkItems.Count > 200)
                {
                    dto.WorkItems = dto.WorkItems.Take(200).ToList();
                }

                var batchRequest = JsonConvert.SerializeObject(new
                {
                    expand = "Relations",
                    ids = dto.WorkItems.Select(wi => wi.Id)
                });
                batchRequest = batchRequest.Replace("expand", "$expand");

                var batchResponse = await SendRequestAsync(HttpMethod.Post, "_apis/wit/workitemsbatch?api-version=5.0", batchRequest);
                if (!batchResponse.Success)
                {
                    return Response<IEnumerable<IWorkItem>>.FromFailure(batchResponse.Message);
                }

                var batchResponseDto = JsonConvert.DeserializeObject<VstsCollectionDto<WorkItemApiResultDto>>(batchResponse.Value);

                var result = new List<IWorkItem>();
                foreach(var workItem in batchResponseDto.Value)
                {
                    result.Add(_mapper.Map<WorkItemModel>(workItem));
                }

                return Response<IEnumerable<IWorkItem>>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<IEnumerable<IWorkItem>>.FromException($"An error occurred getting the requested work items", ex);
            }
        }

        public Task OpenWorkItemInBrowser(IWorkItem workItem) => throw new NotImplementedException();

        public Task<IResponse> UpdateWorkItemAsync(int id, string fieldToUpdate, string newValue) => throw new NotImplementedException();

        public Task<IResponse> UpdateWorkItemAsync(int[] id, string fieldToUpdate, string newValue) => throw new NotImplementedException();

        private async Task<IResponse<AuthenticationResult>> AuthenticateAsync(IPlatformParameters promptBehavior)
        {
            try
            {
                if (_authenticationResult == null || _authenticationResult.ExpiresOn >= DateTime.Now)
                {
                    if (_tokenCache == null)
                    {
                        var cacheFolder =
                            Path.GetFullPath(
                                Path.GetDirectoryName(
                                    Assembly.GetEntryAssembly().Location));
                        _tokenCache = new FilesBasedTokenCache(
                            Path.Combine(cacheFolder, _adalV3CacheFileName),
                            Path.Combine(cacheFolder, _msalCacheFileName));
                    }

                    if (_context == null)
                    {
                        _context = new AuthenticationContext("https://login.windows.net/common", _tokenCache);
                        if (_context.TokenCache.Count > 0)
                        {
                            var homeTenant = _context.TokenCache.ReadItems().First().TenantId;
                            _context = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant, _tokenCache);
                        }
                    }

                    AuthenticationResult result = null;
                    try
                    {
                        result = await _context.AcquireTokenSilentAsync(_azureDevOpsResourceId, _clientId);
                    }
                    catch (AdalException adalException)
                    {
                        if (adalException.ErrorCode == AdalError.FailedToAcquireTokenSilently ||
                            adalException.ErrorCode == AdalError.InteractionRequired)
                        {
                            result = await _context.AcquireTokenAsync(_azureDevOpsResourceId, _clientId, new Uri(_replyUri), promptBehavior);
                        }
                    }

                    if (result == null || string.IsNullOrEmpty(result.AccessToken))
                    {
                        return Response<AuthenticationResult>.FromFailure("Unable to acquire access token");
                    }
                    _authenticationResult = result;
                }
                return Response<AuthenticationResult>.FromSuccess(_authenticationResult);
            }
            catch(Exception ex)
            {
                return Response<AuthenticationResult>.FromException("There was an error acquiring an access token", ex);
            }
        }

        private async Task<IResponse<HttpClient>> GetHttpClient(AuthenticationHeaderValue authHeader, string accept = null)
        {
            try
            {
                var settingsResponse = await _settingsService.GetSettings();
                if (!settingsResponse.Success)
                {
                    return Response<HttpClient>.FromFailure(settingsResponse.Message);
                }

                var baseUrl = settingsResponse.Value.TfsUrl;
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    return Response<HttpClient>.FromFailure("No TFS url found in settings");
                }

                if(baseUrl.Last() != '/')
                {
                    baseUrl += '/';
                }

                if(_client == null)
                {
                    _client = new HttpClient
                    {
                        BaseAddress = new Uri(baseUrl)
                    };
                }

                _client.DefaultRequestHeaders.Accept.Clear();
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(accept ?? "application/json"));
                _client.DefaultRequestHeaders.Add("User-Agent", "ManagedClientConsoleAppSample");
                _client.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
                _client.DefaultRequestHeaders.Authorization = authHeader;

                return Response<HttpClient>.FromSuccess(_client);
            }
            catch (Exception ex)
            {
                return Response<HttpClient>.FromException("There was an error creating the http client", ex);
            }
        }

        private async Task<IResponse<string>> SendRequestAsync(HttpMethod method, string url, string body = null)
        {
            try
            {
                var authResponse = await AuthenticateAsync(_defaultPromptBehavior);
                if (!authResponse.Success)
                {
                    return Response<string>.FromFailure(authResponse.Message);
                }

                var authToken = authResponse.Value.AccessToken;
                var clientResponse = await GetHttpClient(new AuthenticationHeaderValue("Bearer", authToken));
                if (!clientResponse.Success)
                {
                    return Response<string>.FromFailure(clientResponse.Message);
                }

                var client = clientResponse.Value;

                var request = new HttpRequestMessage(method, url);
                if (!string.IsNullOrEmpty(body))
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return Response<string>.FromSuccess(result);
            }
            catch(Exception ex)
            {
                return Response<string>.FromException("There was an error sending the request", ex);
            }
        }

        private async Task<IResponse<string>> SendRequestAsync(HttpMethod method, string url, byte[] body)
        {
            try
            {
                var authResponse = await AuthenticateAsync(_defaultPromptBehavior);
                if (!authResponse.Success)
                {
                    return Response<string>.FromFailure(authResponse.Message);
                }

                var authToken = authResponse.Value.AccessToken;
                var clientResponse = await GetHttpClient(
                    new AuthenticationHeaderValue("Bearer", authToken),
                    "application/octet-stream");
                if (!clientResponse.Success)
                {
                    return Response<string>.FromFailure(clientResponse.Message);
                }

                var client = clientResponse.Value;

                var request = new HttpRequestMessage(method, url)
                {
                    Content = new ByteArrayContent(body)
                };
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();
                return Response<string>.FromSuccess(result);
            }
            catch (Exception ex)
            {
                return Response<string>.FromException("There was an error sending the request", ex);
            }
        }
    }
}
