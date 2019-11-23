using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Data
{
    public enum InvoiceCycles
    {
        Weekly = 1,
        Monthly = 2,
        BiWeekly = 3,
        BiMonthly = 4
    }


    public enum TimesheetStatuses
    {
        Created = 1,
        Approved = 2,
        Rejected = 3,
        Invoiced = 4,
        InvoiceGenerated = 5

    }
}
