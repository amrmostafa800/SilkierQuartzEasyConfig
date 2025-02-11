using Microsoft.AspNetCore.Authorization;
using SilkierQuartz;
using SilkierQuartz.Authorization;

namespace SilkierQuartzEasyConfig
{
    [Obsolete("Use Normal Class (Base Class Here)")]
    internal class ImprovedSilkierQuartzDefaultAuthorizationHandler : SilkierQuartzDefaultAuthorizationHandler
    {
        private readonly SilkierQuartzOptions silkierQuartzOptions = SilkierQuartzEasy.SilkierQuartzOptions ?? throw new Exception("Error In ImprovedSilkierQuartzDefaultAuthorizationHandler");
        public ImprovedSilkierQuartzDefaultAuthorizationHandler(SilkierQuartzOptions silkierQuartzOptions) : base(SilkierQuartzEasy.AuthenticationOptions)
        {
            this.silkierQuartzOptions = silkierQuartzOptions ?? throw new ArgumentNullException("silkierQuartzOptions");
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SilkierQuartzDefaultAuthorizationRequirement requirement)
        {
            if (context.Resource is Microsoft.AspNetCore.Mvc.ActionContext)
            {
                if (!((Microsoft.AspNetCore.Mvc.ActionContext)context.Resource!).HttpContext.Request.Path.StartsWithSegments(silkierQuartzOptions.VirtualPathRoot))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            return base.HandleRequirementAsync(context, requirement);
        }
    }
}
