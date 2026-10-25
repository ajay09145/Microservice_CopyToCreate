using static Mango.Web.Utility.SD;

namespace Mango.Web.Models
{
    public class RequestDto
    {
        public APITypes APIType { get; set; } = APITypes.GET;
        public string? Url { get; set; }
        //public string? Data { get; set; }
        public object? Data { get; set; }
        public string? AccessToken { get; set; }
    }
}
