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

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public string ExternalCustomerId { get; set; }
        public string Notes { get; set; }

        public string TermName { get; set; }

    }
}
