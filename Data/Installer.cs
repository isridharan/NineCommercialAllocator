using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddDataDependency(this IServiceCollection services)
        {
            services.AddScoped<IBreakReadRepository, BreakRepository>();
            services.AddScoped<ICommercialReadRepository, CommercialRepository>();
            return services;
        }
    }
}
