using Accounts.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accounts.Invoicing
{
    public class TimesheetToInvoiceConverter : ITypeConverter<Timesheet, Invoice>
    {
        public Invoice Convert(Timesheet source, Invoice destination, ResolutionContext context)
        {
            var invoice = new Invoice
            {
                TotalHours = source.HourLogEntries.Sum(x => x.Hours),
                ConsultantId = source.Project.ConsultantId,
                CompanyId = source.Project.CompanyId,
                TermId = source.Project.TermId,
                InvoiceDate = DateTime.Now,
                Rate = source.Project.Rate,

            };

            invoice.Total = System.Convert.ToDecimal(invoice.Rate * invoice.TotalHours);

            return invoice;
        }
    }
}
