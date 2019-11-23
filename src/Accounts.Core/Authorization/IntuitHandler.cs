using Abp.Configuration;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Authorization
{
    public class IntuitHandler :
              AuthorizationHandler<IntuitRequirement>
    {
        private readonly ISettingManager SettingManager;

        public IntuitHandler(ISettingManager settingManager)
        {
            SettingManager = settingManager;
        }
        protected override Task HandleRequirementAsync(
          AuthorizationHandlerContext context,
          IntuitRequirement requirement)
        {
            // Save User object to access claims
            var user = context.User;


            context.Fail();

            return Task.CompletedTask;
        }
    }
}
