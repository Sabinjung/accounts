using Abp.Application.Services;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Extensions;
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
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
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
        private readonly QueryBuilderFactory QueryBuilder;

        public InvoiceAppService(IRepository<Invoice> repository, IInvoicingService invoicingService, IObjectMapper mapper,
            OAuth2Client oAuth2Client,
             IntuitDataProvider intuitDataProvider, QueryBuilderFactory queryBuilderFactory
           )
            : base(repository)
        {
            InvoicingService = invoicingService;
            Mapper = mapper;
            OAuth2Client = oAuth2Client;
            IntuitDataProvider = intuitDataProvider;

            QueryBuilder = queryBuilderFactory;
            CreatePermissionName = "Invoice.Create";
            UpdatePermissionName = "Invoice.Update";
            DeletePermissionName = "Invoice.Delete";
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
        public async Task GenerateAndSubmit(int timesheetId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                await InvoicingService.Submit(timesheetId, currentUserId);
            }
        }

        [AbpAuthorize("Invoicing.Submit")]
        public async Task GenerateAndSave(int timesheetId, string qbInvoiceId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            await InvoicingService.Save(timesheetId, currentUserId, qbInvoiceId);
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

        [HttpGet]
        public async Task<Page<InvoiceQueryDto>> Search(InvoiceQueryParameter queryParameter)
        {
            var query2 = await Repository.GetAllListAsync();

            var query = QueryBuilder.Create<Invoice, InvoiceQueryParameter>(Repository.GetAll());
            query.WhereIf(x => !x.ConsultantName.IsNullOrWhiteSpace(), c => p => p.Consultant.FirstName.ToUpper().Contains(c.ConsultantName.ToUpper()));
            query.WhereIf(x => !x.CompanyName.IsNullOrWhiteSpace(), c => p => p.Company.DisplayName.ToUpper().Contains(c.CompanyName.ToUpper()));
            query.WhereIf(x => x.ConsultantId.HasValue, c => p => p.ConsultantId == c.ConsultantId);
            query.WhereIf(x => x.CompanyId.HasValue, c => p => p.CompanyId == c.CompanyId);
            query.WhereIf(x => !x.IssueDate.ToString().IsNullOrWhiteSpace(), c => p => p.InvoiceDate.Date.ToString().Contains(c.IssueDate.ToString()));
            var sorts = new Sorts<Invoice>();
            sorts.Add(true, x => x.Consultant.FirstName);
            query.ApplySorts(sorts);
            var results = await query.ExecuteAsync<InvoiceQueryDto>(queryParameter);
            return results;
        }
    }
}