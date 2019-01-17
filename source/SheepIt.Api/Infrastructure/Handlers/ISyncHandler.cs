namespace SheepIt.Api.Infrastructure.Handlers
{
    public interface ISyncHandler<TRequest, TResponse>
    {
        TResponse Handle(TRequest request);
    }
    
    public interface ISyncHandler<TRequest>
    {
        void Handle(TRequest request);
    }
}