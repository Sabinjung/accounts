using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Invoicing.Dto
{
    public class GenerateInvoiceInputDto : EntityDto
    {
        public int TimesheetId { get; set; }
    }
}
