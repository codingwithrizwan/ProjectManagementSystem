

using Microsoft.OpenApi;
using System.Collections.Generic;

namespace PMS.API.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Project Management Api",
                    Version = "v1",
                    Description = "Api for project management system by Rizwan"
                });

                var bearerScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Paste only JWT token (without 'Bearer ' prefix).",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                };

                opt.AddSecurityDefinition("Bearer", bearerScheme);
                opt.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference("Bearer", doc, null!), new List<string>() }
                });
            });

            return services;

        }


        public static IApplicationBuilder UseSwaggerUI(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                // Serve Swagger UI at application root '/'
                opt.RoutePrefix = string.Empty;
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Project Management Api V1");
                opt.DocumentTitle = "Project Management Api By Rizwan";
            });

            return app;
        }
    }
}
