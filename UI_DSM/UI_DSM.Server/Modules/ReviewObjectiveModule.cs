﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveModule.cs" company="RHEA System S.A.">
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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Server.Managers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="ReviewObjectiveModule" /> is a
    ///     <see cref="ContainedEntityModule{TEntity,TEntityDto, TEntityContainer}" /> to manage
    ///     <see cref="ReviewObjective" /> related to a <see cref="Review" />
    /// </summary>
    [Route("api/Project/{projectId:guid}/Review/{reviewId:guid}/ReviewObjective")]
    public class ReviewObjectiveModule : ContainedEntityModule<ReviewObjective, ReviewObjectiveDto, Review>
    {
        /// <summary>
        ///     The route key for the ProjectId
        /// </summary>
        private const string ProjectKey = "projectId";

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntities(IEntityManager<ReviewObjective> manager, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntities(manager, context, deepLevel);
        }

        /// <summary>
        ///     Get a <see cref="ReviewObjectiveDto" /> based on its <see cref="Guid" /> with all associated entities
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task GetEntity(IEntityManager<ReviewObjective> manager, Guid entityId, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.GetEntity(manager, entityId, context, deepLevel);
        }

        /// <summary>
        ///     Tries to create a new <see cref="ReviewObjective" /> based on its <see cref="ReviewObjectiveDto" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="dto">The <see cref="ReviewObjectiveDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" /></returns>
        [Authorize]
        public override async Task CreateEntity(IEntityManager<ReviewObjective> manager, ReviewObjectiveDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            dto.Author = participant.Id;
            await base.CreateEntity(manager, dto, context, deepLevel);
        }

        /// <summary>
        ///     Tries to delete an <see cref="ReviewObjective" /> defined by the given <see cref="Guid" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /> to delete</param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /> as result</returns>
        [Authorize]
        public override async Task<RequestResponseDto> DeleteEntity(IEntityManager<ReviewObjective> manager, Guid entityId, HttpContext context)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return new RequestResponseDto();
            }

            return await base.DeleteEntity(manager, entityId, context);
        }

        /// <summary>
        ///     Tries to update an existing <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="manager">The <see cref="IEntityManager{TEntity}" /></param>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="dto">The <see cref="ReviewObjectiveDto" /></param>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <param name="deepLevel">An optional parameters for the deep level</param>
        /// <returns>A <see cref="Task" />as result</returns>
        [Authorize]
        public override async Task UpdateEntity(IEntityManager<ReviewObjective> manager, Guid entityId, ReviewObjectiveDto dto, HttpContext context, int deepLevel = 0)
        {
            var participant = await this.GetParticipantBasedOnRequest(context, ProjectKey);

            if (participant == null)
            {
                return;
            }

            await base.UpdateEntity(manager, entityId, dto, context, deepLevel);
        }

        /// <summary>
        ///     Adds the <see cref="ReviewObjective" /> into the corresponding collection of the <see cref="Review" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewObjective" /></param>
        /// <param name="container">The <see cref="Review" /></param>
        protected override void AddEntityIntoContainerCollection(ReviewObjective entity, Review container)
        {
            container.ReviewObjectives.Add(entity);
        }

        /// <summary>
        ///     Verifies if the provided routes is correctly formatted with all containment
        /// </summary>
        /// <param name="context">The <see cref="HttpContext" /></param>
        /// <returns>A <see cref="Task" /> with the result of the check</returns>
        protected override async Task<bool> AdditionalRouteValidation(HttpContext context)
        {
            var reviewManager = context.RequestServices.GetService<IContainedEntityManager<Review>>();

            if (reviewManager == null)
            {
                context.Response.StatusCode = 500;
                return false;
            }

            var reviewId = this.GetAdditionalRouteId(context.Request, this.ContainerRouteKey);
            var projectId = this.GetAdditionalRouteId(context.Request, ProjectKey);

            if (!await reviewManager.EntityIsContainedBy(reviewId, projectId))
            {
                context.Response.StatusCode = 400;
                return false;
            }

            return true;
        }
    }
}