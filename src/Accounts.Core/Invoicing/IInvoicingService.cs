using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Core.Invoicing
{
    public interface IInvoicingService
    {
        Task<Invoice> GenerateInvoice(int timesheetId, int userId, bool shouldAssociate = false);
        Task Submit(int invoiceId, int userId, bool isMailing);
        Task SendMail(int invoiceId, bool isMailing);
        Task<string> Save(int timesheetId, int userId, string referenceNo);
        Task ReadInvoice(int invoiceId);
    }
}
