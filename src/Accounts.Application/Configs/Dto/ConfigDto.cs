using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Data;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Configs.Dto
{
    [AutoMap(typeof(Config))]
    public class ConfigDto : EntityDto
    {
        public string Data { get; set; }
        public int ConfigTypeId { get; set; }
        public string ConfigTypeName { get; set; }
    }
}
