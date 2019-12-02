using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Core.Invoicing
{
    public interface IInvoicingService
    {
        Task<Invoice> GenerateInvoice(int timesheetId, int userId);
        Task<string> Submit(int invoiceId, int userId);
        Task ReadInvoice(int invoiceId);
    }
}
