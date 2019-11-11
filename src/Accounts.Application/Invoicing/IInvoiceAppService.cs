using Abp.Application.Services;
using Accounts.Invoicing.Dto;
using System.Threading.Tasks;

namespace Accounts.Invoicing
{
    public interface IInvoiceAppService : IAsyncCrudAppService<InvoiceDto>
    {
        Task<InvoiceDto> GenerateInvoice(GenerateInvoiceInputDto input);

        Task Submit(int invoiceId);
    }
}