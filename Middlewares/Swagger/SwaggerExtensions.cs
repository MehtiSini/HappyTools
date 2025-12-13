using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HappyTools.Middlewares.Swagger
{
    public static class SwaggerExtensions
    {
        public static void UseSwaggerMiddleware(this IApplicationBuilder app , string swaggerTitle)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", swaggerTitle);
                c.RoutePrefix = string.Empty;
            });

        }
    }
}
