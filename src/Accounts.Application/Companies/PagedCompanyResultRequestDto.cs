using Abp.Application.Services.Dto;

namespace Accounts.Companies
{
    public class PagedCompanyResultRequestDto: PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}