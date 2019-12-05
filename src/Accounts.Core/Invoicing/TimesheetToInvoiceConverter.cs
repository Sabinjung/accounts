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
                DueDate = DateTime.Now.AddDays(source.Project.Term.DueDays),
                Rate = source.Project.Rate,
                ProjectId = source.ProjectId,
                Company = source.Project.Company,
                Term = source.Project.Term,
                Consultant = source.Project.Consultant,
                Project = source.Project

            };
            CalculateTotal(source.Project, invoice);
            invoice.Attachments = source.Attachments;
            invoice.Description = $"Billing Period {source.StartDt.ToShortDateString()} -  {source.EndDt.ToShortDateString()}";
            return invoice;
        }

        private void CalculateTotal(Project project, Invoice invoice)
        {
            decimal discount = 0;
            invoice.SubTotal = System.Convert.ToDecimal(invoice.Rate * invoice.TotalHours);
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
            invoice.DiscountAmount = discount;
            invoice.Total = invoice.SubTotal - discount;
        }
    }
}
