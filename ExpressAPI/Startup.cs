using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExpressAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

      //  This method gets called by the runtime.Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ELPL APPLICATION v1", Version = "v1" });
            });
            var tokenKey = Configuration.GetValue<string>("TokenKey");
            var key = Encoding.ASCII.GetBytes(tokenKey);


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                };
            });

            services.AddSingleton<IJWTAuthenticationManager>(new JWTAuthenticationManager(tokenKey));
        }
        //public void ConfigureServices(IServiceCollection services)
        //{
        //    services.AddControllers();
        //    services.AddSwaggerGen(c =>
        //    {
        //        c.SwaggerDoc("v1", new OpenApiInfo
        //        {
        //            Version = "v1",
        //            Title = "ToDo API",
        //            Description = "A simple example ASP.NET Core Web API",
        //            TermsOfService = new Uri("https://example.com/terms"),
        //            Contact = new OpenApiContact
        //            {
        //                Name = "Shayne Boyer",
        //                Email = string.Empty,
        //                Url = new Uri("https://twitter.com/spboyer"),
        //            },
        //            License = new OpenApiLicense
        //            {
        //                Name = "Use under LICX",
        //                Url = new Uri("https://example.com/license"),
        //            }
        //        });
        //        // Set the comments path for the Swagger JSON and UI.
        //        //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        //        //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        //        //c.IncludeXmlComments(xmlPath);
        //    });
        //}
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}
            if (env.IsDevelopment())
            {
                var SwaggerEndpoint = "/swagger/v1/swagger.json";
                var SwaggerTitle = "ELPL APPLICATION v1";
                var RoutePrfix = string.Empty;
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.RoutePrefix = RoutePrfix;
                    c.SwaggerEndpoint(SwaggerEndpoint, SwaggerTitle);
                });

                //app.UseSwaggerUI(c =>
                ////c.SwaggerEndpoint("/swagger/v1/swagger.json", "ELPL APPLICATION v1"));
                //c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                //c.RoutePrefix = "swagger";
                //);
                //c.RoutePrefix = string.Empty;);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
