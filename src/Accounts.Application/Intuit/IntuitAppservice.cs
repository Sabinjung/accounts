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

        private readonly IRepository<Term> TermRepository;

        private readonly IRepository<PaymentMethod> PaymentMethodRepository;

        private readonly IRepository<Invoice> InvoiceRepository;

        private readonly IMapper Mapper;

        private readonly OAuth2Client OAuth2Client;

        public IntuitAppService(
            IntuitDataProvider intuitDataProvider,
            IRepository<Company> companyRepository,
            IRepository<Term> termRepository,
            IRepository<PaymentMethod> paymentMethodRepository,
            IRepository<Invoice> invoiceRepository,
            OAuth2Client oAuth2CLient,
            IMapper mapper)
        {
            IntuitDataProvider = intuitDataProvider;
            CompanyRepository = companyRepository;
            TermRepository = termRepository;
            PaymentMethodRepository = paymentMethodRepository;
            InvoiceRepository = invoiceRepository;
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
                        {
                            updatedCompany.Term = existingTerm;
                        }
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
                        if (existingTerm != null)
                        {
                            updatedCompany.Term = existingTerm;
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
