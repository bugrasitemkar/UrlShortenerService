using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.Abstract;
using Services;
using Services.Abstract;
using Repository.DAL;

namespace ShortUrl.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public static void RegisterDependencies(this IServiceCollection services, IConfiguration Configuration)
        {
            string mongoConnectionString = Configuration.GetConnectionString("MongoConnectionString");
            services.AddTransient<IRepository>(s => new ShortUrlRepository(mongoConnectionString, "Url", "ShortUrl"));
            services.AddTransient<IShortUrlService, ShortUrlServices>();
        }
    }
}
