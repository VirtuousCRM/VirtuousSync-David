using System;
using System.Collections.Generic;
using System.Linq;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

            var response = await _restClient.PostAsync<PagedResult<AbbreviatedContact>>(request);
            response.List = FilterForState(response, "AZ");
            return response;
        }

        public List<AbbreviatedContact> FilterForState(PagedResult<AbbreviatedContact> contacts, string state)
        {
            contacts.List = contacts.List.Where(x => !string.IsNullOrWhiteSpace(x.Address)).ToList();

            foreach (var item in contacts.List)
            {
                var stringList = item.Address.Split(',');
                if (stringList.Length <= 1) continue;

                var stateArray = stringList[1].ToCharArray().Skip(1).Take(2).ToArray();
                item.State = new string(stateArray);
            }

            if (state != null )
            {
                contacts.List = contacts.List.Where(x => x.State == state).ToList();

            }
           

            return contacts.List;
        }
    }
}
