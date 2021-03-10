using Accounts.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

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
                DueDate = DateTime.Now.AddDays(source.Project.Term.DueDays),
                Rate = source.Project.Rate,
                ProjectId = source.ProjectId,
                Company = source.Project.Company,
                Term = source.Project.Term,
                Consultant = source.Project.Consultant,
                Project = source.Project,
                IsInvoiceEdited =false,
                EndClientName = source.Project.EndClientId != null ? source.Project.EndClient.ClientName : null
            };
            invoice.LineItems = context.Mapper.Map<IList<LineItem>>(source.Expenses);
            CalculateTotal(source.Project, invoice);
            invoice.Attachments = source.Attachments;
            invoice.Description = $"Billing Period {source.StartDt.ToShortDateString()} -  {source.EndDt.ToShortDateString()}";
            return invoice;
        }


        private void CalculateTotal(Project project, Invoice invoice)
        {
            decimal discount = 0;

            invoice.ServiceTotal = System.Convert.ToDecimal(invoice.Rate * invoice.TotalHours);
            invoice.SubTotal = invoice.ServiceTotal + invoice.LineItems.Sum(x => x.Amount);
            switch (project.DiscountType)
            {
                case Data.DiscountType.Percentage:

                    if (project.DiscountValue.HasValue)
                    {
                        discount = (project.DiscountValue.Value / 100) * invoice.SubTotal;
                    }
                    break;
                case Data.DiscountType.Value:
                    if (project.DiscountValue.HasValue)
                    {
                        discount = project.DiscountValue.Value;
                    }
                    break;
            }

            invoice.DiscountType = project.DiscountType;
            invoice.DiscountValue = project.DiscountValue;
            invoice.DiscountAmount = Math.Round(discount, 2);
            invoice.Total = Math.Round(invoice.SubTotal - discount, 2);
            invoice.Balance = Math.Round(invoice.SubTotal - discount, 2);
        }
    }
}
