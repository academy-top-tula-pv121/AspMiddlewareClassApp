using Microsoft.Extensions.Primitives;

namespace AspMiddlewareClassApp
{
    public class TokenMiddlware
    {
        RequestDelegate next;
        string tokenPattern;
        public TokenMiddlware(RequestDelegate next, string tokenPattern)
        {
            this.next = next;
            this.tokenPattern = tokenPattern;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Query["token"];
            if(token != tokenPattern)
            {
                context.Response.StatusCode = 403;
            }
            else
                await next.Invoke(context);
        }
    }
    public static class TokenExtesions
    {
        public static IApplicationBuilder UseToken(this IApplicationBuilder applicationBuilder,
                                                    string tokenPatten)
        {
            return applicationBuilder.UseMiddleware<TokenMiddlware>(tokenPatten);
        }
    }
    
    public class RoutingMiddlware
    {
        RequestDelegate next;
        public RoutingMiddlware(RequestDelegate next)
        {
            this.next = next;
        }

        public  async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path;
            if(path == "/")
            {
                await context.Response.WriteAsync("Home page");
            }
            else if(path == "/about")
            {
                await context.Response.WriteAsync("About page");
            }
            else
            {
                context.Response.StatusCode = 404;
            }
        }
    }

    public class ErrorHandleMiddlware
    {
        RequestDelegate next;
        public ErrorHandleMiddlware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await next.Invoke(context);
            if (context.Response.StatusCode == 403)
                await context.Response.WriteAsync("Auth error");
            else if(context.Response.StatusCode == 404)
                await context.Response.WriteAsync("Page not found");
            
        }
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            //app.UseMiddleware<TokenMiddlware>("567");
            //app.UseToken("567");
            //app.Run(async (context) => await context.Response.WriteAsync("You enter to site!"));


            //app.UseMiddleware<ErrorHandleMiddlware>();
            //app.UseMiddleware<TokenMiddlware>("123");
            //app.UseMiddleware<RoutingMiddlware>();

            var appEnvName = app.Environment.EnvironmentName;

            Console.WriteLine(appEnvName);
            

            app.Run();
        }
    }
}