using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using SilkierQuartz;
using SilkierQuartz.Authorization;

namespace SilkierQuartzEasyConfig
{
    public static class Extensions
    {
        public static IServiceCollection AddSilkierQuartzEasy(this IServiceCollection services, SilkierQuartzOptions options)
        {
            SilkierQuartzEasy.SilkierQuartzOptions = options;
            services.AddSingleton(options);
            services.AddOptions();
            //services.TryAddSingleton<IAuthorizationHandler, ImprovedSilkierQuartzDefaultAuthorizationHandler>();
            services.TryAddSingleton<IAuthorizationHandler, SilkierQuartzDefaultAuthorizationHandler>();
            return services;
        }

        public static AuthenticationBuilder AddSilkierQuartzEasyCookies(this AuthenticationBuilder builder, SilkierQuartzAuthenticationOptions authenticationOptions)
        {
            if (SilkierQuartzEasy.SilkierQuartzOptions == null)
            {
                throw new NullReferenceException("Must Call AddSilkierQuartzEasy Before AddAuthentication().AddSilkierQuartzEasy()");
            }

            SilkierQuartzEasy.AuthenticationOptions = authenticationOptions;
            builder.Services.AddSingleton(authenticationOptions);

            if (authenticationOptions.AccessRequirement != 0)
            {
                builder.AddCookie(authenticationOptions.AuthScheme, delegate (CookieAuthenticationOptions cfg)
                {
                    cfg.Cookie.Name = "sq_authenticationOptions.AuthScheme";
                    cfg.LoginPath = SilkierQuartzEasy.SilkierQuartzOptions.VirtualPathRoot + "/Authenticate/Login";
                    cfg.AccessDeniedPath = SilkierQuartzEasy.SilkierQuartzOptions.VirtualPathRoot + "/Authenticate/Login";
                    cfg.ExpireTimeSpan = TimeSpan.FromDays(7.0);
                    cfg.SlidingExpiration = true;
                });
            }

            return builder;
        }

        public static AuthorizationBuilder AddSilkierQuartzEasy(this AuthorizationBuilder builder)
        {
            if (SilkierQuartzEasy.AuthenticationOptions == null)
            {
                throw new NullReferenceException("Must Call AddAuthentication().AddSilkierQuartzEasy() Before AddAuthorization().AddSilkierQuartzEasy()");
            }

            builder.AddPolicy("SilkierQuartz", ConfigPolicy);
            return builder;
        }

        public static AuthorizationOptions AddSilkierQuartzEasyPolicy(this AuthorizationOptions options)
        {
            if (SilkierQuartzEasy.AuthenticationOptions == null)
            {
                throw new NullReferenceException("Must Call AddAuthentication().AddSilkierQuartz() Before AddAuthorization().AddSilkierQuartz()");
            }

            options.AddPolicy("SilkierQuartz", ConfigPolicy);
            return options;
        }

        private static void ConfigPolicy(AuthorizationPolicyBuilder policy)
        {
            policy.AddRequirements(new SilkierQuartzDefaultAuthorizationRequirement(SilkierQuartzEasy.AuthenticationOptions!.AccessRequirement));
            policy.AuthenticationSchemes = [SilkierQuartzEasy.AuthenticationOptions.AuthScheme];
        }

        public static void UseSilkierQuartzEasy(this WebApplication app)
        {
            var scheduler = app.Services.GetRequiredService<ISchedulerFactory>().GetScheduler().Result;

            app.UseSilkierQuartz(o =>
            {
                o.Scheduler = scheduler;
            });
        }
    }
}
