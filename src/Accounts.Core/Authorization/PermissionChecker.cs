using Abp.Authorization;
using Accounts.Authorization.Roles;
using Accounts.Authorization.Users;

namespace Accounts.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
