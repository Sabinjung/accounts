using Abp.Application.Services;
using Accounts.Configs.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Configs
{
    public interface IConfigAppService : IAsyncCrudAppService<ConfigDto>
    {
    }
}
