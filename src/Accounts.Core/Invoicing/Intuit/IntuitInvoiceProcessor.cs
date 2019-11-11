using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Accounts.Intuit;
using Accounts.Models;
using Intuit.Ipp.Core;
using Intuit.Ipp.Core.Configuration;
using Intuit.Ipp.QueryFilter;
using Intuit.Ipp.Security;
using Microsoft.Extensions.Options;
using IntuitData = Intuit.Ipp.Data;

namespace Accounts.Core.Invoicing.Intuit
{
    public class IntuitInvoiceProcessor : IInvoiceProcessor, ISingletonDependency
    {
        private readonly IntuitDataProvider IntuitDataProvider;



        public IntuitInvoiceProcessor(IntuitDataProvider intuitDataProvider)
        {
            IntuitDataProvider = intuitDataProvider;

        }
        public async Task<int> Send(Invoice invoice)
        {

            var intuitInvoice = new IntuitData.Invoice
            {
                Deposit = 0,
                DepositSpecified = true,
                DueDate = invoice.DueDate,
                DueDateSpecified = true,
                PrintStatus = IntuitData.PrintStatusEnum.NotSet,
                PrintStatusSpecified = true,
                EmailStatus = IntuitData.EmailStatusEnum.NotSet,
                EmailStatusSpecified = true,
                TxnDate = invoice.InvoiceDate,
                TxnDateSpecified = true,
                TotalAmt = invoice.Total,
                TotalAmtSpecified = true
            };

            intuitInvoice.CustomerRef = new IntuitData.ReferenceType()
            {
                name = invoice.Company.DisplayName,
                Value = invoice.Company.ExternalCustomerId.ToString()
            };

            intuitInvoice.SalesTermRef = new IntuitData.ReferenceType()
            {
                name = invoice.Term.Name,
                Value = invoice.TermId.ToString()
            };

            AddCustomFields(intuitInvoice, invoice);
            //var accountForDiscount = IntuitHelperService.FindOrAddAccount(serviceContext, IntuitData.AccountTypeEnum.Income, "DiscountsRefundsGiven", "Discount given");
            //AddLines(intuitInvoice, invoice, accountForDiscount);

            //IntuitHelperService.Add(serviceContext, intuitInvoice);
            IntuitDataProvider.Add(intuitInvoice);
            return 1;

        }


        private void AddCustomFields(IntuitData.Invoice intuitInvoice, Invoice invoice)
        {
            var consultant = invoice.Consultant;

            var customFieldsForCandidateInfo = new List<IntuitData.CustomField>();

            //Candidate Name
            var candidateCustomField = new IntuitData.CustomField();
            candidateCustomField.Name = "Candidate Name";
            candidateCustomField.Type = IntuitData.CustomFieldTypeEnum.StringType;
            candidateCustomField.AnyIntuitObject = consultant.FirstName + " " + consultant.LastName;
            candidateCustomField.DefinitionId = "1";

            //Memo
            var memoCustomField = new IntuitData.CustomField();
            memoCustomField.Name = "Memo";
            memoCustomField.Type = IntuitData.CustomFieldTypeEnum.StringType;
            memoCustomField.AnyIntuitObject = invoice.Memo;
            memoCustomField.DefinitionId = "2";

            customFieldsForCandidateInfo.Add(candidateCustomField);
            customFieldsForCandidateInfo.Add(memoCustomField);

            intuitInvoice.CustomField = customFieldsForCandidateInfo.ToArray();

        }


        private void AddLines(IntuitData.Invoice intuitInvoice, Invoice invoice, IntuitData.Account accountForDiscount)
        {
            var lineList = new List<IntuitData.Line>();
            var line = new IntuitData.Line();
            line.Id = "1";
            line.LineNum = "1";
            line.Description = invoice.Description;
            line.Amount = invoice.SubTotal;
            line.AmountSpecified = true;
            line.DetailType = IntuitData.LineDetailTypeEnum.SalesItemLineDetail;
            line.DetailTypeSpecified = true;
            line.AnyIntuitObject = new IntuitData.SalesItemLineDetail()
            {
                //ServiceDate = invoiceEntity.ServiceDate,
                //ServiceDateSpecified = true,
                Qty = new Decimal(invoice.TotalHours),
                QtySpecified = true,
                AnyIntuitObject = invoice.Rate,
                ItemElementName = IntuitData.ItemChoiceType.UnitPrice,
            };



            var subTotalLine = new IntuitData.Line();
            subTotalLine.Id = "2";
            subTotalLine.LineNum = "2";
            subTotalLine.DetailType = IntuitData.LineDetailTypeEnum.SubTotalLineDetail;
            subTotalLine.DetailTypeSpecified = true;
            subTotalLine.Amount = invoice.SubTotal;


            //Discount
            var discountLine = new IntuitData.Line();
            discountLine.Id = "3";
            discountLine.LineNum = "3";
            discountLine.DetailType = IntuitData.LineDetailTypeEnum.DiscountLineDetail;
            discountLine.DetailTypeSpecified = true;

            //discountLine.Amount = 0;            
            //discountLine.AmountSpecified = true;


            var discountLineDetail = new IntuitData.DiscountLineDetail();




            if (invoice.IsDiscountPercentageApplied)
            {
                discountLineDetail.DiscountPercent = new Decimal(invoice.DiscountPercentage);
                discountLineDetail.DiscountPercentSpecified = true;
                discountLineDetail.PercentBased = true;

            }
            else
            {
                discountLine.Amount = invoice.DiscountAmount;
                discountLine.AmountSpecified = true;
                discountLineDetail.PercentBased = false;

            }
            discountLineDetail.PercentBasedSpecified = true;
            discountLine.AnyIntuitObject = discountLineDetail;
            discountLineDetail.DiscountAccountRef = new IntuitData.ReferenceType()
            {
                name = "Discounts given",
                Value = accountForDiscount.Id
            };

            lineList.Add(line);
            lineList.Add(subTotalLine);
            lineList.Add(discountLine);
            intuitInvoice.Line = lineList.ToArray();
        }


    }
}
