using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
   public  class InvoiceAggregateQueryParameter:NamedQueryParameter
    {

        public int? ConsultantId { get; set; }
        public int? CompanyId { get; set; }
        

        public int? ProjectId { get; set; }
    }
}
