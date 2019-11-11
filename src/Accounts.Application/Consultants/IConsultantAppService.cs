using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Consultants
{
    public interface IConsultantAppService : IApplicationService
    {

        string GetConsultant();
    }
}
