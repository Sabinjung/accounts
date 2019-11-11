using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Accounts.Blob;
using Accounts.Controllers;
using Accounts.Models;
using Microsoft.AspNetCore.Mvc;

namespace Accounts.Controllers
{
    public class AttachmentController : AccountsControllerBase
    {

        private readonly IAzureBlobService AzureBlobService;


        private readonly IRepository<Attachment> AttachmentRepository;

        public AttachmentController(IRepository<Attachment> attachmentRepository, IAzureBlobService azureBlobService)
        {
            AttachmentRepository = attachmentRepository;
            AzureBlobService = azureBlobService;
        }

        public async Task<IActionResult> Index(int id)
        {
            var attachment = await AttachmentRepository.GetAsync(id);
            if (attachment != null)
            {
                var blob = await AzureBlobService.DownloadFilesAsync(attachment.Name);
                var blobStream = blob.OpenReadAsync().Result;
                return File(blobStream, attachment.ContentType);
            }

            return NotFound();
        }
    }
}