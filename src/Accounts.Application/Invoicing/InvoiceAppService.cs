using Abp.Application.Services;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Extensions;
using Abp.ObjectMapping;
using Accounts.Core.Invoicing;
using Accounts.Intuit;
using Accounts.Invoicing.Dto;
using Accounts.Models;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using PQ;
using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MoreLinq;
using System.Collections.Concurrent;
using Abp.UI;

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
        private readonly IRepository<Project> ProjectRepository;
        private readonly IRepository<Timesheet> TimesheetRepository;
        private readonly IRepository<HourLogEntry> HourLogRepository;

        public InvoiceAppService(IRepository<Invoice> repository, IInvoicingService invoicingService, IObjectMapper mapper,
            OAuth2Client oAuth2Client, IRepository<Project> projectRepository, IRepository<Timesheet> timesheetRepository,
             IRepository<HourLogEntry> hourLogRepository, IntuitDataProvider intuitDataProvider, QueryBuilderFactory queryBuilderFactory
           )
            : base(repository)
        {
            InvoicingService = invoicingService;
            Mapper = mapper;
            OAuth2Client = oAuth2Client;
            IntuitDataProvider = intuitDataProvider;
            ProjectRepository = projectRepository;
            TimesheetRepository = timesheetRepository;
            HourLogRepository = hourLogRepository;
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
            var projectId = TimesheetRepository.Get(timesheetId).ProjectId;
            var dto = Mapper.Map<InvoiceDto>(invoice);
            dto.TimesheetId = timesheetId;
            dto.IsSendMail = ProjectRepository.Get(projectId).IsSendMail;
            return dto;
        }

        [AbpAuthorize("Invoicing.Submit")]
        public async Task GenerateAndSubmit(int timesheetId)
        {
            var currentUserId = Convert.ToInt32(AbpSession.UserId);
            bool isMailing = false;
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                await InvoicingService.Submit(timesheetId, currentUserId, isMailing);
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
            bool isMailing = false;
            if (isConnectionEstablished)
            {
                var currentUserId = Convert.ToInt32(AbpSession.UserId);
                await InvoicingService.Submit(invoiceId, currentUserId, isMailing);
            }
        }
        
        [AbpAuthorize("Invoicing.SubmitAndMail")]
        public async Task GenerateAndMailInvoice(int timesheetId)
        {
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            bool isMailing = true;
            if (isConnectionEstablished)
            {
                var currentUserId = Convert.ToInt32(AbpSession.UserId);
                await InvoicingService.Submit(timesheetId, currentUserId, isMailing);
            }
        }
        [AbpAuthorize("Invoicing.Edit")]
        public async Task UpdateInvoice(UpdateInvoiceInputDto input)
        {
            var distinctHourLogs = input.UpdatedHourLogEntries.DistinctBy(x => new { x.Day, x.ProjectId }).OrderBy(x => x.Day);
            var hourLogEntries = await HourLogRepository.GetAllListAsync(x => distinctHourLogs.Any(y => y.ProjectId == x.ProjectId && x.Day == y.Day));
            var timesheet = TimesheetRepository.GetAll().Where(x => x.InvoiceId == input.Invoice.Id).FirstOrDefault();

            var invoice = await Repository.GetAsync(input.Invoice.Id);
            invoice.Rate = input.Invoice.Rate;
            invoice.TotalHours = input.Invoice.TotalHours;
            invoice.DiscountType = input.Invoice.DiscountType;
            invoice.DiscountValue = input.Invoice.DiscountValue;
            invoice.ServiceTotal = input.Invoice.ServiceTotal;
            invoice.DiscountAmount = input.Invoice.DiscountAmount;
            invoice.SubTotal = input.Invoice.SubTotal;
            invoice.Total = input.Invoice.Total;

            if(input.UpdatedHourLogEntries.Max(x => x.Day) < timesheet.EndDt)
            {
                timesheet.EndDt = input.UpdatedHourLogEntries.Max(x => x.Day);
                invoice.Description = "Billing Period " + input.UpdatedHourLogEntries.Min(x => x.Day).ToShortDateString() + "-" + input.UpdatedHourLogEntries.Max(x => x.Day).ToShortDateString();
            }
            var removedHourLogs = await HourLogRepository.GetAllListAsync(x => x.TimesheetId == timesheet.Id && hourLogEntries.Max(y => y.Day) < x.Day);
            foreach (var hour in removedHourLogs)
            {
                hour.TimesheetId = null;
            }
            Parallel.ForEach(distinctHourLogs, log =>
            {
                var existingHourLog = hourLogEntries.FirstOrDefault(x => x.ProjectId == log.ProjectId && x.Day == log.Day);
                if(existingHourLog != null && existingHourLog.Day != log.Day)
                {
                    existingHourLog.TimesheetId = null;
                }
                if (existingHourLog != null)
                {
                    if (log.Hours.HasValue)
                    {
                        existingHourLog.Hours = log.Hours;
                        HourLogRepository.Update(existingHourLog);
                    }
                }
                else
                {
                    throw new UserFriendlyException("Could not find existing hours to updated.");
                }
            });

            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var currentUserId = Convert.ToInt32(AbpSession.UserId);
                await InvoicingService.UpdateAndSendMail(invoice.Id, input.Invoice.IsSendMail);
            }

        }
        private QueryBuilder<Invoice, InvoiceQueryParameter> GetQuery(InvoiceQueryParameter queryParameter)
        {
            var query = QueryBuilder.Create<Invoice, InvoiceQueryParameter>(Repository.GetAll());
            query.WhereIf(x => !x.ConsultantName.IsNullOrWhiteSpace(), c => p => p.Consultant.FirstName.ToUpper().Contains(c.ConsultantName.ToUpper()));
            query.WhereIf(x => !x.CompanyName.IsNullOrWhiteSpace(), c => p => p.Company.DisplayName.ToUpper().Contains(c.CompanyName.ToUpper()));
            query.WhereIf(x => x.ConsultantId.HasValue, c => p => p.ConsultantId == c.ConsultantId);
            query.WhereIf(x => x.ProjectId.HasValue, c => p => p.ProjectId == c.ProjectId);
            query.WhereIf(x => x.CompanyId.HasValue, c => p => p.CompanyId == c.CompanyId);
            query.WhereIf(x => x.StartDate.HasValue && x.EndDate.HasValue, c => p => p.InvoiceDate.Date >= c.StartDate && p.InvoiceDate.Date <= c.EndDate);
            query.WhereIf(x => x.DueDate.HasValue, c => p => p.DueDate.Date == c.DueDate.Value);
            var sorts = new Sorts<Invoice>();
            sorts.Add(true, x => x.Consultant.FirstName);
            query.ApplySorts(sorts);

            return query;
        }

        [HttpGet]
        public async Task<InvoiceListItemDto> Search(InvoiceQueryParameter queryParameter)
        {
            var query = GetQuery(queryParameter);
            var details = await query.ExecuteAsync<IncoiceListItemDto>(queryParameter);
            var results = new InvoiceListItemDto
            {
                LastUpdated = Repository.GetAllList().Max(x => x.LastUpdated),
                ListItemDto = details
            };
            return results;
        }

        [HttpGet]
        public async Task<IEnumerable<InvoiceMonthReportDto>> GetInvoicesByMonthReport(InvoiceQueryParameter queryParameter)
        {
            var query = GetQuery(queryParameter);

            return await query.ExecuteAsync((o) =>
                 from t1 in o
                 group t1 by new
                 {
                     t1.InvoiceDate.Month,
                     t1.InvoiceDate.Year
                 } into g

                 select new InvoiceMonthReportDto
                 {
                     Year = g.Key.Year,
                     MonthName = g.Key.Month,
                     MonthAmount = g.Sum(y => y.Total),
                 }, queryParameter);
        }

        [AbpAuthorize("AgingReport")]
        public async Task<IEnumerable<AgeingReportDto>> GetAgeingReport()
        {
            var details = await Repository.GetAllListAsync();
            var childrens = Mapper.Map<List<Children>>(details);
            AgeingReport report = new AgeingReport();
            var result = await report.GetAgeingReport(childrens);
            return result;
        }
    }
}