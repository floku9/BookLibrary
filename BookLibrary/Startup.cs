using BookLibrary.Models;
using BookLibrary.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookLibrary
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
            services.Configure<BooksLibraryDBSettings>(
                Configuration.GetSection("BookLibrarySettings"));

            services.AddSingleton<IBooksLibraryDBSettings>(ser =>
                ser.GetRequiredService<IOptions<BooksLibraryDBSettings>>().Value);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).
                AddJwtBearer(opt =>
                {
                    opt.RequireHttpsMetadata = false;
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = AuthSettings.ISSUER,

                        ValidateAudience = true,
                        ValidAudience = AuthSettings.AUDIENCE,

                        ValidateLifetime = true,

                        IssuerSigningKey = AuthSettings.GetSymmetricSecurityKey(),
                        ValidateIssuerSigningKey = true
                    };
                    opt.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = 401;
                            context.Response.Headers.Append("Content-Type", "application/json; charset=utf-8");
                            await context.Response.WriteAsync(JsonConvert.SerializeObject(
                                new Response()
                                {
                                    Error = new SimpleError() { Message = "Вы не авторизованы" }
                                }));
                        }
                    };
                });
            services.AddHttpContextAccessor();
            services.AddSingleton<BookService>();
            services.AddSingleton<AuthService>();
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
             
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
