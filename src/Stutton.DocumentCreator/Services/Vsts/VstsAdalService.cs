using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Flurl;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
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
        private const string _clientId = "872cd9fa-d31f-45e0-9eab-6e460a02d1f1";
        private const string _replyUri = "urn:ietf:wg:oauth:2.0:oob";
        private const string _azureDevOpsResourceId = "499b84ac-1321-427f-aa17-267ca6975798";
        private readonly IPlatformParameters _defaultPromptBehavior = new PlatformParameters(PromptBehavior.Auto);
        private readonly ISettingsService _settingsService;

        private AuthenticationContext _context;
        private AuthenticationResult _authenticationResult;
        private HttpClient _client;

        public VstsAdalService(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
        }

        public Task<IResponse> AttachFileToWorkItemAsync(string filePath, int workItemId) => throw new NotImplementedException();

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
            var response = SendRequestAsync(HttpMethod.Get, $"_apis/wit/workitems/{id}?api-version=5.0");

            throw new NotImplementedException();
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

        public Task<IResponse<string>> GetWorkItemFieldValue(int id, string field) => throw new NotImplementedException();

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
                    dto.WorkItems = dto.WorkItems.Take(1).ToList();
                }

                var batchRequest = JsonConvert.SerializeObject(new
                {
                    expand = "Relations",
                    ids = dto.WorkItems.Select(wi => wi.Id),
                    //fields = new[]
                    //{
                    //    "System.Id",
                    //    "System.Title",
                    //    "System.WorkItemType",
                    //    "System.Description",
                    //    "System.AreaPath",
                    //    "System.TeamProject",
                    //    "System.State"
                    //}
                });
                batchRequest = batchRequest.Replace("expand", "$expand");

                var batchResponse = await SendRequestAsync(HttpMethod.Post, "_apis/wit/workitemsbatch?api-version=5.0", batchRequest);
                if (!batchResponse.Success)
                {
                    return Response<IEnumerable<IWorkItem>>.FromFailure(batchResponse.Message);
                }



                throw new NotImplementedException();
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
                    if (_context == null)
                    {
                        _context = new AuthenticationContext("https://login.windows.net/common");
                        if (_context.TokenCache.Count > 0)
                        {
                            var homeTenant = _context.TokenCache.ReadItems().First().TenantId;
                            _context = new AuthenticationContext("https://login.microsoftonline.com/" + homeTenant);
                        }
                    }

                    var result = await _context.AcquireTokenAsync(_azureDevOpsResourceId, _clientId, new Uri(_replyUri), promptBehavior);
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

        private async Task<IResponse<HttpClient>> GetHttpClient(AuthenticationHeaderValue authHeader)
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
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
    }
}
