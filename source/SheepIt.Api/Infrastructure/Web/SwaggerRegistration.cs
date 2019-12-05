//using Microsoft.AspNetCore.Mvc.ApiExplorer;
//using Microsoft.Extensions.DependencyInjection;
//using Newtonsoft.Json;
//using Swashbuckle.AspNetCore.Swagger;
//using Swashbuckle.AspNetCore.SwaggerGen;
//
//namespace SheepIt.Api.Infrastructure.Web
//{
//    public static class SwaggerRegistration
//    {
//        public static void FixSwaggerRegistration(this IServiceCollection services)
//        {
//            // this method fixes Swashbuckle's middleware resolution, which fails because
//            // Autofac cannot choose between multiple constructors of these services
//            
//            services.ReplaceTransient<ISwaggerProvider>(serviceProvider => new SwaggerGenerator(
//                serviceProvider.GetService<IApiDescriptionGroupCollectionProvider>(),
//                serviceProvider.GetService<ISchemaRegistryFactory>(),
//                serviceProvider.GetService<SwaggerGeneratorOptions>()
//            ));
//            
//            services.ReplaceTransient<ISchemaRegistryFactory>(serviceProvider => new SchemaRegistryFactory(
//                serviceProvider.GetService<JsonSerializerSettings>(),
//                serviceProvider.GetService<SchemaRegistryOptions>()
//            ));
//        }
//    }
//}