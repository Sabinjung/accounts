﻿using Abp.Application.Services;
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

        Task UpdateInvoice(UpdateInvoiceInputDto input);

        Task GenerateAndMailInvoice(int timesheetId);
        Task<InvoiceListItemDto> Search(InvoiceQueryParameter queryParameter);
        
        Task<IEnumerable<InvoiceMonthReportDto>> GetInvoicesByMonthReport(InvoiceQueryParameter queryParameter);

        Task<IEnumerable<AgeingReportDto>> GetAgeingReport(int? companyId);
    }
}