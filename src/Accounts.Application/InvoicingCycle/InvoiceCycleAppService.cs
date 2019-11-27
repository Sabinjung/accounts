using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Terms
{
    public class InvoiceCycleAppService : AsyncCrudAppService<InvoiceCycle, InvoiceCycleDto>
    {
        public InvoiceCycleAppService(IRepository<InvoiceCycle, int> repository) : base(repository)
        {
        }
    }
}
