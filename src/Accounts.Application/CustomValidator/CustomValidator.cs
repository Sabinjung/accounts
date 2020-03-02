using Accounts.Projects;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.CustomValidator
{
    public class AttachmentNotNullValidator : AbstractValidator<CreateTimesheetInputDto>
    {
        public AttachmentNotNullValidator()
        {
            RuleFor(x => x.AttachmentIds).NotEmpty();
        }
    }
}