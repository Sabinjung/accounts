using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;

namespace Accounts.Configs.Dto
{
    [AutoMap(typeof(ConfigType))]

    public class ConfigTypeDto: EntityDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}