using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.ObjectMapping;
using Abp.Runtime.Session;
using Abp.UI;
using Accounts.AzureServices;
using Accounts.Core.Notify;
using Accounts.Data;
using Accounts.Intuit;
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
        private readonly IntuitDataProvider IntuitDataProvider;
        private readonly INotifyService NotifyService;
        private readonly IObjectMapper Mapper;

        private readonly IInvoiceProcessor InvoiceProcessor;
        public InvoicingService(
            IRepository<Invoice> invoiceRepository,
            IRepository<Timesheet> timesheetRepository,
            IInvoiceProcessor invoiceProcessor,
            IRepository<Project> projectRepository,
             IntuitDataProvider intuitDataProvider,
             INotifyService notifyService,
        IObjectMapper mapper)
        {
            InvoiceRepository = invoiceRepository;
            TimesheetRepository = timesheetRepository;
            InvoiceProcessor = invoiceProcessor;
            ProjectRepository = projectRepository;
            IntuitDataProvider = intuitDataProvider;
            NotifyService = notifyService;
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

        public async Task SyncInvoiceAndNotify(IntuitNotifyDto invoice)
        {
            if (invoice.Name == "Payment" && invoice.Operation == "Create")
            {
                await SendPaymentNotification(invoice.Id, invoice.LastUpdated.Date);
            }
            if (invoice.Name == "Invoice")
            {
                switch (invoice.Operation)
                {
                    case "Update":
                        var inv1 = new IntuitData.Invoice { Id = invoice.Id };
                        var Intuit1 = IntuitDataProvider.FindById<IntuitData.Invoice>(inv1);
                        var databaseInv = InvoiceRepository.FirstOrDefault(x => x.QBOInvoiceId == Intuit1.Id);
                        if (databaseInv.Balance != Intuit1.Balance && databaseInv.Balance > Intuit1.Balance)
                        {
                            var newBalance = databaseInv.Balance - Intuit1.Balance;
                            await NotifyService.NotifyPayment(newBalance, Intuit1.CustomerRef.name, databaseInv.EInvoiceId, invoice.LastUpdated.ToString(" dd/MM/yyyy"));
                        }
                        else
                        {
                            await NotifyService.NotifyInvoice(GeteInvoiceId(invoice.Id), "Modified");
                        }
                        await SyncInvoice(invoice.Id);
                        break;
                    case "Emailed":
                        await SyncInvoice(invoice.Id);
                        break;
                    case "Void":
                        await NotifyService.NotifyInvoice(GeteInvoiceId(invoice.Id), "Voided");
                        await SyncInvoice(invoice.Id);
                        break;
                    case "Delete":
                        var toDelete = InvoiceRepository.FirstOrDefault(x => x.QBOInvoiceId == invoice.Id);
                        await NotifyService.NotifyInvoice(toDelete.EInvoiceId, "Deleted");
                        toDelete.Balance = 0;
                        toDelete.IsDeletedInIntuit = true;
                        await InvoiceRepository.InsertOrUpdateAsync(toDelete);
                        break;
                }
            }
        }
        public string GeteInvoiceId(string invoiceId)
        {
            var inv = new IntuitData.Invoice { Id = invoiceId };
            var eInvoiceId = IntuitDataProvider.FindById<IntuitData.Invoice>(inv);
            return eInvoiceId.DocNumber;
        }
        public async Task SendPaymentNotification(string paymentId, DateTime lastUpdatedDate)
        {
            var payment = new IntuitData.Payment { Id = paymentId };
            string invId = "";
            var Intuit = IntuitDataProvider.FindById<IntuitData.Payment>(payment);
            foreach (var item in Intuit.Line)
            {
                if (item.LinkedTxn.Any(x => x.TxnType == "Invoice"))
                {
                    invId = item.LinkedTxn.Where(x => x.TxnType == "Invoice").FirstOrDefault().TxnId;
                    var databaseInv = InvoiceRepository.FirstOrDefault(x => x.QBOInvoiceId == invId);
                    if (databaseInv.EInvoiceId != null)
                    {
                        await NotifyService.NotifyPayment(item.Amount, Intuit.CustomerRef.name, databaseInv.EInvoiceId, lastUpdatedDate.ToString(" dd/MM/yyyy"));
                        await SyncInvoice(invId);
                    }
                    
                }                             
            }


        }
    }
}
