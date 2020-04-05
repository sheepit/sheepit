using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SheepIt.Api.DataAccess;
using SheepIt.Api.Infrastructure.Handlers;
using SheepIt.Api.Infrastructure.Resolvers;
using SheepIt.Api.Model.Components;

namespace SheepIt.Api.UseCases.ProjectOperations.Components
{
    public class UpdateComponentsRequest : IRequest
    {
        public string ProjectId { get; set; }
        public List<ComponentDto> Components { get; set; }

        public class ComponentDto
        {
            public int? Id { get; set; }
            public string Name { get; set; }
        }
    }

    public class UpdateComponentsValidator : AbstractValidator<UpdateComponentsRequest>
    {
        public UpdateComponentsValidator()
        {
            RuleFor(request => request.ProjectId)
                .NotNull();

            RuleFor(request => request.Components)
                .NotEmpty()
                .NotNull();
        }
    }

    [Route("api")]
    [ApiController]
    public class UpdateComponentsController : MediatorController
    {
        [HttpPost]
        [Route("project/components/update")]
        public async Task UpdateComponents(UpdateComponentsRequest request)
        {
            await Handle(request);
        }
    }
    
    public class UpdateComponentsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            BuildRegistration.Type<UpdateComponentsHandler>()
                .WithDefaultResponse()
                .RegisterAsHandlerIn(builder);
        }
    }

    public class UpdateComponentsHandler : IHandler<UpdateComponentsRequest>
    {
        private readonly SheepItDbContext _dbContext;
        private readonly ComponentFactory _componentFactory;

        public UpdateComponentsHandler(SheepItDbContext dbContext, ComponentFactory componentFactory)
        {
            _dbContext = dbContext;
            _componentFactory = componentFactory;
        }

        public async Task Handle(UpdateComponentsRequest request)
        {
            var currentComponents = await _dbContext.Components
                .FromProject(request.ProjectId)
                .ToArrayAsync();

            ValidateNoComponentsWereRemoved(
                currentComponents: currentComponents,
                request.Components
            );

            var currentRank = 1;
            
            foreach (var updatedComponent in request.Components)
            {
                if (updatedComponent.Id.HasValue)
                {
                    var currentComponent = currentComponents.Single(
                        component => component.Id == updatedComponent.Id
                    );

                    currentComponent.Rank = currentRank;
                    currentComponent.Name = updatedComponent.Name;
                }
                else
                {
                    var newComponent = await _componentFactory.Create(
                        projectId: request.ProjectId,
                        name: updatedComponent.Name,
                        rank: currentRank
                    );

                    _dbContext.Add(newComponent);
                }

                currentRank++;
            }

            await _dbContext.SaveChangesAsync();
        }

        private static void ValidateNoComponentsWereRemoved(
            IEnumerable<Component> currentComponents,
            IEnumerable<UpdateComponentsRequest.ComponentDto> updatedComponents)
        {
            var updatedComponentIds = updatedComponents
                .Where(component => component.Id != null)
                .Select(component => component.Id.Value)
                .ToHashSet();
            
            var currentComponentIds = currentComponents
                .Select(component => component.Id)
                .ToHashSet();

            if (!updatedComponentIds.SetEquals(currentComponentIds))
            {
                throw new InvalidOperationException("Removing components is not supported.");
            }
        }
    }
}