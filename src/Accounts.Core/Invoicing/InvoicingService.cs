using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Accounts.Data;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IntuitData = Intuit.Ipp.Data;

namespace Accounts.Core.Invoicing
{
    public class InvoicingService : DomainService, IInvoicingService
    {
        private readonly IRepository<Invoice> InvoiceRepository;

        private readonly IRepository<Timesheet> TimesheetRepository;

        private readonly IObjectMapper Mapper;

        private readonly IInvoiceProcessor InvoiceProcessor;
        public InvoicingService(
            IRepository<Invoice> invoiceRepository,
            IRepository<Timesheet> timesheetRepository,
            IInvoiceProcessor invoiceProcessor,
            IObjectMapper mapper)
        {
            InvoiceRepository = invoiceRepository;
            TimesheetRepository = timesheetRepository;
            InvoiceProcessor = invoiceProcessor;
            Mapper = mapper;
        }
        public async Task<Invoice> GenerateInvoice(int timesheetId, int userId)
        {
            var timesheet = await TimesheetRepository.GetAsync(timesheetId);
            var generatedInvoice = Mapper.Map<Invoice>(timesheet);
            var invoice = InvoiceRepository.Insert(generatedInvoice);

            timesheet.Invoice = invoice;
            timesheet.StatusId = (int)TimesheetStatuses.InvoiceGenerated;
            return invoice;


        }
        public async Task<string> Submit(int invoiceId, int userId)
        {
            var invoice = await InvoiceRepository.GetAsync(invoiceId);
            var timesheet = await TimesheetRepository.FirstOrDefaultAsync(t => t.InvoiceId == invoiceId);
            var referenceNo = await InvoiceProcessor.Send(invoice);
            invoice.QBOInvoiceId = referenceNo;
            timesheet.StatusId = (int)TimesheetStatuses.Invoiced;
            timesheet.InvoiceGeneratedByUserId = userId;
            timesheet.InvoiceGeneratedDate = DateTime.UtcNow;

            return await Task.FromResult(referenceNo);
        }
    }
}
