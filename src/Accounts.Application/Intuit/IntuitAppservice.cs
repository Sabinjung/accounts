using Abp.Authorization;
using Abp.Domain.Repositories;
using Accounts.Intuit.Dto;
using Accounts.Core.Notify;
using Accounts.Models;
using AutoMapper;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using IntuitData = Intuit.Ipp.Data;
using Abp.UI;

namespace Accounts.Intuit
{
    [AbpAuthorize]
    public class IntuitAppService : AccountsAppServiceBase, IIntuitAppService
    {
        private readonly IntuitDataProvider IntuitDataProvider;

        private readonly IRepository<Company> CompanyRepository;
        private readonly IRepository<Project> ProjectRepository;
        private readonly IRepository<Term> TermRepository;

        private readonly IRepository<PaymentMethod> PaymentMethodRepository;

        private readonly IRepository<Invoice> InvoiceRepository;
        private readonly IRepository<InvoiceCycle> InvoiceCycleRepository;


        private readonly IMapper Mapper;

        private readonly OAuth2Client OAuth2Client;

        public IntuitAppService(
            IntuitDataProvider intuitDataProvider,
            IRepository<Company> companyRepository,
            IRepository<Project> projectRepository,
            IRepository<Term> termRepository,
            IRepository<PaymentMethod> paymentMethodRepository,
            IRepository<Invoice> invoiceRepository,
            IRepository<InvoiceCycle> invoiceCycleRepository,
            OAuth2Client oAuth2CLient,
            IMapper mapper)
        {
            IntuitDataProvider = intuitDataProvider;
            CompanyRepository = companyRepository;
            ProjectRepository = projectRepository;
            TermRepository = termRepository;
            PaymentMethodRepository = paymentMethodRepository;
            InvoiceRepository = invoiceRepository;
            InvoiceCycleRepository = invoiceCycleRepository;
            Mapper = mapper;
            OAuth2Client = oAuth2CLient;

        }


        [HttpGet]
        [AbpAuthorize("Company.Sync")]
        public async Task SyncCompanies()
        {
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var customers = IntuitDataProvider.GetCustomers();
                foreach (var customer in customers)
                {
                    var existingCompany = await CompanyRepository.FirstOrDefaultAsync(x => x.ExternalCustomerId == customer.Id);
                    var updatedCompany = Mapper.Map(customer, existingCompany);
                    if (customer.SalesTermRef != null)
                    {
                        var existingTerm = await TermRepository.FirstOrDefaultAsync(x => x.ExternalTermId == customer.SalesTermRef.Value);
                        if (existingTerm != null)
                            updatedCompany.Term = existingTerm;
                        
                    }
                    if(customer.PaymentMethodRef !=null)
                    {
                        var existingPayment = await PaymentMethodRepository.FirstOrDefaultAsync(x => x.ExternalPaymentId == customer.PaymentMethodRef.Value);
                        if (existingPayment != null)
                          updatedCompany.PaymentMethod = existingPayment;

                    }
                    CompanyRepository.InsertOrUpdate(updatedCompany);
                }
            }
        }

