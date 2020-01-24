using Abp.Domain.Entities.Auditing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Accounts.Models
{
    public class Consultant : FullAuditedEntity
    {

        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(100)")]
        public string LastName { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(200)")]
        public string Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }

        [NotMapped]
        public string DisplayName => $"{FirstName} {LastName}";
    }
}
