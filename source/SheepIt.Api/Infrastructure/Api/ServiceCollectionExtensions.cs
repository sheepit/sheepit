using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SheepIt.Api.Infrastructure.Api
{
    public static class ServiceCollectionExtensions
    {
        public static IEnumerable<ServiceDescriptor> WithoutTypes(
            this IServiceCollection services,
            params Type[] excludedTypes)
        {
            return services.Where(serviceDescriptor => !excludedTypes.Contains(serviceDescriptor.ServiceType));
        }

        public static void AddInstance<TService>(this IServiceCollection services, TService service)
        {
            services.Add(CreateInstanceServiceDescriptor(service));
        }
        
        public static void ReplaceWithInstance<TService>(this IServiceCollection services, TService service)
        {
            services.Replace(CreateInstanceServiceDescriptor(service));
        }

        private static ServiceDescriptor CreateInstanceServiceDescriptor<TService>(TService service)
        {
            return new ServiceDescriptor(typeof(TService), service);
        }
    }
}