using Abp.Application.Services;
using Abp.Domain.Repositories;
using Abp.ObjectMapping;
using Accounts.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Intuit
{
    public class IntuitAppservice : ApplicationService, IIntuitAppService
    {
        private readonly IntuitDataProvider IntuitDataProvider;

        private readonly IRepository<Company> CompanyRepository;

        private readonly IRepository<Term> TermRepository;

        private readonly IMapper Mapper;

        public IntuitAppservice(
            IntuitDataProvider intuitDataProvider,
            IRepository<Company> companyRepository,
            IRepository<Term> termRepository,
            IMapper mapper)
        {
            IntuitDataProvider = intuitDataProvider;
            CompanyRepository = companyRepository;
            TermRepository = termRepository;
            Mapper = mapper;
        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = "Intuit")]
        //[AbpAuthorize("Invoicing.Submit")]
        public async Task SyncCompanies()
        {
            var customers = IntuitDataProvider.GetCustomers();
            foreach (var customer in customers)
            {
                var existingCompany = await CompanyRepository.FirstOrDefaultAsync(x => x.ExternalCustomerId == customer.Id);
                var updatedCompany = Mapper.Map(customer, existingCompany);
                CompanyRepository.InsertOrUpdate(updatedCompany);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Intuit")]
        //[AbpAuthorize("Invoicing.Submit")]
        public async Task SyncTerms()
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
}
