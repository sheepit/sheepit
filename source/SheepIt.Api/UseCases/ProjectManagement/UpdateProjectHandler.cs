using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.Environments;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class UpdateProjectRequest : IRequest, IProjectRequest
    {
        public string ProjectId { get; set; }
        public string RepositoryUrl { get; set; }

        public EnvironmentDto[] Environments { get; set; }

        public class EnvironmentDto
        {
            public int Id { get; set; }
            public string DisplayName { get; set; }
            public int Rank { get; set; }
        }
    }

    public class UpdateProjectRequestValidator : AbstractValidator<UpdateProjectRequest>
    {
        public UpdateProjectRequestValidator()
        {
            RuleFor(x => x.ProjectId)
                .NotNull()
                .MinimumLength(3);

            RuleFor(x => x.RepositoryUrl)
                .NotNull();

            RuleForEach(x => x.Environments)
                .NotNull();
        }
    }

    [Route("api")]
    [ApiController]
    public class UpdateProjectController : MediatorController
    {
        [HttpPost]
        [Route("update-project")]
        public async Task UpdateProject(UpdateProjectRequest request)
        {
            await Handle(request);
        }
    }

    public class UpdateProjectHandler : IHandler<UpdateProjectRequest>
    {
        private readonly IProjectContext _projectContext;
        private readonly SheepItDatabase _database;

        public UpdateProjectHandler(
            IProjectContext projectContext,
            SheepItDatabase database)
        {
            _projectContext = projectContext;
            _database = database;
        }
        
        public async Task Handle(UpdateProjectRequest request)
        {
            _projectContext.Project.UpdateRepositoryUrl(request.RepositoryUrl);
            
            await _database.Projects
                .ReplaceOneById(_projectContext.Project);

            await PersistEnvironments(request);
        }

        private async Task PersistEnvironments(UpdateProjectRequest request)
        {
            var persistedEnvironments = await _database.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .ToListAsync();

            var environmentFromRequest = request.Environments;

            var toAdd = new List<Environment>();
            
            foreach (var environmentDto in environmentFromRequest)
            {
                var env = Map(environmentDto, request.ProjectId);

                if (environmentDto.Id == 0)
                {
                    env.Id = await _database.GetNextSequence("EnvironmentId");
                    toAdd.Add(env);
                }
                else if(persistedEnvironments.Any(x => x.Id == environmentDto.Id))
                {
                    await _database.Environments.ReplaceOneById(env);
                }
            }

            await _database.Environments.InsertManyAsync(toAdd);
        }

        private Environment Map(UpdateProjectRequest.EnvironmentDto dto, string projectId)
        {
            return new Environment
            {
                Id = dto.Id,
                Rank = dto.Rank,
                DisplayName = dto.DisplayName,
                ProjectId = projectId
            };
        }
    }
    
    public class UpdateProjectModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateProjectHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }
}