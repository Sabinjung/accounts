using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Abp.ObjectMapping;
using Abp.UI;
using Accounts.AzureServices;
using Accounts.Core.Notify;
using Accounts.Data;
using Accounts.Intuit;
using Accounts.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;
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
        private readonly IRepository<Company> CompanyRepository;
        private readonly IConfiguration Configuration;

        public InvoicingService(
            IRepository<Invoice> invoiceRepository,
            IRepository<Timesheet> timesheetRepository,
            IInvoiceProcessor invoiceProcessor,
            IRepository<Project> projectRepository,
            IntuitDataProvider intuitDataProvider,
            INotifyService notifyService,
            IObjectMapper mapper,
            IRepository<Company> companyRepository,
            IConfiguration configuration
            )
        {
            InvoiceRepository = invoiceRepository;
            TimesheetRepository = timesheetRepository;
            InvoiceProcessor = invoiceProcessor;
            ProjectRepository = projectRepository;
            IntuitDataProvider = intuitDataProvider;
            NotifyService = notifyService;
            Mapper = mapper;
            CompanyRepository = companyRepository;
            Configuration = configuration;
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
        //sync balance from intuit to accounts app
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

        public async Task SyncInvoiceAndNotify(IntuitNotifyDto transaction)
        { 
            //runs when invoice operations are triggered
            if (transaction.Name == "Invoice" || transaction.Name == "Payment")
            {
                switch (transaction.Operation)
                {
                    case "Create":
                        //await NotifyService.NotifyInvoice(GeteInvoiceId(transaction.Id), "Created");
                        if(transaction.Name == "Payment")
                        {
                            await SendPaymentNotification(transaction.Id, "Amount has been paid by vendor.",transaction.Operation);
                        }
                        else
                        {
                            await SyncInvoice(transaction.Id);
                        }
                        break;
                    case "Update":
                        //var inv1 = new IntuitData.Invoice { Id = invoice.Id };
                        //var Intuit1 = IntuitDataProvider.FindById<IntuitData.Invoice>(inv1);
                        //var databaseInv = InvoiceRepository.FirstOrDefault(x => x.QBOInvoiceId == Intuit1.Id);
                        //if (databaseInv.Balance != Intuit1.Balance && databaseInv.Balance > Intuit1.Balance && databaseInv.DiscountAmount == Intuit1.DiscountAmt)
                        //{
                        //    var newBalance = databaseInv.Balance - Intuit1.Balance;
                        //    await NotifyService.NotifyPayment(newBalance, Intuit1.CustomerRef.name, databaseInv.EInvoiceId, invoice.LastUpdated.ToString(" dd/MM/yyyy"));
                        //}
                        if (transaction.Name == "Payment")
                        {
                            await SendPaymentNotification(transaction.Id, "Payment has been updated",transaction.Operation);
                        }
                        else
                        {
                            await NotifyService.NotifyInvoice(GeteInvoiceId(transaction.Id), "edited");
                            await SyncInvoice(transaction.Id);
                        }
                        break;
                    case "Emailed":
                        if (transaction.Name == "Payment")
                        {
                            await SendPaymentNotification(transaction.Id, "Payment has been emailed",transaction.Operation);
                        }
                        else
                        {
                            await SyncInvoice(transaction.Id);
                        }
                        break;
                    case "Void":
                            await NotifyService.NotifyInvoice(GeteInvoiceId(transaction.Id), "voided");
                            await SyncInvoice(transaction.Id);
                        break;
                    case "Delete":
                            var toDelete = InvoiceRepository.FirstOrDefault(x => x.QBOInvoiceId == transaction.Id);
                            await NotifyService.NotifyInvoice(toDelete.EInvoiceId, "deleted");
                            toDelete.Balance = 0;
                            toDelete.IsDeletedInIntuit = true;
                            await InvoiceRepository.InsertOrUpdateAsync(toDelete);
                        break;
                }
            }
        }
        //gets eInvoiceId from etransId
        public string GeteInvoiceId(string invoiceId)
        {
            var inv = new IntuitData.Invoice { Id = invoiceId };
            var eInvoiceId = IntuitDataProvider.FindById<IntuitData.Invoice>(inv);
            return eInvoiceId.DocNumber;
        }
        //gets invoice id from paytment id, sends notification and sync balance
        public async Task SendPaymentNotification(string transId, string titleMsg,string operation)
        {
            var paymentId = new IntuitData.Payment { Id = transId };
            string invId = "";
            var payment = IntuitDataProvider.FindById<IntuitData.Payment>(paymentId);
            var message = "";
            var invoiceUrl = Configuration.GetSection("App:ServerRootAddress").Value;
            foreach (var item in payment.Line)
            {
                if (item.LinkedTxn.Any(x => x.TxnType == "Invoice"))
                {
                    invId = item.LinkedTxn.Where(x => x.TxnType == "Invoice").FirstOrDefault().TxnId;
                    var intuitId = new IntuitData.Invoice { Id = invId };
                    var intuitInvoice = IntuitDataProvider.FindById<IntuitData.Invoice>(intuitId);
                    var databaseInv = InvoiceRepository.FirstOrDefaultAsync(x => x.QBOInvoiceId == invId).Result;
                    var customerName = CompanyRepository.FirstOrDefaultAsync(x => x.Id == databaseInv.CompanyId).Result.DisplayName;
                    if (databaseInv.EInvoiceId != null)
                    {
                        message +=
                        $"{titleMsg}\n" +
                        $"Date: {payment.TxnDate.Date.ToString(" MM/dd/yyyy")}\n";
                        if (operation == "Create") { message += $"Amount Received: ${item.Amount}\n"; }
                        if (operation == "Create" || operation == "Update") { message += $"Remaining Balance: ${intuitInvoice.Balance}\n"; }
                        message +=
                        $"Customer Name: {customerName}\n" +
                        $"eInvoice ID: {databaseInv.EInvoiceId}\n" +
                        $"Invoice link to Accounts application: {invoiceUrl + "invoices/" + databaseInv.Id + "\n"}\n";
                                
                    }
                }                             
            }
            await NotifyService.NotifyPayment(message);
            await SyncInvoice(invId);

        }
    }
}
