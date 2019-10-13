using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Mongo;
using SheepIt.Api.Infrastructure.Resolvers;
using Environment = SheepIt.Api.Core.Environments.Environment;

namespace SheepIt.Api.UseCases.ProjectManagement
{
    public class UpdateProjectRequest : IRequest, IProjectRequest
    {
        public string ProjectId { get; set; }

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
            RuleFor(request => request.ProjectId)
                .NotNull();

            RuleForEach(request => request.Environments)
                .NotNull();
            
            // todo: check if they are unique
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
        private readonly SheepItDatabase _database;
        private readonly IdentityProvider _identityProvider;

        public UpdateProjectHandler(
            SheepItDatabase database,
            IdentityProvider identityProvider)
        {
            _database = database;
            _identityProvider = identityProvider;
        }
        
        public async Task Handle(UpdateProjectRequest request)
        {
            await PersistEnvironments(request);
        }

        private async Task PersistEnvironments(UpdateProjectRequest request)
        {
            var currentEnvironments = await _database.Environments
                .Find(filter => filter.FromProject(request.ProjectId))
                .ToListAsync();

            foreach (var environmentDto in request.Environments)
            {
                if (environmentDto.Id == 0)
                {
                    var newEnvironment = new Environment
                    {
                        Id = await _identityProvider.GetNextId("Environment"),
                        ProjectId = request.ProjectId,
                        Rank = environmentDto.Rank,
                        DisplayName = environmentDto.DisplayName
                    };
                    
                    await _database.Environments.InsertOneAsync(newEnvironment);
                }
                else
                {
                    var environmentToUpdate = currentEnvironments.FirstOrDefault(
                        environment => environment.Id == environmentDto.Id
                    );

                    if (environmentToUpdate == null)
                    {
                        throw new InvalidOperationException(
                            $"Environment with id {environmentDto.Id} does not exist in project {request.ProjectId}."
                        );
                    }

                    environmentToUpdate.Rank = environmentDto.Rank;
                    environmentToUpdate.DisplayName = environmentDto.DisplayName;
                    
                    await _database.Environments.ReplaceOneById(environmentToUpdate);
                }
            }
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