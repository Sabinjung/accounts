using Abp.Application.Services;
using Accounts.Invoicing.Dto;
using System.Threading.Tasks;

namespace Accounts.Invoicing
{
    public interface IInvoiceAppService : IAsyncCrudAppService<InvoiceDto>
    {
        Task<InvoiceDto> GenerateInvoice(int timesheetId);

        Task Submit(int invoiceId);
    }
}