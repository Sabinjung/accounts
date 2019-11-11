using Abp.Authorization.Users;
using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Accounts.Models
{
    public class Note : FullAuditedEntity
    {
        public int NoteId { get; set; }

        [StringLength(200)]
        public string NoteTitle { get; set; }

        public string NoteText { get; set; }

    }
}
