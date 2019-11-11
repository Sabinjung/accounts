using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Accounts.Core;
using Accounts.Core.Invoicing;
using Accounts.Invoicing.Dto;
using Accounts.Models;
using Intuit.Ipp.Core;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Invoicing
{
    public class InvoiceAppService : AsyncCrudAppService<Invoice, InvoiceDto>, IInvoiceAppService
    {
        private readonly IInvoicingService InvoicingService;
        private readonly IObjectMapper Mapper;

        public InvoiceAppService(IRepository<Invoice> repository, IInvoicingService invoicingService, IObjectMapper mapper)
            : base(repository)
        {
            InvoicingService = invoicingService;
            Mapper = mapper;
        }


        //[AbpAuthorize("Invoicing.GenerateInvoice")]
        public async Task<InvoiceDto> GenerateInvoice(GenerateInvoiceInputDto input)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            var invoice = await InvoicingService.GenerateInvoice(1, currentUserId);
            return Mapper.Map<InvoiceDto>(invoice);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Intuit")]
        //[AbpAuthorize("Invoicing.Submit")]
        public async Task Submit(int invoiceId)
        {
            await InvoicingService.Submit(invoiceId);
        }
    }
}
