using Abp.Authorization;
using Abp.Domain.Repositories;
using Accounts.Models;
using AutoMapper;
using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Accounts.Intuit
{
    [AbpAuthorize]
    public class IntuitAppService : AccountsAppServiceBase, IIntuitAppService
    {
        private readonly IntuitDataProvider IntuitDataProvider;

        private readonly IRepository<Company> CompanyRepository;

        private readonly IRepository<Term> TermRepository;

        private readonly IMapper Mapper;

        private readonly OAuth2Client OAuth2Client;

        public IntuitAppService(
            IntuitDataProvider intuitDataProvider,
            IRepository<Company> companyRepository,
            IRepository<Term> termRepository,
            OAuth2Client oAuth2CLient,
            IMapper mapper)
        {
            IntuitDataProvider = intuitDataProvider;
            CompanyRepository = companyRepository;
            TermRepository = termRepository;
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

    }
}
