using Accounts.HourLogEntries.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    public class UpdateInvoiceInputDto
    {
        public InvoiceDto Invoice { get; set; }
        public IEnumerable<UpdatedHourLogEntries> UpdatedHourLogEntries { get; set; }
    }

    public class UpdatedHourLogEntries
    {
        public double? Hours { get; set; }
        public DateTime Day { get; set; }
        public int? ProjectId { get; set; }
    }
}
