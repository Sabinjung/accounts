using PQ.Pagination;

namespace Accounts.Projects
{
    public class ProjectQueryParameters : NamedQueryParameter
    {
        public bool? IsProjectActive { get; set; }

        public string Keyword { get; set; }

        public int? InvoiceCyclesId { get; set; }

        public int? TermId { get; set; }
    }
}