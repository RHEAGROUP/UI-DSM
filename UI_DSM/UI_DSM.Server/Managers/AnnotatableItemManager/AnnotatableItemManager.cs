﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="AnnotatableItemManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.AnnotatableItemManager
{
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="AnnotatableItem" />s
    /// </summary>
    public class AnnotatableItemManager : IAnnotatableItemManager
    {
        /// <summary>
        ///     The <see cref="IReviewObjectiveManager" />
        /// </summary>
        private readonly IReviewObjectiveManager reviewObjectiveManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AnnotatableItemManager" /> class.
        /// </summary>
        /// <param name="reviewObjectiveManager">The <see cref="IReviewObjectiveManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager"/></param>
        public AnnotatableItemManager(IReviewObjectiveManager reviewObjectiveManager, IAnnotationManager annotationManager)
        {
            this.reviewObjectiveManager = reviewObjectiveManager;
            annotationManager.InjectManager(this);
        }

        /// <summary>
        ///     Gets a collection of all <see cref="Entity" />s and associated <see cref="Entity" />
        /// </summary>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> as result</returns>
        public async Task<IEnumerable<Entity>> GetEntities(int deepLevel = 0)
        {
            var annotatableItems = new List<Entity>();
            annotatableItems.AddRange(await this.reviewObjectiveManager.GetEntities(deepLevel));
            return annotatableItems.DistinctBy(x => x.Id);
        }

        /// <summary>
        ///     Tries to get a <see cref="Entity" /> based on its <see cref="Guid" /> and its associated <see cref="Entity" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <param name="deepLevel">The level of deepnest that we need to retrieve associated <see cref="Entity" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /> if found</returns>
        public async Task<IEnumerable<Entity>> GetEntity(Guid entityId, int deepLevel = 0)
        {
            var annotatableItem = await this.FindEntity(entityId);

            return annotatableItem == null ? Enumerable.Empty<Entity>() : annotatableItem.GetAssociatedEntities(deepLevel);
        }

        /// <summary>
        ///     Tries to get a <see cref="AnnotatableItem" /> based on its <see cref="Guid" />
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="AnnotatableItem" /> if found</returns>
        public async Task<AnnotatableItem> FindEntity(Guid entityId)
        {
            return await this.reviewObjectiveManager.FindEntity(entityId);
        }

        /// <summary>
        ///     Tries to get all <see cref="AnnotatableItem" /> based on their <see cref="Guid" />
        /// </summary>
        /// <param name="entitiesId">A collection of <see cref="Guid" /></param>
        /// <returns>A collection of <see cref="AnnotatableItem" /></returns>
        public async Task<IEnumerable<AnnotatableItem>> FindEntities(IEnumerable<Guid> entitiesId)
        {
            var entities = new List<AnnotatableItem>();

            foreach (var id in entitiesId)
            {
                var entity = await this.FindEntity(id);

                if (entity != null)
                {
                    entities.Add(entity);
                }
            }

            return entities;
        }

        /// <summary>
        ///     Creates a new <see cref="AnnotatableItem" /> and store it into the <see cref="DatabaseContext" />
        /// </summary>
        /// <param name="entity">The <see cref="AnnotatableItem" /> to create</param>
        /// <returns>A <see cref="Task" /> with the result of the creation</returns>
        public Task<EntityOperationResult<AnnotatableItem>> CreateEntity(AnnotatableItem entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Updates a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="entity">The <see cref="AnnotatableItem" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public Task<EntityOperationResult<AnnotatableItem>> UpdateEntity(AnnotatableItem entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Deletes a <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="entity">The <see cref="AnnotatableItem" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public Task<EntityOperationResult<AnnotatableItem>> DeleteEntity(AnnotatableItem entity)
        {
            return HandleNotSupportedOperation();
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="AnnotatableItem" />
        /// </summary>
        /// <param name="entity">The <see cref="AnnotatableItem" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public Task ResolveProperties(AnnotatableItem entity, EntityDto dto)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Provides the correct returns values on unsupported operation
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private static async Task<EntityOperationResult<AnnotatableItem>> HandleNotSupportedOperation()
        {
            await Task.CompletedTask;
            return EntityOperationResult<AnnotatableItem>.Failed("Operation not supported");
        }
    }
}