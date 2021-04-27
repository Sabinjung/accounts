using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Configs.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Configs
{
    public class ConfigTypeAppService : AsyncCrudAppService<ConfigType, ConfigTypeDto>, IConfigTypeAppService
    {
        public ConfigTypeAppService(IRepository<ConfigType> repository) : base(repository)
        {
        }
    }
}
