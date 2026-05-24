using System.Net.Http.Headers;
using System.Net;

namespace PMS.Web.Security
{
    public class ApiAuthHeaderHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiAuthHeaderHandler(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null)
            {
                return await base.SendAsync(request, cancellationToken);
            }

            var token = CookieReader.GetToken(context);
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                CookieReader.ClearAuthCookies(context);
            }

            return response;
        }
    }
}
