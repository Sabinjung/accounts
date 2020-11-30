using Accounts.Intuit;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Core.Invoicing
{
    public interface IInvoiceProcessor
    {
        Task<IntuitInvoiceDto> Send(Invoice invoice, bool isMailing);
    }
}
