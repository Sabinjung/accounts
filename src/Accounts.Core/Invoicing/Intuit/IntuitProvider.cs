using Abp.Dependency;
using Accounts.Core;
using Accounts.Core.Invoicing.Intuit;
using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using Intuit.Ipp.Exception;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.ReportService;
using Intuit.Ipp.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounts.Intuit
{
    public class IntuitDataProvider : ISingletonDependency
    {
        private readonly AccountsAppSession AccountsAppSession;

        private readonly IntuitSettings IntuitSettings;

        public IntuitDataProvider(AccountsAppSession accountsAppSession, IOptions<IntuitSettings> options)
        {
            AccountsAppSession = accountsAppSession;
            IntuitSettings = options.Value;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var serviceContext = GetServiceContext();
            // Create a QuickBooks QueryService using ServiceContext
            var customerService = new QueryService<Customer>(serviceContext);
            var customers = customerService.ExecuteIdsQuery("SELECT * FROM Customer WHERE active = true");
            return customers;
        }


        public IEnumerable<Term> GetTerms()
        {
            var serviceContext = GetServiceContext();
            // Create a QuickBooks QueryService using ServiceContext
            var termsService = new QueryService<Term>(serviceContext);
            var terms = termsService.ExecuteIdsQuery("SELECT * FROM Term");
            return terms;
        }




        private ServiceContext GetServiceContext()
        {
            var oauthValidator = new OAuth2RequestValidator(AccountsAppSession.AccessToken);

            // Create a ServiceContext with Auth tokens and realmId
            var serviceContext = new ServiceContext(AccountsAppSession.RealmId, IntuitServicesType.QBO, oauthValidator);
            serviceContext.IppConfiguration.MinorVersion.Qbo = IntuitSettings.MinorVersion;
            serviceContext.IppConfiguration.BaseUrl.Qbo = IntuitSettings.ApiUrl;
            serviceContext.IppConfiguration.Message.Request.SerializationFormat = SerializationFormat.Json;
            serviceContext.IppConfiguration.Message.Response.SerializationFormat = SerializationFormat.Json;
            return serviceContext;
        }

        public T Add<T>(T entity) where T : IEntity
        {
            var serviceContext = GetServiceContext();
            //Initializing the Dataservice object with ServiceContext
            DataService service = new DataService(serviceContext);

            //Adding the Bill using Dataservice object

            T added = service.Add<T>(entity);

            return added;
        }
    }
}
