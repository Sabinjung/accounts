using Abp.Application.Services;
using Abp.Domain.Repositories;
using Accounts.Projects.Dto;
using Accounts.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Accounts.Blob;
using Abp.ObjectMapping;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Abp.Auditing;
using Abp.Authorization;

namespace Accounts.Projects
{
    public class ProjectAppService : AsyncCrudAppService<Project, ProjectDto>, IProjectAppService
    {
        private readonly IAzureBlobService AzureBlobService;

        private readonly IObjectMapper Mapper;

        private readonly IRepository<Attachment> AttachmentRepository;

        public ProjectAppService(IRepository<Project> repository, IRepository<Attachment> attachmentRepository, IAzureBlobService azureBlobService, IObjectMapper mapper)
       : base(repository)
        {
            AzureBlobService = azureBlobService;
            Mapper = mapper;
            AttachmentRepository = attachmentRepository;
        }

        public async Task UploadAttachment(int projectId, IFormFile file)
        {
            var project = await Repository.GetAsync(projectId);
            if (project != null)
            {
                var fileName = $"{project.Consultant.FirstName}_{project.Consultant.LastName}_{DateTime.Now.ToShortDateString()}";
                var uri = await AzureBlobService.UploadSingleFileAsync(file, fileName);
                project.Attachments.Add(new Attachment
                {
                    ContentType = file.ContentType,
                    Name = fileName,
                    OriginalName = file.FileName,
                    ContentUrl = uri.PrimaryUri.ToString()
                });
            }
        }

        public async Task<IEnumerable<AttachmentDto>> GetAttachments(int projectId)
        {
            var project = await Repository.GetAsync(projectId);
            if (project != null)
            {
                return Mapper.Map<IEnumerable<AttachmentDto>>(project.Attachments);
            }
            return null;
        }


        [HttpDelete]
        public async Task DeleteAttachment(int projectId, int attachmentId)
        {
            await Repository.GetAsync(projectId);
            var attachment = await AttachmentRepository.GetAsync(attachmentId);
            await AttachmentRepository.DeleteAsync(attachment);
        }
    }
}
