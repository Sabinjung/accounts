using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Accounts.Core;
using Accounts.Core.Invoicing;
using Accounts.Intuit;
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
using IntuitData = Intuit.Ipp.Data;

namespace Accounts.Invoicing
{
    [AbpAuthorize]
    public class InvoiceAppService : AsyncCrudAppService<Invoice, InvoiceDto>, IInvoiceAppService
    {
        private readonly IInvoicingService InvoicingService;
        private readonly IObjectMapper Mapper;
        private readonly OAuth2Client OAuth2Client;
        private readonly IntuitDataProvider IntuitDataProvider;

        public InvoiceAppService(IRepository<Invoice> repository, IInvoicingService invoicingService, IObjectMapper mapper,
            OAuth2Client oAuth2Client,
            IntuitDataProvider intuitDataProvider)
            : base(repository)
        {
            InvoicingService = invoicingService;
            Mapper = mapper;
            OAuth2Client = oAuth2Client;
            IntuitDataProvider = intuitDataProvider;
        }

        [HttpGet]
        [AbpAuthorize("Timesheet.GenerateInvoice")]
        public async Task<InvoiceDto> GenerateInvoice(int timesheetId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            var invoice = await InvoicingService.GenerateInvoice(timesheetId, currentUserId);
            var dto = Mapper.Map<InvoiceDto>(invoice);
            dto.TimesheetId = timesheetId;
            return dto;
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

        [AbpAuthorize("Invoicing.Submit")]
        public async Task GenerateAndSubmit(int timesheetId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            //var invoice = await InvoicingService.GenerateInvoice(timesheetId, currentUserId, false);
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {

                await InvoicingService.Submit(timesheetId, currentUserId);
            }
        }

        public async Task ReadInvoice(string invoiceId)
        {
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var cus = new IntuitData.Invoice { Id = invoiceId };
                var invoice = IntuitDataProvider.FindById(cus);
            }
        }
    }

}