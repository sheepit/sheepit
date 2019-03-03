using SheepIt.Api.Infrastructure.Mongo;

namespace SheepIt.Api.Core.Deployments
{
    public class DeploymentsStorage
    {
        private readonly SheepItDatabase _database;

        public DeploymentsStorage(SheepItDatabase database)
        {
            _database = database;
        }

        public int Add(Deployment deployment)
        {
            var nextId = _database.Deployments.GetNextIdSync();
            
            deployment.Id = nextId;

            _database.Deployments
                .InsertOne(deployment);

            return nextId;
        }

        public void Update(Deployment deployment)
        {
            _database.Deployments
                .ReplaceOneById(deployment);
        }

        public Deployment Get(string projectId, int deploymentId)
        {
            return _database.Deployments
                .FindByProjectAndId(projectId, deploymentId);
        }

        public Deployment[] GetAll(string projectId)
        {
            return _database.Deployments
                .Find(filter => filter.FromProject(projectId))
                .ToArray();
        }
    }
}