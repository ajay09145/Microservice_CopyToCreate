using Mango.Web.Models;
using Mango.Web.Services.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;

namespace Mango.Web.Services.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearer = true)
        {
            try
            {
                HttpClient httpClient = _httpClientFactory.CreateClient("MangoAPI");
                HttpRequestMessage httpRequestMessage = new();
                httpRequestMessage.Headers.Add("Accept", "application/json");

                //Token here
                if(withBearer)
                {
                    var token = _tokenProvider.GetToken();
                    httpRequestMessage.Headers.Add("Authorization", $"Bearer {token}");
                    
                }

                httpRequestMessage.RequestUri = new Uri(requestDto.Url);
                if (requestDto.Data != null)
                {
                    httpRequestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }
                HttpResponseMessage? apiResponse = null;

                switch (requestDto.APIType)
                {
                    case Utility.SD.APITypes.POST:
                        httpRequestMessage.Method = HttpMethod.Post;
                        break;
                    case Utility.SD.APITypes.PUT:
                        httpRequestMessage.Method = HttpMethod.Put;
                        break;
                    case Utility.SD.APITypes.DELETE:
                        httpRequestMessage.Method = HttpMethod.Delete;
                        break;
                    default:
                        httpRequestMessage.Method = HttpMethod.Get;
                        break;
                }

                apiResponse = await httpClient.SendAsync(httpRequestMessage);

                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "not found" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internale Error 500" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.BadRequest:
                        return new() { IsSuccess = false, Message = "Bad Request" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto()
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
