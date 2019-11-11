using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Companies
{
    public interface ICompanyAppService : IApplicationService
    {

        string GetCompany();
    }
}
