using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace SheepIt.Api.Infrastructure.ProjectContext
{
    public interface IProjectLock
    {
        Task<IDisposable> LockAsync(string projectId);
    }

    // todo: [rt] test... somehow
    public class ProjectLock : IProjectLock
    {
        private readonly ConcurrentDictionary<string, AsyncLock> _projectLocks = new ConcurrentDictionary<string, AsyncLock>();

        public async Task<IDisposable> LockAsync(string projectId)
        {
            var projectLock = _projectLocks.GetOrAdd(projectId, id => new AsyncLock());

            return await projectLock.LockAsync();
        }
    }
}