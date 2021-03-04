using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;

namespace Accounts.Terms
{
    [AutoMap(typeof(Term))]
    public class TermDto : EntityDto
    {
        public string Name { get; set; }
        public string ExternalTermId { get; set; }
    }
}