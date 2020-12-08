 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.UI;
using Accounts.Blob;
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

        private readonly IAzureBlobService AzureBlobService;

        public IntuitInvoiceProcessor(IntuitDataProvider intuitDataProvider, IAzureBlobService azureBlobService)
        {
            IntuitDataProvider = intuitDataProvider;
            AzureBlobService = azureBlobService;

        }

        public async Task<IntuitInvoiceDto> Send(Invoice invoice, bool isMailing)
        {
            var cus = new IntuitData.Customer { Id = invoice.Company.ExternalCustomerId };
            var customer = IntuitDataProvider.FindById<IntuitData.Customer>(cus);


            var intuitInvoice = new IntuitData.Invoice
            {
                Deposit = 0,
                DepositSpecified = true,
                TxnDate = invoice.InvoiceDate,
                TxnDateSpecified = true,
                TotalAmt = invoice.Total,
                TotalAmtSpecified = true,
                EmailStatus = IntuitData.EmailStatusEnum.NotSet,
                BillEmail = customer.PrimaryEmailAddr,

            };

            intuitInvoice.CustomerRef = new IntuitData.ReferenceType()
            {
                name = invoice.Company.DisplayName,
                Value = invoice.Company.ExternalCustomerId.ToString()
            };

            intuitInvoice.SalesTermRef = new IntuitData.ReferenceType()
            {
                name = invoice.Term.Name,
                Value = invoice.Term.ExternalTermId.ToString()
            };



            AddCustomFields(intuitInvoice, invoice, customer);
            var accountForDiscount = IntuitDataProvider.FindOrAddAccount(IntuitData.AccountTypeEnum.Income, "DiscountsRefundsGiven", "Discount given");
            AddLines(intuitInvoice, invoice, accountForDiscount);
            var savedInvoice = IntuitDataProvider.Add(intuitInvoice);

            try
            {
                // Include Attachments
                var invoiceAttachments = new List<FileForIntuitUploadDTO>();
                foreach (var attachment in invoice.Attachments)
                {
                    var dto = new FileForIntuitUploadDTO();
                    var blob = await AzureBlobService.DownloadFilesAsync(attachment.Name);
                    dto.Stream = await blob.OpenReadAsync();
                    dto.ContentType = attachment.ContentType;
                    dto.FileName = attachment.Name;
                    invoiceAttachments.Add(dto);
                }
                IntuitDataProvider.UploadFiles(savedInvoice.Id, invoiceAttachments);

                if(isMailing == true)
                    IntuitDataProvider.SendEmail(savedInvoice, customer);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
            var invoiceInfo = new IntuitInvoiceDto
            {
                EInvoiceId = savedInvoice.DocNumber,
                QBOInvoiceId = savedInvoice.Id
            };
            return invoiceInfo;
        }

        public async Task UpdateAndSend(Invoice invoice, bool isMailing)
        {
            try
            {
                var cus = new IntuitData.Customer { Id = invoice.Company.ExternalCustomerId };
                var customer = IntuitDataProvider.FindById<IntuitData.Customer>(cus);
                var inv = new IntuitData.Invoice { Id = invoice.QBOInvoiceId };
                var existinginvoice = IntuitDataProvider.FindById<IntuitData.Invoice>(inv);

                var intuitInvoice = new IntuitData.Invoice
                {
                    Deposit = 0,
                    DepositSpecified = true,
                    TxnDate = invoice.InvoiceDate,
                    TxnDateSpecified = true,
                    TotalAmt = invoice.Total,
                    TotalAmtSpecified = true,
                    EmailStatus = IntuitData.EmailStatusEnum.NotSet,
                    BillEmail = customer.PrimaryEmailAddr,
                    DocNumber = invoice.EInvoiceId,
                    Id = invoice.QBOInvoiceId,
                    SyncToken = existinginvoice.SyncToken,
                    domain = "QBO"
                };

                intuitInvoice.CustomerRef = new IntuitData.ReferenceType()
                {
                    name = invoice.Company.DisplayName,
                    Value = invoice.Company.ExternalCustomerId.ToString()
                };

                intuitInvoice.SalesTermRef = new IntuitData.ReferenceType()
                {
                    name = invoice.Term.Name,
                    Value = invoice.Term.ExternalTermId.ToString()
                };

                AddCustomFields(intuitInvoice, invoice, customer);
                var accountForDiscount = IntuitDataProvider.FindOrAddAccount(IntuitData.AccountTypeEnum.Income, "DiscountsRefundsGiven", "Discount given");
                AddLines(intuitInvoice, invoice, accountForDiscount);
                var savedInvoice = IntuitDataProvider.Update(intuitInvoice);

                if (isMailing == true)
                    IntuitDataProvider.SendEmail(savedInvoice, customer);
            }
            catch (Exception e)
            {
                throw new UserFriendlyException(e.Message);
            }
        }
        //public async Task UpdateAndSend(Invoice invoice, bool isMailing)
        //{
        //    try
        //    {
        //        var cus = new IntuitData.Customer { Id = invoice.Company.ExternalCustomerId };
        //        var customer = IntuitDataProvider.FindById<IntuitData.Customer>(cus);
        //        var inv = new IntuitData.Invoice { Id = invoice.QBOInvoiceId };
        //        var existinginvoice = IntuitDataProvider.FindById<IntuitData.Invoice>(inv);
        //        existinginvoice.TxnDate = invoice.InvoiceDate;
        //        existinginvoice.TotalAmt = invoice.Total;
        //        //Line
        //        var line = existinginvoice.Line.Where(x => x.DetailType == IntuitData.LineDetailTypeEnum.SalesItemLineDetail).FirstOrDefault();

        //        line.Amount = invoice.ServiceTotal;
        //        line.AnyIntuitObject = new IntuitData.SalesItemLineDetail()
        //        {
        //            Qty = new Decimal(invoice.TotalHours),
        //            QtySpecified = true,
        //            AnyIntuitObject = invoice.Rate,
        //            ItemElementName = IntuitData.ItemChoiceType.UnitPrice,
        //        };

        //        // Sub Total
        //        var subTotalLine = existinginvoice.Line.Where(x => x.DetailType == IntuitData.LineDetailTypeEnum.SubTotalLineDetail).FirstOrDefault();
        //        if (subTotalLine != null)
        //            subTotalLine.Amount = invoice.SubTotal;

        //        // Discount
        //        var discountLine = existinginvoice.Line.Where(x => x.DetailType == IntuitData.LineDetailTypeEnum.DiscountLineDetail).FirstOrDefault();
        //        var accountForDiscount = IntuitDataProvider.FindOrAddAccount(IntuitData.AccountTypeEnum.Income, "DiscountsRefundsGiven", "Discount given");
        //        if (discountLine != null)
        //        {
        //            var discountLineDetail = new IntuitData.DiscountLineDetail();
        //            if (invoice.DiscountType == Data.DiscountType.Percentage)
        //            {
        //                discountLineDetail.DiscountPercent = Decimal.Round(invoice.DiscountValue.Value, 1);
        //                discountLineDetail.DiscountPercentSpecified = true;
        //                discountLineDetail.PercentBased = true;
        //                discountLineDetail.PercentBasedSpecified = true;
        //            }
        //            else
        //            {
        //                discountLine.AmountSpecified = true;
        //                discountLine.Amount = invoice.DiscountAmount;
        //                discountLineDetail.PercentBased = false;
        //            }

        //            discountLine.AnyIntuitObject = discountLineDetail;
        //            discountLineDetail.DiscountAccountRef = new IntuitData.ReferenceType()
        //            {
        //                name = "Discounts given",
        //                Value = accountForDiscount.Id
        //            };
        //        }
        //        if (discountLine == null && invoice.DiscountAmount != 0)
        //        {
        //            //New Discount
        //            var newDiscountLine = new IntuitData.Line();
        //            newDiscountLine.Id = (existinginvoice.Line.Count() + 1).ToString();
        //            newDiscountLine.LineNum = (existinginvoice.Line.Count() + 1).ToString();
        //            newDiscountLine.DetailType = IntuitData.LineDetailTypeEnum.DiscountLineDetail;
        //            newDiscountLine.DetailTypeSpecified = true;

        //            newDiscountLine.Amount = 0;
        //            newDiscountLine.AmountSpecified = true;
        //            var discountLineDetail = new IntuitData.DiscountLineDetail();

        //            if (invoice.DiscountType == Data.DiscountType.Percentage)
        //            {
        //                discountLineDetail.DiscountPercent = Decimal.Round(invoice.DiscountValue.Value, 1);
        //                discountLineDetail.DiscountPercentSpecified = true;
        //                discountLineDetail.PercentBased = true;
        //                discountLineDetail.PercentBasedSpecified = true;
        //            }
        //            else
        //            {
        //                newDiscountLine.AmountSpecified = true;
        //                newDiscountLine.Amount = invoice.DiscountAmount;
        //                discountLineDetail.PercentBased = false;
        //            }
        //            newDiscountLine.AnyIntuitObject = discountLineDetail;
        //            discountLineDetail.DiscountAccountRef = new IntuitData.ReferenceType()
        //            {
        //                name = "Discounts given",
        //                Value = accountForDiscount.Id
        //            };
        //            //existinginvoice.Line.Append(newDiscountLine);
        //            existinginvoice.Line.Concat(new[] { newDiscountLine });
        //        }
        //        var savedInvoice = IntuitDataProvider.Update(existinginvoice);
        //        if (isMailing == true)
        //            IntuitDataProvider.SendEmail(savedInvoice, customer);
        //    }
        //    catch (Exception e)
        //    {
        //        throw new UserFriendlyException(e.Message);
        //    }
        //}

        private void AddCustomFields(IntuitData.Invoice intuitInvoice, Invoice invoice, IntuitData.Customer customer)
        {
            var consultant = invoice.Consultant;
            var client = invoice.EndClientName;
            var customFields = new List<IntuitData.CustomField>();

            var customerFields = customer.CustomField;

            //Candidate Name
            var candidateCustomField = new IntuitData.CustomField();
            candidateCustomField.Name = "Candidate Name";
            candidateCustomField.Type = IntuitData.CustomFieldTypeEnum.StringType;
            candidateCustomField.AnyIntuitObject = consultant.FirstName + " " + consultant.LastName;
            candidateCustomField.DefinitionId = "1";

            customFields.Add(candidateCustomField);

            var clientCustomField = new IntuitData.CustomField();
            clientCustomField.Name = "End Client Name";
            clientCustomField.Type = IntuitData.CustomFieldTypeEnum.StringType;
            clientCustomField.AnyIntuitObject = client;
            clientCustomField.DefinitionId = "3";
            
            customFields.Add(clientCustomField);

            intuitInvoice.CustomField = customFields.ToArray();
        }


        private void AddLines(IntuitData.Invoice intuitInvoice, Invoice invoice, IntuitData.Account accountForDiscount)
        {
            var lineNum = 1;

            string GetLineNum(bool increment = false)
            {
                if (increment)
                {
                    lineNum++;
                }
                return lineNum.ToString();

            }

            var lineList = new List<IntuitData.Line>();
            var line = new IntuitData.Line();
            line.Id = GetLineNum();
            line.LineNum = GetLineNum();
            line.Description = invoice.Description;
            line.Amount = invoice.ServiceTotal;
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
            lineList.Add(line);


            //Expenses
            for (var i = 0; i < invoice.LineItems.Count(); i++)
            {
                
                var e = invoice.LineItems.ToList()[i];
                var expenseLine = new IntuitData.Line();
                expenseLine.Id = GetLineNum(true);
                expenseLine.LineNum = GetLineNum();
                expenseLine.DetailType = IntuitData.LineDetailTypeEnum.SalesItemLineDetail;
                expenseLine.DetailTypeSpecified = true;
                expenseLine.Amount = e.Amount;
                expenseLine.AmountSpecified = true;
                expenseLine.Description = e.Description;
                expenseLine.AnyIntuitObject = new IntuitData.SalesItemLineDetail()
                {
                    ServiceDate = e.ServiceDt,
                    ServiceDateSpecified = true,


                };
                lineList.Add(expenseLine);

            }

            // Sub Total
            var subTotalLine = new IntuitData.Line();
            subTotalLine.Id = GetLineNum(true);
            subTotalLine.LineNum = GetLineNum();
            subTotalLine.DetailType = IntuitData.LineDetailTypeEnum.SubTotalLineDetail;
            subTotalLine.DetailTypeSpecified = true;
            subTotalLine.Amount = invoice.SubTotal;
            lineList.Add(subTotalLine);



            //Discount
            var discountLine = new IntuitData.Line();
            discountLine.Id = GetLineNum(true);
            discountLine.LineNum = GetLineNum();
            discountLine.DetailType = IntuitData.LineDetailTypeEnum.DiscountLineDetail;
            discountLine.DetailTypeSpecified = true;

            discountLine.Amount = 0;
            discountLine.AmountSpecified = true;
            var discountLineDetail = new IntuitData.DiscountLineDetail();

            if (invoice.DiscountType == Data.DiscountType.Percentage)
            {
                discountLineDetail.DiscountPercent = Decimal.Round(invoice.DiscountValue.Value, 1);
                discountLineDetail.DiscountPercentSpecified = true;
                discountLineDetail.PercentBased = true;
                discountLineDetail.PercentBasedSpecified = true;

            }
            else
            {
                discountLine.AmountSpecified = true;
                discountLine.Amount = invoice.DiscountAmount;
                discountLineDetail.PercentBased = false;

            }


            discountLine.AnyIntuitObject = discountLineDetail;
            discountLineDetail.DiscountAccountRef = new IntuitData.ReferenceType()
            {
                name = "Discounts given",
                Value = accountForDiscount.Id
            };
            lineList.Add(discountLine);


            intuitInvoice.Line = lineList.ToArray();
        }


    }
}
