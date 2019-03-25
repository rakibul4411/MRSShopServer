using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MRSShop.AppIdentity.Data;
using MRSShop.AppIdentity.Models;
using MRSShop.Models;

namespace MRSShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            //Shop Data context
            services.AddDbContext<DataContext>(options =>
             options.UseSqlServer(Configuration["Data:Products:ConnectionString"]));

            //Identity Data context
            services.AddDbContext<IdentityDataContext>(options =>
                  options.UseSqlServer(Configuration["Data:Identity:ConnectionString"]));
            services.AddIdentity<AppUser, AppRole>(
                options => options.Stores.MaxLengthForKeys = 128)
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //session and chash configaration
            services.AddDistributedSqlServerCache(options => {
                options.ConnectionString =
                    Configuration["Data:Products:ConnectionString"];
                options.SchemaName = "dbo";
                options.TableName = "SessionData";
            });

            services.AddSession(options => {
                options.Cookie.Name = "MRSShop.Session";
                options.IdleTimeout = System.TimeSpan.FromHours(48);
                options.Cookie.HttpOnly = false;
            });

            //Authentication configuration

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(x =>
             {
                 x.SaveToken = true;
                 x.RequireHttpsMetadata = false;

                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidAudience = "api/Auth/login",
                     ValidIssuer = "api/Auth/login",


                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MySuperSecureKey")),

                 };


             });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IdentityDataContext Identitycontext, DataContext context, UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager, IAntiforgery antiforgery
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCookiePolicy();

            app.UseCors(options => options
                           .AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials());

            app.UseAuthentication();
            app.UseSession();
            //app.Use(nextDelegate => requestContext =>
            //{
            //    if (requestContext.Request.Path.StartsWithSegments("/api")
            //            || requestContext.Request.Path.StartsWithSegments("/"))
            //    {
            //        requestContext.Response.Cookies.Append("XSRF-TOKEN",
            //        antiforgery.GetAndStoreTokens(requestContext).RequestToken);
            //    }
            //    return nextDelegate(requestContext);
            //});
            app.UseMvc();
            Seedvalue.Initialize(Identitycontext, userManager, roleManager).Wait();
        }
    }
}
