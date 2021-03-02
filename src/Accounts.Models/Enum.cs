using System;
using System.Collections.Generic;
using System.Text;

namespace Accounts.Data
{
    public enum TimesheetStatuses
    {
        Created = 1,
        Approved = 2,
        Rejected = 3,
        Invoiced = 4,
        InvoiceGenerated = 5,
        TimeSheetOpen = 6,
        TimeSheetSubmitted = 7

    }


    public enum InvoiceCycles
    {
        Weekly = 1,
        Monthly = 2,
        BiWeekly = 3,
        BiMonthly = 4
    }

    public enum ConfigTypes
    {
        DefaultEmail = 1,
        NotificationEmail = 2,
        BaseUrl = 3
    }
}
