﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewItemManager.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Server.Managers.ReviewItemManager
{
    using CDP4Common.CommonData;

    using Microsoft.EntityFrameworkCore;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Types;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="ReviewItem" />s
    /// </summary>
    public class ReviewItemManager : ContainedEntityManager<ReviewItem, Review>, IReviewItemManager
    {
        /// <summary>
        ///     The <see cref="IAnnotationManager" />
        /// </summary>
        private readonly IAnnotationManager annotationManager;

        /// <summary>
        ///     The <see cref="IReviewCategoryManager" />
        /// </summary>
        private readonly IReviewCategoryManager reviewCategoryManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewItemManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="reviewCategoryManager">The <see cref="IReviewCategoryManager" /></param>
        /// <param name="annotationManager">The <see cref="IAnnotationManager" /></param>
        public ReviewItemManager(DatabaseContext context, IReviewCategoryManager reviewCategoryManager, IAnnotationManager annotationManager) : base(context)
        {
            this.reviewCategoryManager = reviewCategoryManager;
            this.annotationManager = annotationManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="ReviewItem" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewItem" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(ReviewItem entity, EntityDto dto)
        {
            if (dto is not ReviewItemDto reviewItemDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntityCollection(await this.reviewCategoryManager.FindEntities(reviewItemDto.ReviewCategories));
            relatedEntities.InsertEntityCollection(await this.annotationManager.FindEntities(reviewItemDto.Annotations));
            entity.ResolveProperties(reviewItemDto, relatedEntities);
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="ReviewItem" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var reviewItem = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer)
                .ThenInclude(x => x.EntityContainer).FirstOrDefaultAsync();

            if (reviewItem == null)
            {
                return null;
            }

            var url = $"Project/{reviewItem.EntityContainer.EntityContainer.Id}/Review/{reviewItem.EntityContainer.Id}/ReviewItem/{reviewItem.Id}";
            
            return new SearchResultDto()
            {
                DisplayText = "Review Item",
                BaseUrl = url,
                ObjectKind = nameof(ReviewItem),
                Location = $"{((Project)reviewItem.EntityContainer.EntityContainer).ProjectName} > {((Review)reviewItem.EntityContainer).Title}"
            };
        }

        /// <summary>
        ///     Gets all <see cref="Entity" /> that needs to be unindexed when the current <see cref="Entity" /> is delete
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the entity</param>
        /// <returns>A collection of <see cref="Entity" /></returns>
        public override async Task<IEnumerable<Entity>> GetExtraEntitiesToUnindex(Guid entityId)
        {
            await Task.CompletedTask;
            return Enumerable.Empty<Entity>();
        }

        /// <summary>
        ///     Gets a <see cref="ReviewItem" /> that targets a <see cref="Thing" />
        /// </summary>
        /// <param name="reviewId">The <see cref="Guid" /> of the container of the <see cref="ReviewItem" /></param>
        /// <param name="thingId">The <see cref="Guid" /> of the <see cref="Thing" /></param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetReviewItemForThing(Guid reviewId, Guid thingId, int deepLevel = 0)
        {
            var reviewItem = await this.EntityDbSet.Where(x => x.ThingId == thingId && x.EntityContainer.Id == reviewId)
                .BuildIncludeEntityQueryable(deepLevel)
                .ToListAsync();

            return reviewItem.SelectMany(x => x.GetAssociatedEntities(deepLevel)).ToList();
        }

        /// <summary>
        ///     Updates a <see cref="ReviewItem" />
        /// </summary>
        /// <param name="entity">The <see cref="ReviewItem" /> to update</param>
        /// <returns>A <see cref="Task" /> with the result of the update</returns>
        public override async Task<EntityOperationResult<ReviewItem>> UpdateEntity(ReviewItem entity)
        {
            if (!this.ValidateCurrentEntity(entity, out var entityOperationResult))
            {
                return entityOperationResult;
            }

            var reviewItem = (await this.GetEntity(entity.Id)).OfType<ReviewItem>().First();
            reviewItem.IsReviewed = entity.IsReviewed;

            return await this.UpdateEntityIntoContext(reviewItem);
        }

        /// <summary>
        ///     Gets all <see cref="ReviewItem" /> that targets <see cref="Thing" />
        /// </summary>
        /// <param name="reviewId">The <see cref="Guid" /> of the container of the <see cref="ReviewItem" /></param>
        /// <param name="thingIds">A collection of <see cref="Guid" /> for <see cref="Thing" />s</param>
        /// <param name="deepLevel">The deepLevel</param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="Entity" /></returns>
        public async Task<IEnumerable<Entity>> GetReviewItemsForThings(Guid reviewId, IEnumerable<Guid> thingIds, int deepLevel = 0)
        {
            var reviewItems = new List<Entity>();

            foreach (var thingId in thingIds)
            {
                reviewItems.AddRange(await this.GetReviewItemForThing(reviewId, thingId, deepLevel));
            }

            return reviewItems.DistinctBy(x => x.Id).ToList();
        }

        /// <summary>
        ///     Link the <see cref="Annotation" /> to all <see cref="ReviewItem" /> that are linked to a <see cref="Thing" /> id
        /// </summary>
        /// <param name="review">The <see cref="Review" /> container</param>
        /// <param name="annotation">The <see cref="Annotation" /></param>
        /// <param name="thingsId">A collection of <see cref="Guid" /> of <see cref="Thing" />s</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityOperationResult{ReviewItem}" /></returns>
        public async Task<EntityOperationResult<ReviewItem>> LinkAnnotationToItems(Review review, Annotation annotation, IEnumerable<Guid> thingsId)
        {
            var reviewItems = this.EntityDbSet.Where(x => x.EntityContainer.Id == review.Id && thingsId.Contains(x.ThingId))
                .ToList();

            var missingIds = thingsId.Where(x => reviewItems.All(ri => ri.ThingId != x));

            var reviewItemsToCreate = missingIds.Select(missingId => new ReviewItem(Guid.NewGuid())
            {
                ThingId = missingId,
                Annotations = { annotation }
            }).ToList();

            review.ReviewItems.AddRange(reviewItemsToCreate);
            var entityEntries = reviewItemsToCreate.Select(reviewItem => this.Context.Add(reviewItem)).ToList();
            reviewItems.ForEach(x => x.Annotations.Add(annotation));
            entityEntries.AddRange(reviewItems.Select(reviewItem => this.Context.Update(reviewItem)).ToList());

            var operationResult = new EntityOperationResult<ReviewItem>(entityEntries, EntityState.Added, EntityState.Modified, EntityState.Unchanged);

            try
            {
                await this.Context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                this.HandleException(exception, operationResult);
            }

            return operationResult;
        }
    }
}
