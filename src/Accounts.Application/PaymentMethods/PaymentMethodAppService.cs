using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.PaymentMethods
{
    public class PaymentMethodAppService : AsyncCrudAppService<PaymentMethod, PaymentMethodDto>
    {
        public PaymentMethodAppService(IRepository<PaymentMethod, int> repository) : base(repository)
        {
        }
    }
}
