using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;

namespace Accounts.Invoicing.Dto
{
    [AutoMap(typeof(Invoice))]
    public class InvoiceDto : EntityDto
    {
    }
}