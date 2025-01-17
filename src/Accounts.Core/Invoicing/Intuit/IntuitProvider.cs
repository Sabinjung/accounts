﻿using Abp.Configuration;
using Abp.Dependency;
using Accounts.Core;
using Accounts.Core.Invoicing.Intuit;
using Accounts.Invoicing;
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

        private readonly DefaultEmailSetting DefaultEmailSetting;



        public IntuitDataProvider(AccountsAppSession accountsAppSession, IOptions<IntuitSettings> options, IOptions<DefaultEmailSetting> email)
        {
            AccountsAppSession = accountsAppSession;
            IntuitSettings = options.Value;
            DefaultEmailSetting = email.Value;
        }

        public IEnumerable<Customer> GetCustomers()
        {
            var serviceContext = GetServiceContext();
            // Create a QuickBooks QueryService using ServiceContext
            var customerService = new QueryService<Customer>(serviceContext);
            var customers = customerService.ExecuteIdsQuery("SELECT * FROM Customer WHERE active = true maxresults 1000");
            return customers;
        }

        public IEnumerable<Item> GetItems()
        {
            var serviceContext = GetServiceContext();

            var itemService = new QueryService<Item>(serviceContext);
            var items = itemService.ExecuteIdsQuery("SELECT * FROM Item maxresults 1000");
            return items;
        }

        public string CreateExpense(string expenseName)
        {
          var serviceContext = GetServiceContext();
          DataService service = new DataService(serviceContext);
          Item expense = new Item
          {
              TrackQtyOnHand = true,
              Name = expenseName,
              QtyOnHand = 0,
              IncomeAccountRef = new ReferenceType()
              { Value="1", name = "Services" },
              AssetAccountRef = new ReferenceType() { name = "Inventory Assest" },
              InvStartDate = DateTime.Now,
              TypeSpecified = true,
              Type = ItemTypeEnum.Service,
              ExpenseAccountRef = new ReferenceType() { name = "Cost", Value = "10" }
            };
            var result  = service.Add<Item>(expense);
            return result.Id;
        }

        public IEnumerable<PaymentMethod> GetPaymentMethod()
        {
            var serviceContext = GetServiceContext();
            // Create a QuickBooks QueryService using ServiceContext
            var customerService = new QueryService<PaymentMethod>(serviceContext);
            var paymentMethod = customerService.ExecuteIdsQuery("SELECT * FROM PaymentMethod");
            return paymentMethod;
        }

        public IEnumerable<Term> GetTerms()
        {
            var serviceContext = GetServiceContext();
            // Create a QuickBooks QueryService using ServiceContext
            var termsService = new QueryService<Term>(serviceContext);
            var terms = termsService.ExecuteIdsQuery("SELECT * FROM Term");
            return terms;
        }

        public IEnumerable<Invoice> GetInvoices()
        {
            var serviceContext = GetServiceContext();
            // Create a QuickBooks QueryService using ServiceContext
            var invoiceService = new QueryService<Invoice>(serviceContext);
            var invoices = invoiceService.ExecuteIdsQuery("SELECT * FROM Invoice maxresults 1000");
            return invoices;
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

        public T Update<T>(T entity) where T : IEntity
        {
            var serviceContext = GetServiceContext();
            //Initializing the Dataservice object with ServiceContext
            DataService service = new DataService(serviceContext);

            //Adding the Bill using Dataservice object

            T updated = service.Update<T>(entity);

            return updated;
        }

        public void SendEmail<T>(T entity, Customer customer, bool isMailing) where T : IEntity
        {
            var serviceContext = GetServiceContext();
            var address = "";
            if (isMailing)
                address = customer.PrimaryEmailAddr.Address;
            else
                address = DefaultEmailSetting.EmailAddress;
          
            DataService service = new DataService(serviceContext);

            if (address.Contains(','))
            {
                foreach (string email in address.Split(','))
                {
                    service.SendEmail<T>(entity, email.Trim());
                }
            }
            else
            {
                service.SendEmail<T>(entity, address);
            }
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
        public List<string> FindDeleted(List<IEntity> entity)
        {
            var dtUpper = DateTime.UtcNow.AddDays(-100);
            CDCQuery cdcQuery = new CDCQuery();
            cdcQuery.ChangedSince = dtUpper;
            var serviceContext = GetServiceContext();
            List<string> QboId = new List<string>();
            DataService commonService = new DataService(serviceContext);
            List<Invoice> lstEnt = new List<Invoice>();
            lstEnt.Add(new Invoice());
            IntuitCDCResponse cdcResp = commonService.CDC(entity, dtUpper);
            if (cdcResp.entities.ContainsKey("Invoice"))
                foreach (Invoice qboi in cdcResp.entities["Invoice"])
                {
                    if (qboi.status == EntityStatusEnum.Deleted && qboi.statusSpecified == true)
                    {
                        QboId.Add(qboi.Id);
                    }
                }

            return QboId;
            
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
            ar.IncludeOnSend = true;
            ar.IncludeOnSendSpecified = true;
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

    public class IntuitInvoiceDto
    {
        public string QBOInvoiceId { get; set; }
        public string EInvoiceId { get; set; }
    }
}
