namespace SheepIt.Api.Infrastructure.Handlers
{
    public interface IRequest<TResponse>
    {
    }

    public interface IRequest : IRequest<Nothing>
    {
    }
}