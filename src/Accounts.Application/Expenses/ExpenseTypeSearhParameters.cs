using PQ.Pagination;

namespace Accounts.Expenses
{
    public class ExpenseTypeSearchParameters : NamedQueryParameter
    {
        public string SearchText { get; set; }

    }
}