using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Core.Notify
{
    public class NotifyParam
    {
        public List<string> EmailAddress { get; set; }
        public string Message { get; set; }
        public int ImType { get; set; }
    }
}
