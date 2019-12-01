using Abp.Configuration;
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
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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


        public List<T> FindAll<T>(T entity, int startPosition = 1, int maxResults = 100) where T : IEntity
        {
            var serviceContext = GetServiceContext();
            DataService service = new DataService(serviceContext);
            ReadOnlyCollection<T> entityList = service.FindAll(entity, startPosition, maxResults);
            return entityList.ToList<T>();
        }

        public T FindById<T>(T entity) where T : IEntity
        {
            var serviceContext = GetServiceContext();
            DataService service = new DataService(serviceContext);
            T foundEntity = service.FindById(entity);

            return foundEntity;
        }


        public Account FindOrAddAccount(AccountTypeEnum accountType, string subType, string accountName)
        {
            var serviceContext = GetServiceContext();
            Account typeOfAccount = null;
            List<Account> listOfAccount = FindAll<Account>(new Account(), 1, 500);
            if (listOfAccount.Count > 0)
            {
                foreach (Account acc in listOfAccount)
                {

                    if (acc.AccountType == accountType && acc.AccountSubType == subType && acc.status != EntityStatusEnum.SyncError)
                    {
                        typeOfAccount = acc;
                        break;
                    }
                }
            }

            if (typeOfAccount == null)
            {
                DataService service = new DataService(serviceContext);
                Account account;
                account = new Account();
                account.Name = accountName;

                account.AccountType = accountType;

                Account createdAccount = service.Add<Account>(account);
                typeOfAccount = createdAccount;
            }

            return typeOfAccount;
        }

        public void UploadFiles(string invoiceNo, List<FileForIntuitUploadDTO> attachments)
        {
            foreach (var attachment in attachments)
            {
                Attachable attachable = CreateAttachableForUpload(invoiceNo);
                attachable.ContentType = attachment.ContentType;
                attachable.FileName = attachment.FileName;
                Upload(attachable, attachment.Stream);
            }
        }

        private Attachable Upload(Attachable attachable, Stream stream)
        {
            var serviceContext = GetServiceContext();
            var service = new DataService(serviceContext);
            var uploaded = service.Upload(attachable, stream);
            return uploaded;
        }


        private Attachable CreateAttachableForUpload(string invoiceNo)
        {
            var attachable = new Attachable();
            var attachableRefs = new AttachableRef[1];
            var ar = new AttachableRef();
            ar.EntityRef = new ReferenceType();
            ar.EntityRef.type = objectNameEnumType.Invoice.ToString();
            ar.EntityRef.name = objectNameEnumType.Invoice.ToString();
            ar.EntityRef.Value = invoiceNo;
            attachableRefs[0] = ar;
            attachable.AttachableRef = attachableRefs;
            return attachable;
        }
    }

    public class ErrorMessages
    {
        //Intuit
        public static string INTUIT_UNAUTHORIZED_MESSAGE = "Unauthorized-401";
        public static string INTUIT_AUTH_NULL = "Null auth";

        //File
        public static string FILE_NOT_FOUND = "File not found";

    }

    public class FileForIntuitUploadDTO
    {
        public string ContentType { get; set; }

        public Stream Stream { get; set; }

        public string FileName { get; set; }

    }
}
