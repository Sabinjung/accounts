using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;

namespace Accounts.Terms
{
    [AutoMap(typeof(InvoiceCycle))]
    public class InvoiceCycleDto : EntityDto
    {
        public string Name { get; set; }
    }
}