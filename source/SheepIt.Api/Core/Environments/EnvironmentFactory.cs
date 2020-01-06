using System.Threading.Tasks;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Core.Environments
{
    public class EnvironmentFactory
    {
        private readonly IdStorage _idStorage;

        public EnvironmentFactory(IdStorage idStorage)
        {
            _idStorage = idStorage;
        }

        public async Task<Environment> Create(string projectId, int rank, string displayName)
        {
            return new Environment
            {
                Id = await _idStorage.GetNext(IdSequence.Environment),
                ProjectId = projectId,
                Rank = rank,
                DisplayName = displayName
            };
        }
    }
}