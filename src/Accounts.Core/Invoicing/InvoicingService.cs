using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.UI;
using Accounts.Data;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using IntuitData = Intuit.Ipp.Data;

namespace Accounts.Core.Invoicing
{
    public class InvoicingService : DomainService, IInvoicingService
    {
        private readonly IRepository<Invoice> InvoiceRepository;

        private readonly IRepository<Timesheet> TimesheetRepository;

        private readonly IRepository<Project> ProjectRepository;

        private readonly IObjectMapper Mapper;

        private readonly IInvoiceProcessor InvoiceProcessor;
        public InvoicingService(
            IRepository<Invoice> invoiceRepository,
            IRepository<Timesheet> timesheetRepository,
            IInvoiceProcessor invoiceProcessor,
            IRepository<Project> projectRepository,
        IObjectMapper mapper)
        {
            InvoiceRepository = invoiceRepository;
            TimesheetRepository = timesheetRepository;
            InvoiceProcessor = invoiceProcessor;
            ProjectRepository = projectRepository;
            Mapper = mapper;
        }
        public async Task<Invoice> GenerateInvoice(int timesheetId, int userId, bool shouldAssociate= false)
        {
            var timesheet = await TimesheetRepository.GetAsync(timesheetId);

            if (timesheet.InvoiceId.HasValue)
            {
                throw new UserFriendlyException("Invoice is already generated.");
            }
            var generatedInvoice = Mapper.Map<Invoice>(timesheet);
            
            return generatedInvoice;

        }
        public async Task Submit(int timesheetId, int userId, bool isMailing)
        {
            var timesheet = await TimesheetRepository.FirstOrDefaultAsync(timesheetId);
            if (timesheet.InvoiceId.HasValue && !string.IsNullOrEmpty(timesheet.Invoice.QBOInvoiceId))
            {
                throw new UserFriendlyException("Invoice is already submitted.");
            }
            var invoice = await GenerateInvoice(timesheetId, userId);         
            timesheet.Invoice = invoice;
            timesheet.StatusId = (int)TimesheetStatuses.InvoiceGenerated;   
            var referenceNo = await InvoiceProcessor.Send(invoice, isMailing);
            invoice.QBOInvoiceId = referenceNo.QBOInvoiceId;
            invoice.EInvoiceId = referenceNo.EInvoiceId;
            timesheet.StatusId = (int)TimesheetStatuses.Invoiced;
            timesheet.InvoiceGeneratedByUserId = userId;
            timesheet.InvoiceGeneratedDate = DateTime.UtcNow;
        

            //return await Task.FromResult(referenceNo);
        }

        public async Task<string> Save(int timesheetId, int userId, string referenceNo)
        {
            var timesheet = await TimesheetRepository.FirstOrDefaultAsync(timesheetId);
            if (timesheet.InvoiceId.HasValue && !string.IsNullOrEmpty(timesheet.Invoice.QBOInvoiceId))
            {
                throw new UserFriendlyException("Invoice is already submitted.");
            }
            var invoice = await GenerateInvoice(timesheetId, userId);
            timesheet.Invoice = invoice;
            timesheet.StatusId = (int)TimesheetStatuses.InvoiceGenerated;
            invoice.QBOInvoiceId = referenceNo;
            timesheet.StatusId = (int)TimesheetStatuses.Invoiced;
            timesheet.InvoiceGeneratedByUserId = userId;
            timesheet.InvoiceGeneratedDate = DateTime.UtcNow;

            return await Task.FromResult(referenceNo);
        }

        public async Task ReadInvoice(int invoiceId)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateAndSendMail(int invoiceId, bool isMailing)
        {
            var invoice = await InvoiceRepository.GetAsync(invoiceId);
            await InvoiceProcessor.UpdateAndSend(invoice, isMailing);
        }

        public async Task SyncInvoice(string invoiceId)
        {
            var balance = await InvoiceProcessor.SyncInvoice(invoiceId);
            var invoice = InvoiceRepository.GetAllList().Where(x => x.QBOInvoiceId == invoiceId).FirstOrDefault();
            if(invoice == null)
                throw new UserFriendlyException("Invoice not found.");

            invoice.Balance = balance;
            invoice.LastUpdated = DateTime.UtcNow;
            await InvoiceRepository.UpdateAsync(invoice);
        }
    }
}
