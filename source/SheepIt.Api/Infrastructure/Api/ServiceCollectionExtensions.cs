using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

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
    }
}