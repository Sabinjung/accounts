using PQ.Pagination;

namespace Accounts.Expenses.Dto
{
    public class ExpenseTypeSearchParameters : NamedQueryParameter
    {
        public string SearchText { get; set; }
    }
}