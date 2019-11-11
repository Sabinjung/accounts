using Abp.Application.Services;
using Accounts.Projects.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Projects
{
    public interface IProjectAppService : IAsyncCrudAppService<ProjectDto>
    {
        Task UploadAttachment(int projectId, IFormFile file);

    }
}
