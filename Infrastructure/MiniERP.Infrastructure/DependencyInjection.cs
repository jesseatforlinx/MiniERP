using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniERP.ApplicationLayer.Interfaces;
using MiniERP.ApplicationLayer.Services;
using MiniERP.Infrastructure.Data;
using MiniERP.Infrastructure.Repositories;

namespace MiniERP.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            // 注册DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));

            // 注册Repository
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IQuotationRepository, QuotationRepository>();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // 注册Application Services
            services.AddScoped<IArticleService, ArticleService>();
            services.AddScoped<ICustomerService, CustomerService>();

            return services;
        }
    }
}

