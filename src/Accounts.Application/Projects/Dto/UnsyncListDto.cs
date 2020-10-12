using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Projects.Dto
{
    public class UnsyncListDto
    {
        public List<ProjectInfo>AccountUnsyncedProject { get; set; }
        public List<ProjectInfo> IhrmsUnsyncedProject { get; set; }
    }

    public class ProjectInfo
    {
        public int Id { get; set; }
        public string ConsultantName { get; set; }
    }

}
