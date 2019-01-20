using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public class MediatorController : Controller
    {
        protected async Task<TResponse> Handle<TResponse>(IRequest<TResponse> request)
        {
            var mediator = HttpContext.RequestServices.GetService<HandlerMediator>();

            return await mediator.Handle(request);
        }
    }
}