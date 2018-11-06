using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebHooks.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NJsonSchema;
using NSwag;
using NSwag.AspNetCore;
using NSwag.SwaggerGeneration.Processors.Security;

namespace CustomSender.Sender
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwagger();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                // enable Webhooks
                .AddWebHooks()
                // enable a custom filter provider (a list of filters available)
                .AddWebHookFilterProvider<MyFilterProvider>();

            // Configure sample JWT AuthN (do not use as-is in production!)
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                // WARNING Remove the sample Token Validator
                // You have to use a valid JWT Token in the header
                // e.g. Authorization Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6Imtub20iLCJpYXQiOjE1MTYyMzkwMjJ9.KNMIiurWFsmpMrwAP7QUvBH1ZZxDjxSKXn1FIAKmgpU
                jwt.SecurityTokenValidators.Add(new AlwaysTokenValidator());
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Enable AuthN
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable Swagger UI
            app.UseSwaggerUi3WithApiExplorer(settings =>
            {
                settings.SwaggerRoute = "/swagger/v1/swagger.json";
                settings.GeneratorSettings.DefaultPropertyNameHandling =
                    PropertyNameHandling.CamelCase;

                settings.GeneratorSettings.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT"));

                settings.GeneratorSettings.DocumentProcessors.Add(
                    new SecurityDefinitionAppender("JWT", new SwaggerSecurityScheme
                    {
                        Type = SwaggerSecuritySchemeType.ApiKey,
                        Name = "Authorization",
                        Description = "Copy 'Bearer ' + valid JWT token into field",
                        In = SwaggerSecurityApiKeyLocation.Header
                    }));
            });

            // app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
