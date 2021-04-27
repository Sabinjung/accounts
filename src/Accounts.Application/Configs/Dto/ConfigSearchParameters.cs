using PQ.Pagination;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Configs.Dto
{
    public class ConfigSearchParameters : NamedQueryParameter
    {
       public string ConfigType { get; set; }
        public string Email { get; set; }
    }
}