        [HttpGet]
        [AbpAuthorize("Company.Sync")]
        public async Task SyncPaymentMethods()
        {
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var paymentMethods = IntuitDataProvider.GetPaymentMethod();
                foreach (var paymentMethod in paymentMethods)
                {
                    var existingPaymentMethod = await PaymentMethodRepository.FirstOrDefaultAsync(x => x.ExternalPaymentId == paymentMethod.Id);
                    var updatedTerm = Mapper.Map(paymentMethod, existingPaymentMethod);
                    PaymentMethodRepository.InsertOrUpdate(updatedTerm);
                }
            }
        }

        [HttpGet]
        public async Task<IntuitCompanyDto> GetCompany(string externalCustomerId)
        {
            var intuitCompanyDto = new IntuitCompanyDto();

            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if(isConnectionEstablished)
            {
                var customer = IntuitDataProvider.GetCustomers().FirstOrDefault(x => x.Id == externalCustomerId);
                if (customer != null)
                {
                    var companyInvoiceCycle= CompanyRepository.FirstOrDefaultAsync(x=>x.ExternalCustomerId == customer.Id);
                    intuitCompanyDto.ExternalCustomerId = externalCustomerId;
                    var companyDto = Mapper.Map(customer, intuitCompanyDto);
                    if (companyInvoiceCycle.Result.InvoiceCycle != null)
                    {
                        companyDto.InvoiceCycleId = (int)companyInvoiceCycle.Result.InvoiceCycleId;

                    }
                    if(companyInvoiceCycle.Result.InvoiceCycle ==null)
                    {
                        companyDto.InvoiceCycleId = 1;
                    }
                    return companyDto;

                }
            }
            throw new UserFriendlyException("Warnings!!", "No Company Found.");

        }
        [HttpPost]
        [AbpAuthorize("Company.Edit")]
        public async Task EditCompany(IntuitCompanyDto data)
        {
            try
            {
                var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
                if (isConnectionEstablished)
                {
                    var cus = new IntuitData.Customer { Id = data.ExternalCustomerId };
                    var existingCompany = await CompanyRepository.FirstOrDefaultAsync(x => x.ExternalCustomerId == cus.Id);
                    var customer = IntuitDataProvider.FindById<IntuitData.Customer>(cus);
                    var updatedCus = Mapper.Map(data, customer);
                    updatedCus.SyncToken = customer.SyncToken;
                    IntuitDataProvider.Update(updatedCus);

                    var updatedDatabaseCompany = Mapper.Map(updatedCus, existingCompany);
                    if (updatedCus.SalesTermRef != null)
                    {
                        var existingTerm = await TermRepository.FirstOrDefaultAsync(x => x.ExternalTermId == updatedCus.SalesTermRef.Value);
                        var existingInvoiceCycle = await InvoiceCycleRepository.FirstOrDefaultAsync(x => x.Id == data.InvoiceCycleId);
                        var existingPaymentMethod = await PaymentMethodRepository.FirstOrDefaultAsync(x => x.ExternalPaymentId == updatedCus.PaymentMethodRef.Value);


                        if (existingTerm != null || existingInvoiceCycle != null || existingPaymentMethod != null)
                        {
                            updatedDatabaseCompany.Term = existingTerm;
                            updatedDatabaseCompany.InvoiceCycle = existingInvoiceCycle;
                            updatedDatabaseCompany.PaymentMethod = existingPaymentMethod;

                        }
                    }
                    CompanyRepository.InsertOrUpdate(updatedDatabaseCompany);
                }
            }
            catch(Exception ex)
            {
                throw new UserFriendlyException("Company Create Failed!!", "Company already exists.");

            }

        }

        [HttpPost]
        [AbpAuthorize("Company.Create")]
        public async Task CreateCompany(IntuitCompanyDto data)
        {
            try
            {
                var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
                if (isConnectionEstablished)
                {
                    var customer = Mapper.Map<IntuitData.Customer>(data);
                    var cus = IntuitDataProvider.Add(customer);
                    var existingCompany = await CompanyRepository.FirstOrDefaultAsync(x => x.ExternalCustomerId == cus.Id);
                    var updatedCompany = Mapper.Map(cus, existingCompany);
                    if (customer.SalesTermRef != null)
                    {
                        var existingTerm = await TermRepository.FirstOrDefaultAsync(x => x.ExternalTermId == customer.SalesTermRef.Value);
                        var existingInvoiceCycle = await InvoiceCycleRepository.FirstOrDefaultAsync(x => x.Id == data.InvoiceCycleId);
                        var existingPayment = await PaymentMethodRepository.FirstOrDefaultAsync(x => x.ExternalPaymentId == customer.PaymentMethodRef.Value);
                        if (existingTerm != null)
                        {
                            updatedCompany.Term = existingTerm;
                            updatedCompany.InvoiceCycle = existingInvoiceCycle;
                            updatedCompany.PaymentMethod = existingPayment;
                        }
                    }
                    CompanyRepository.InsertOrUpdate(updatedCompany);
                }
            }
            catch(Exception ex) 
            { 
                throw new UserFriendlyException("Company Create Failed!!", "Company already exists."); 
            }
            
        }

        [HttpGet]
        [AbpAuthorize("Company.Sync")]
        public async Task SyncTerms()
        {
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var terms = IntuitDataProvider.GetTerms();
                foreach (var term in terms)
                {
                    var existingTerm = await TermRepository.FirstOrDefaultAsync(x => x.ExternalTermId == term.Id);
                    var updatedTerm = Mapper.Map(term, existingTerm);
                    TermRepository.InsertOrUpdate(updatedTerm);
                }
            }
        }

        [AbpAllowAnonymous]
        [HttpGet]
        public async Task SyncInvoices()
        {
            var getAllInvoices = InvoiceRepository.GetAll().Where(x => x.EInvoiceId == null);
            if (getAllInvoices != null)
            {
                foreach (var item in getAllInvoices)
                {
                    if (item.QBOInvoiceId != null)
                    {
                        string id = item.QBOInvoiceId;
                        var data = new IntuitData.Invoice { Id = id };
                        var invoice = IntuitDataProvider.FindById<IntuitData.Invoice>(data);
                        if(invoice!= null)
                        {
                            item.EInvoiceId = invoice.DocNumber;
                            InvoiceRepository.InsertOrUpdate(item);
                        }
                       
                    }
                }
            }
            var isConnectionEstablished = await OAuth2Client.EstablishConnection(SettingManager);
            if (isConnectionEstablished)
            {
                var invoices = IntuitDataProvider.GetInvoices();
                foreach (var invoice in invoices)
                {
                    var existingInvoice = await InvoiceRepository.FirstOrDefaultAsync(x => x.QBOInvoiceId == invoice.Id);
                    if(existingInvoice != null)
                    {
                        if (invoice.DueDate < DateTime.UtcNow)
                        {
                            existingInvoice.DueDays = (DateTime.UtcNow - invoice.DueDate).Days;
                        }
                        if (invoice.Balance != existingInvoice.Balance)
                        {
                            existingInvoice.LastUpdated = DateTime.UtcNow;
                        }
                        var updatedInvoice = Mapper.Map(invoice, existingInvoice);
                        updatedInvoice.Id = existingInvoice.Id;
                        InvoiceRepository.Update(updatedInvoice);
                    }
                }
            }
        }
    }
}
