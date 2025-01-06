using RestSharp;
using System.Threading.Tasks;

namespace Sync
{
    /// <summary>
    /// API Docs found at https://docs.virtuoussoftware.com/
    /// </summary>
    internal class VirtuousService
    {
        private readonly RestClient _restClient;

        public VirtuousService(IConfiguration configuration) 
        {
            var apiBaseUrl = configuration.GetValue("VirtuousApiBaseUrl");
            var apiKey = configuration.GetValue("VirtuousApiKey");

            var options = new RestClientOptions(apiBaseUrl)
            {
                Authenticator = new RestSharp.Authenticators.OAuth2.OAuth2AuthorizationRequestHeaderAuthenticator(apiKey)
            };

            _restClient = new RestClient(options);
        }

        public async Task<PagedResult<AbbreviatedContact>> GetContactsAsync(int skip, int take)
        {
            var request = new RestRequest("/api/Contact/Query", Method.Post);
            request.AddQueryParameter("Skip", skip);
            request.AddQueryParameter("Take", take);

            var body = new ContactQueryRequest();
            request.AddJsonBody(body);

            var response = await _restClient.GetAsync<PagedResult<AbbreviatedContact>>(request);
            return response;
        }
    }
}
