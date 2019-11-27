using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Terms
{
    public class TermAppService : AsyncCrudAppService<Term, TermDto>
    {
        public TermAppService(IRepository<Term, int> repository) : base(repository)
        {
        }
    }
}
