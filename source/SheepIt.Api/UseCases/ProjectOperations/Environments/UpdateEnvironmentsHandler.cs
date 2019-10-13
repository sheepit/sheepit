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

namespace SheepIt.Api.UseCases.ProjectOperations.Environments
{
    public class UpdateEnvironmentsRequest : IRequest, IProjectRequest
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

    public class UpdateEnvironmentsRequestValidator : AbstractValidator<UpdateEnvironmentsRequest>
    {
        public UpdateEnvironmentsRequestValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull();

            RuleForEach(request => request.Environments)
                .NotNull();
        }
    }

    [Route("api")]
    [ApiController]
    public class UpdateEnvironmentsController : MediatorController
    {
        [HttpPost]
        [Route("project/environment/update-environments")]
        public async Task UpdateProject(UpdateEnvironmentsRequest request)
        {
            await Handle(request);
        }
    }

    public class UpdateEnvironmentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateEnvironmentsHandler>()
                .WithDefaultResponse()
                .InProjectContext()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class UpdateEnvironmentsHandler : IHandler<UpdateEnvironmentsRequest>
    {
        private readonly SheepItDatabase _database;
        private readonly IdentityProvider _identityProvider;

        public UpdateEnvironmentsHandler(
            SheepItDatabase database,
            IdentityProvider identityProvider)
        {
            _database = database;
            _identityProvider = identityProvider;
        }
        
        public async Task Handle(UpdateEnvironmentsRequest request)
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
}