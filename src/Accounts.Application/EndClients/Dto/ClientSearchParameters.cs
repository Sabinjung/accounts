using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.EndClients.Dto
{
    public class ClientSearchParameters : NamedQueryParameter
    {
        public string SearchText { get; set; }
    }
}
