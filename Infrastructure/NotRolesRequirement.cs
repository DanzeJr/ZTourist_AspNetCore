using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ZTourist.Infrastructure
{
    public class NotRolesRequirement : IAuthorizationRequirement
    {
        public string[] NotAllowedRoles { get; set; }

        public NotRolesRequirement(params string[] notAllowedRoles)
        {
            NotAllowedRoles = notAllowedRoles;
        }
    }

    public class NotRolesHandler : AuthorizationHandler<NotRolesRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NotRolesRequirement requirement)
        {
            if (!context.User.Identity.IsAuthenticated || !requirement.NotAllowedRoles.Any(role => context.User.IsInRole(role)))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
            return Task.CompletedTask;
        }
    }
}
