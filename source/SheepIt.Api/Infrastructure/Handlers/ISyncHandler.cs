namespace SheepIt.Api.Infrastructure.Handlers
{
    public interface ISyncHandler<in TRequest, out TResponse>
    {
        TResponse Handle(TRequest request);
    }
    
    public interface ISyncHandler<in TRequest>
    {
        void Handle(TRequest request);
    }
}