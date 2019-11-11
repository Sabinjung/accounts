using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Companies.Dto
{
    [AutoMap(typeof(Company))]
    public class CompanyDto : EntityDto
    {
        [Required]
        public string DisplayName { get; set; }

    }
}
