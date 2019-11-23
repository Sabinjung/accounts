using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Accounts.Core;
using Accounts.Core.Invoicing;
using Accounts.Invoicing.Dto;
using Accounts.Models;
using Intuit.Ipp.Core;
using Intuit.Ipp.OAuth2PlatformClient;
using Intuit.Ipp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Invoicing
{
    [AbpAuthorize]
    public class InvoiceAppService : AsyncCrudAppService<Invoice, InvoiceDto>, IInvoiceAppService
    {
        private readonly IInvoicingService InvoicingService;
        private readonly IObjectMapper Mapper;
        private readonly OAuth2Client OAuth2Client;

        public InvoiceAppService(IRepository<Invoice> repository, IInvoicingService invoicingService, IObjectMapper mapper,
            OAuth2Client oAuth2Client)
            : base(repository)
        {
            InvoicingService = invoicingService;
            Mapper = mapper;
            OAuth2Client = oAuth2Client;
        }

        [AbpAuthorize("Timesheet.GenerateInvoice")]
        public async Task<InvoiceDto> GenerateInvoice(int timesheetId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            var invoice = await InvoicingService.GenerateInvoice(timesheetId, currentUserId);
            return Mapper.Map<InvoiceDto>(invoice);
        }

        [AbpAuthorize("Invoicing.Submit")]
        public async Task Submit(int invoiceId)
        {
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var currentUserId = Convert.ToInt32(AbpSession.UserId);
                await InvoicingService.Submit(invoiceId, currentUserId);
            }
        }
    }
}
