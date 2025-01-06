using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync
{
    public class PagedResult<T>
    {
        public int Total { get; set; }

        public List<T> List { get; set; }
    }
}
