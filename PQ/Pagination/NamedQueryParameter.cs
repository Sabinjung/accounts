using System;
using System.Collections.Generic;
using System.Text;

namespace PQ.Pagination
{
    public class NamedQueryParameter
    {
        public string Name { get; set; }
        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }
        public bool IsActive { get;  set; }
    }
}
