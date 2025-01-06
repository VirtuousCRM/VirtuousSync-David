using CsvHelper;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

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
            var apiKey = "REPLACE_WITH_API_KEY_PROVIDED";
            var configuration = new Configuration(apiKey);
            var virtuousService = new VirtuousService(configuration);

            var skip = 0;
            var take = 100;
            var maxContacts = 1000;
            var hasMore = true;

            using (var writer = new StreamWriter($"Contacts_{DateTime.Now:MM_dd_yyyy}.csv"))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                do
                {
                    var contacts = await virtuousService.GetContactsAsync(skip, take);
                    skip += take;
                    csv.WriteRecords(contacts.List);
                    hasMore = skip > maxContacts;
                }
                while (!hasMore);
            }
        }
    }
}
