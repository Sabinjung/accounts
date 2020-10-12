using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Projects.Dto
{
    [AutoMap(typeof(Project))]
    public class IhrmsProjectDto:EntityDto
    {
        public string  CompanyName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
