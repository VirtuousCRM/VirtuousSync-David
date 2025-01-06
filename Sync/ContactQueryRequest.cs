using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    public class ContactQueryRequest
    {
        public ContactQueryRequest()
        {
            Groups = new List<object>();
            SortBy = "Id";
            Descending = false;
        }

        public List<object> Groups { get; set; }

        public string SortBy { get; set; }

        public bool Descending { get; set; }
    }
}
