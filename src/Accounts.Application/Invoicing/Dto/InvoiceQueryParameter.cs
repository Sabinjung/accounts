using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    public class InvoiceQueryParameter : NamedQueryParameter
    {
        public int? CompanyId { get; set; }
        public int? ConsultantId { get; set; }

        public string ConsultantName { get; set; }
        public string CompanyName { get; set; }

        public DateTime? IssueDate { get; set; }
    }
}