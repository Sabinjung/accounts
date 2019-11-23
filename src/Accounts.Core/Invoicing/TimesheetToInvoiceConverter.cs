using Accounts.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.Invoicing
{
    // Convert Timesheet to Invoice
    public class TimesheetToInvoiceConverter : ITypeConverter<Timesheet, Invoice>
    {
        public Invoice Convert(Timesheet source, Invoice destination, ResolutionContext context)
        {
            var invoice = new Invoice
            {
                TotalHours = source.TotalHrs,
                ConsultantId = source.Project.ConsultantId,
                CompanyId = source.Project.CompanyId,
                TermId = source.Project.TermId,
                InvoiceDate = DateTime.Now,
                Rate = source.Project.Rate,
            };

            invoice.SubTotal = System.Convert.ToDecimal(invoice.Rate * invoice.TotalHours);
            invoice.Total = invoice.SubTotal;
            invoice.Attachments = source.Attachments;
            invoice.Description = $"Billing Period {source.StartDt.ToShortDateString()} -  {source.EndDt.ToShortDateString()}";

            return invoice;
        }
    }
}
