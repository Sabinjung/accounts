using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.EndClients.Dto
{
    [AutoMap(typeof(EndClient))]
    public class EndClientDto : EntityDto
    {
        public string ClientName { get; set; }
    }
}
