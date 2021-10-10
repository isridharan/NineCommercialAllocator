using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Service.Mapper;


namespace Service
{
    public static class IServiceCollectionExtension
    {
        public static IServiceCollection AddServiceDependency(this IServiceCollection services)
        {
            // Auto Mapper Configurations
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new BreaksProfile());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            services.AddSingleton(mapper);                       
            services.AddScoped<IAllocationService, AllocationService>();          
            return services;
        }
    }
}
