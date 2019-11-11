using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Consultants.Dto
{
    public class ConsultantSearchParameters : NamedQueryParameter
    {
        public string SearchText { get; set; }

    }
}
