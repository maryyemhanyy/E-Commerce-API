using E_Commerce.Application.Contracts;
using E_Commerce.Application.Profiles;
using E_Commerce.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application
{
    public static class ApplicationServicesRegisterations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(c => c.AddProfile(new ProductProfile()), typeof(ApplicationServicesRegisterations).Assembly);
            services.AddScoped<IProductServices, ProductServices>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddSingleton<ICacheServices, CacheServices>();
            services.AddScoped<IIdentityServices, IdentityServices>();
            services.AddScoped<IAuthenticationServices, AuthenticationServices>();
            services.AddScoped<ITokenServices, TokenServices>();
            services.AddScoped<IOrderServices, OrderServices>();
            services.AddScoped<IPaymentService, PaymentService>();

            return services;
        }
    }
}
