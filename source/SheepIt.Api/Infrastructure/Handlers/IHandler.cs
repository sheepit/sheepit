using System.Threading.Tasks;

namespace SheepIt.Api.Infrastructure.Handlers
{
    public interface IHandler<in TRequest, TResponse>
    {
        Task<TResponse> Handle(TRequest request);
    }

    public interface IHandler<in TRequest>
    {
        Task Handle(TRequest request);
    }

    public class Nothing
    {
        public static Nothing Value { get; } = new Nothing();

        private Nothing()
        {
        }
    }
}