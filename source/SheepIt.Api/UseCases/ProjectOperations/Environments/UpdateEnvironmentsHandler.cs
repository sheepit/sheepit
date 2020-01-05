using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using SheepIt.Api.Core.Environments.Queries;
using SheepIt.Api.Core.ProjectContext;
using SheepIt.Api.Core.Projects;
using SheepIt.Api.DataAccess.Sequencing;
using SheepIt.Api.Infrastructure.Handlers;
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
        private readonly GetEnvironmentsQuery _getEnvironmentsQuery;
        private readonly IdStorage _idStorage;
        private readonly EnvironmentRepository _environmentRepository;

        public UpdateEnvironmentsHandler(
            GetEnvironmentsQuery getEnvironmentsQuery,
            IdStorage idStorage,
            EnvironmentRepository environmentRepository)
        {
            _getEnvironmentsQuery = getEnvironmentsQuery;
            _idStorage = idStorage;
            _environmentRepository = environmentRepository;
        }
        
        public async Task Handle(UpdateEnvironmentsRequest request)
        {
            var currentEnvironments = await _getEnvironmentsQuery.Get(request.ProjectId); 

            foreach (var environmentDto in request.Environments)
            {
                if (environmentDto.Id == 0)
                {
                    var newEnvironment = new Environment
                    {
                        Id = await _idStorage.GetNext(IdSequence.Environment),
                        ProjectId = request.ProjectId,
                        Rank = environmentDto.Rank,
                        DisplayName = environmentDto.DisplayName
                    };

                    _environmentRepository.Add(newEnvironment);
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

                    _environmentRepository.Update(environmentToUpdate);
                }
            }
            
            await _environmentRepository.Save();
        }
    }
}