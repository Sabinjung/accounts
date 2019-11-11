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
            var timesheetPermission = context.CreatePermission("Timesheet");
            timesheetPermission.CreateChildPermission("Timesheet.LogHour");
            timesheetPermission.CreateChildPermission("Timesheet.Create");
            timesheetPermission.CreateChildPermission("Timesheet.Approve");
            timesheetPermission.CreateChildPermission("Timesheet.GenerateInvoice");
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AccountsConsts.LocalizationSourceName);
        }
    }
}
