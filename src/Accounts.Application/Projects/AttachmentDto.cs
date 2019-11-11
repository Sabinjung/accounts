using Abp.AutoMapper;
using Accounts.Models;

namespace Accounts.Projects
{

    [AutoMap(typeof(Attachment))]
    public class AttachmentDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string Name { get; set; }

        public string OriginalName { get; set; }

        public string ContentType { get; set; }

    }
}