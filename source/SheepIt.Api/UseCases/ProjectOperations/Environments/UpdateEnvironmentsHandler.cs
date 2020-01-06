using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Environments;

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
                .InProjectLock()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class UpdateEnvironmentsHandler : IHandler<UpdateEnvironmentsRequest>
    {
        private readonly SheepItDbContext _dbContext;
        private readonly EnvironmentFactory _environmentFactory;

        public UpdateEnvironmentsHandler(
            SheepItDbContext dbContext,
            EnvironmentFactory environmentFactory)
        {
            _dbContext = dbContext;
            _environmentFactory = environmentFactory;
        }
        
        public async Task Handle(UpdateEnvironmentsRequest request)
        {
            var currentEnvironments = await _dbContext.Environments
                .FromProject(request.ProjectId)
                .OrderByRank()
                .ToArrayAsync();

            foreach (var environmentDto in request.Environments)
            {
                if (environmentDto.Id == 0)
                {
                    var newEnvironment = await _environmentFactory.Create(
                        projectId: request.ProjectId,
                        rank: environmentDto.Rank,
                        displayName: environmentDto.DisplayName
                    );

                    _dbContext.Environments.Add(newEnvironment);
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
                }
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}