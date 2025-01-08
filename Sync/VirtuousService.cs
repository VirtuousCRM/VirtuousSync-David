using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using RestSharp;

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

        public async Task<PagedResult<AbbreviatedContact>> GetContactsAsync(int skip, int take, string state)
        {
            var request = new RestRequest("/api/Contact/Query", Method.Post);
            request.AddQueryParameter("Skip", skip);
            request.AddQueryParameter("Take", take);
           
            var body = new ContactQueryRequest();
            request.AddJsonBody(body);

            var response = await _restClient.PostAsync<PagedResult<AbbreviatedContact>>(request);
            response.List = FilterForState(response, state);
            return response;
        }

        public List<AbbreviatedContact> FilterForState(PagedResult<AbbreviatedContact> contacts, string state)
        {
            if (state == null || state.Length != 2)
            {
                return contacts.List;
            }
            
            // Remove blank addresses 
            contacts.List = contacts.List.Where(x => !string.IsNullOrWhiteSpace(x.Address)).ToList();

            foreach (var item in contacts.List)
            {
                var stringList = item.Address.Split(',');
                if (stringList.Length <= 1) continue;

                item.State = stringList[1].Trim().Substring(0, 2);
            }

            return contacts.List.Where(x => x.State == state).ToList();
        }


        public  void AddContactsToDatabase(List<AbbreviatedContact> contacts)
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = "(localdb)\\MSSQLLocalDB",
                UserID = "Virtuous_user",
                Password = "Jlfdaso23891dsa",
                InitialCatalog = "Virtuous"
            };
            

            var connectionString = builder.ConnectionString;

            var cmdText = @"
            insert into dbo.Contacts (Id, Name, ContactType, ContactName, Address, Email, Phone)
            values (@Id, @Name, @ContactType, @ContactName, @Address, @Email, @Phone)";

            using var connection = new SqlConnection(connectionString);

            foreach (var item in contacts)
            {
                // Logic for Apostrophes 
                if (item.Name.Contains("'") || item.ContactName.Contains("'"))
                {
                    item.Name = item.Name.Replace("'", "''");
                    item.ContactName = item.ContactName.Replace("'", "''");

                }
                
                var command = new SqlCommand(cmdText, connection);
                command.Parameters.AddWithValue("@Id", item.Id);
                command.Parameters.AddWithValue("@Name", item.Name);
                command.Parameters.AddWithValue("@ContactType", item.ContactType);
                command.Parameters.AddWithValue("@ContactName", item.ContactName);
                command.Parameters.AddWithValue("@Address", item.Address);
                command.Parameters.AddWithValue("@Email", item.Email);
                command.Parameters.AddWithValue("@Phone", item.Phone);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

            

        }
    }
}
