using Abp.Application.Services;
using Accounts.Invoicing.Dto;
using PQ.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounts.Invoicing
{
    public interface IInvoiceAppService : IAsyncCrudAppService<InvoiceDto>
    {
        Task<InvoiceDto> GenerateInvoice(int timesheetId);

        Task Submit(int invoiceId);

        Task<Page<IncoiceListItemDto>> Search(InvoiceQueryParameter queryParameter);
        Task<IEnumerable<InvoiceMonthReportDto>> GetInvoicesByMonthReport(InvoiceQueryParameter queryParameter);
    }
}