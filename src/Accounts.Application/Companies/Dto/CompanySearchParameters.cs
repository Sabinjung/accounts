using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Companies.Dto
{
    public class CompanySearchParameters : NamedQueryParameter
    {
        public string SearchText { get; set; }

    }
}
