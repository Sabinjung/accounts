using Abp.Authorization;
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

            // Invoice Permissions
            var invoicePermission = context.CreatePermission("Invoicing", L("Invoicing"));
            invoicePermission.CreateChildPermission("Invoicing.Submit", L("Submit Invoice"));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AccountsConsts.LocalizationSourceName);
        }
    }
}
