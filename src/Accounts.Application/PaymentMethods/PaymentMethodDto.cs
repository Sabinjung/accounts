using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.PaymentMethods
{
    [AutoMap(typeof(PaymentMethod))]
    public class PaymentMethodDto : EntityDto
    {
        public string Name { get; set; }

        public string ExternalPaymentId { get; set; }
    }
}
