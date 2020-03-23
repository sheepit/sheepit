using System.Threading.Tasks;
using SheepIt.Api.DataAccess.Sequencing;

namespace SheepIt.Api.Model.Components
{
    public class ComponentFactory
    {
        private readonly IdStorage _idStorage;

        public ComponentFactory(IdStorage idStorage)
        {
            _idStorage = idStorage;
        }

        public async Task<Component> Create(string projectId, string name)
        {
            return new Component
            {
                Id = await _idStorage.GetNext(IdSequence.Component),
                ProjectId = projectId,
                Name = name
            };
        }
    }
}