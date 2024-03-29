﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskModule.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Modules
{
    using System.Diagnostics.CodeAnalysis;

    using Carter.Response;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReviewTaskModule" /> is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto,TEntityContainer}" /> to manage
    ///     <see cref="ReviewTask" /> related to a <see cref="ReviewObjective" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Review/{reviewId:guid}/ReviewObjective/{reviewObjectiveId:guid}/ReviewTask")]
    public class ReviewTaskModule : ContainedEntityModule<ReviewTask, ReviewTaskDto, ReviewObjective>
    {
        /// <summary>
        ///     The route key for the ProjectId
        /// </summary>
        private const string ProjectKey = "projectId";

        /// <summary>
        ///     The route key for the Review Id
        /// </summary>
        private const string ReviewKey = "reviewId";

        /// <summary>
        ///     Adds routes to the <see cref="IEndpointRouteBuilder" />
        /// </summary>
        /// <param name="app">The <see cref="IEndpointRouteBuilder" /></param>
        [ExcludeFromCodeCoverage]
        public override void AddRoutes(IEndpointRouteBuilder app)
        {
            base.AddRoutes(app);

            app.MapGet($"{this.MainRoute}/CommentsCount", this.GetCommentsCount)
                .Produces<Dictionary<Guid, AdditionalComputedProperties>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetCommentsCount");

            app.MapGet(this.MainRoute+"/View/{view}", this.GetReviewTasksForView)
                .Produces<IEnumerable<EntityDto>>()
                .WithTags(this.EntityName)
                .WithName($"{this.EntityName}/GetReviewTasksForView");
        }

        /// <summary>
        ///     Gets all <see cref="ReviewTask" /> with related <see cref="Entity" /> and Container that are contained inside a
        ///     <see cref="Review" /> and could access to a <see cref="View" />
        /// </summary>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager"/></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewObjectiveId">The <see cref="ReviewObjective"/> id, not used.</param>
        /// <param name="view">the <see cref="View" /></param>
        /// <param name="context">The <see cref="HttpContext"/></param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public async Task GetReviewTasksForView(IReviewTaskManager reviewTaskManager, Guid projectId, Guid reviewId, Guid reviewObjectiveId, string view, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            if (!Enum.TryParse(typeof(View), view, out var viewParsed))
            {
                context.Response.StatusCode = 300;
            }
            else
            {
                var entities = await reviewTaskManager.GetReviewTasksForView(projectId, reviewId, (View)viewParsed!);
                await context.Response.Negotiate(entities.ToDtos());
            }
        }

        /// <summary>
        ///     Gets, for all <see cref="ReviewTask" /> inside a <see cref="ReviewObjective" />, the number of
        ///     <see cref="Comment" />
        ///     related to the <see cref="ReviewTask" />
        /// </summary>
        /// <param name="reviewTaskManager">The <see cref="IReviewTaskManager" /></param>
        /// <param name="reviewObjectiveId"> The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task GetCommentsCount(IReviewTaskManager reviewTaskManager, Guid reviewObjectiveId, HttpContext context)
        {
            var reviewTasksId = (await reviewTaskManager.GetContainedEntities(reviewObjectiveId)).Select(x => x.Id);

            var computedProperties = await reviewTaskManager.GetCommentsCount(reviewTasksId);
            await context.Response.Negotiate(computedProperties);
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<ReviewTask> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ReviewTaskDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<ReviewTask> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewTask" /> based on its <see cref="ReviewTaskDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReviewTaskDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override Task CreateEntity(IEntityManager<ReviewTask> manager, ReviewTaskDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            context.Response.StatusCode = 405;
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Tries to delete an <see cref="ReviewTask" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /> to delete</param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<ReviewTask> manager, Guid entityId, ISearchService searchService, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            return await base.DeleteEntity(manager, entityId, searchService, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="ReviewTask" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewTask" /></param>
        /// <param name="dto">The <see cref="ReviewTaskDto" /></param>
        /// <param name="searchService">The <see cref="ISearchService" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<ReviewTask> manager, Guid entityId, ReviewTaskDto dto, ISearchService searchService, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            var objectiveManager = context.RequestServices.GetService<IReviewObjectiveManager>();
            ((IReviewTaskManager)manager).InjectManager(objectiveManager);

            await base.UpdateEntity(manager, entityId, dto, searchService, context, deepLevel);
        }

        /// <summary>
        ///     Adds the <see cref="ReviewTask" /> into the corresponding collection of the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewTask" /></param>
        /// <param name="container">The <see cref="ReviewObjective" /></param>
        protected override void AddEntityIntoContainerCollection(ReviewTask entity, ReviewObjective container)
        {
            container.ReviewTasks.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            var reviewObjectiveManager = context.RequestServices.GetService<IContainedEntityManager<ReviewObjective>>();

            if (reviewObjectiveManager == null)
            {
                context.Response.StatusCode = 500;
                return false;
            }

            var reviewManager = context.RequestServices.GetService<IContainedEntityManager<Review>>();

            if (reviewManager == null)
            {
                context.Response.StatusCode = 500;
                return false;
            }

            var reviewObjectiveId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var reviewId = this.GetAdditionalRouteId(context.Request, ReviewKey);
            var projectId = this.GetAdditionalRouteId(context.Request, ProjectKey);

            if (!await reviewObjectiveManager.EntityIsContainedBy(reviewObjectiveId, reviewId)
                || !await reviewManager.EntityIsContainedBy(reviewId, projectId))
            {
                context.Response.StatusCode = 400;
                return false;
            }

            return true;
        }
    }
}
