using CsvHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace Sync
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Sync().GetAwaiter().GetResult();
        }

        private static async Task Sync()
        {
            var apiKey = "v_eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiN2VhYTBhNTQtYTBiZC00OTNlLWFjNDMtZjNjZGEwZmVlNWQ5IiwiZXhwIjoyMTQ3NDgzNjQ3LCJpc3MiOiJodHRwczovL2FwcC52aXJ0dW91c3NvZnR3YXJlLmNvbSIsImF1ZCI6Imh0dHBzOi8vYXBpLnZpcnR1b3Vzc29mdHdhcmUuY29tIn0.oN0bfmYMS7lPxGtVH3ouEVhD0Kuzoqa2nAnuvPTyPpk";
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;
            var stateFilter = "AZ";

            using var writer = new StreamWriter($"Contacts_{DateTime.Now:MM_dd_yyyy}.csv");
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            do
            {
                var contacts = await virtuousService.GetContactsAsync(skip, take, stateFilter);

                skip += take;
                //await csv.WriteRecordsAsync(contacts.List);
                virtuousService.AddContactsToDatabase(contacts.List);
                hasMore = skip > maxContacts - take;
            }
            while (!hasMore);

            //try
            //{
               

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine($"Error occurred: {ex.Message}");
            //}


        }

       
    }
}
