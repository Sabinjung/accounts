﻿using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;

namespace Accounts.Authorization
{
    public class AccountsAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(PermissionNames.Pages_Users, L("Users"));
            context.CreatePermission(PermissionNames.Pages_Roles, L("Roles"));
            context.CreatePermission(PermissionNames.Pages_Tenants, L("Tenants"), multiTenancySides: MultiTenancySides.Host);

            // Timesheet Permissions
            var timesheetPermission = context.CreatePermission("Timesheet", L("Timesheet"));
            timesheetPermission.CreateChildPermission("Timesheet.LogHour", L("Hour Logging"));
            timesheetPermission.CreateChildPermission("Timesheet.Create", L("Timesheet Creation"));
            timesheetPermission.CreateChildPermission("Timesheet.Approve", L("Timesheet Approval"));
            timesheetPermission.CreateChildPermission("Timesheet.GenerateInvoice", L("Invoice Generation"));
            timesheetPermission.CreateChildPermission("Timesheet.Delete", L("Timesheet Delete"));

            // Invoice Permissions
            var invoicePermission = context.CreatePermission("Invoicing", L("Invoicing"));
            invoicePermission.CreateChildPermission("Invoicing.Submit", L("Submit Invoice"));
            invoicePermission.CreateChildPermission("Invoicing.SubmitAndMail", L("Submit And Mail"));

            // Consultant Permissions
            var consultantPermission = context.CreatePermission("Consultant", L("Consultant"));
            consultantPermission.CreateChildPermission("Consultant.Create", L("Consultant.Create"));
            consultantPermission.CreateChildPermission("Consultant.Delete", L("Consultant.Delete"));
            consultantPermission.CreateChildPermission("Consultant.Update", L("Consultant.Update"));

            // Company Permissions
            var companyPermission = context.CreatePermission("Company", L("Company"));
            companyPermission.CreateChildPermission("Company.Sync", L("Company.Sync"));

            // Project Permissions
            var projectPermission = context.CreatePermission("Project", L("Project"));
            projectPermission.CreateChildPermission("Project.Create", L("Project.Create"));
            projectPermission.CreateChildPermission("Project.Update", L("Project.Update"));
            projectPermission.CreateChildPermission("Project.Delete", L("Project.Delete"));

            // Expense Permissions
            var expensePermission = context.CreatePermission("Expense", L("Expense"));
            expensePermission.CreateChildPermission("Expense.Create", L("Expense.Create"));
            expensePermission.CreateChildPermission("Expense.Update", L("Expense.Update"));
            expensePermission.CreateChildPermission("Expense.Delete", L("Expense.Delete"));

        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AccountsConsts.LocalizationSourceName);
        }
    }
}
