﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="NoteManager.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Managers.NoteManager
{
    using Microsoft.EntityFrameworkCore;
    using UI_DSM.Server.Context;
    using UI_DSM.Server.Extensions;
    using UI_DSM.Server.Managers.AnnotatableItemManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This manager handles operation to the Database for <see cref="Note" />s
    /// </summary>
    public class NoteManager : ContainedEntityManager<Note, Project>, INoteManager
    {
        /// <summary>
        ///     The <see cref="IParticipantManager" />
        /// </summary>
        private readonly IParticipantManager participantManager;

        /// <summary>
        ///     The <see cref="IAnnotatableItemManager" />
        /// </summary>
        private IAnnotatableItemManager annotatableItemManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NoteManager" /> class.
        /// </summary>
        /// <param name="context">The <see cref="DatabaseContext" /></param>
        /// <param name="participantManager">The <see cref="IParticipantManager" /></param>
        public NoteManager(DatabaseContext context, IParticipantManager participantManager) : base(context)
        {
            this.participantManager = participantManager;
        }

        /// <summary>
        ///     Resolve all properties for the <see cref="Note" />
        /// </summary>
        /// <param name="entity">The <see cref="Note" /></param>
        /// <param name="dto">The <see cref="EntityDto" /></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task ResolveProperties(Note entity, EntityDto dto)
        {
            if (dto is not NoteDto noteDto)
            {
                return;
            }

            var relatedEntities = new Dictionary<Guid, Entity>();
            relatedEntities.InsertEntity(await this.participantManager.FindEntity(noteDto.Author));
            relatedEntities.InsertEntityCollection(await this.annotatableItemManager.FindEntities(noteDto.AnnotatableItems));

            entity.ResolveProperties(noteDto, relatedEntities);
        }

        /// <summary>
        ///     Injects a <see cref="IAnnotatableItemManager" /> to break the circular dependency
        /// </summary>
        /// <param name="manager">The <see cref="IAnnotatableItemManager" /></param>
        public void InjectManager(IAnnotatableItemManager manager)
        {
            this.annotatableItemManager = manager;
        }

        /// <summary>
        ///     Sets specific properties before the creation of the <see cref="Note" />
        /// </summary>
        /// <param name="entity">The <see cref="Note" /></param>
        protected override void SetSpecificPropertiesBeforeCreate(Note entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
        }

        /// <summary>
        ///     Gets the <see cref="SearchResultDto"/> based on a <see cref="Guid"/>
        /// </summary>
        /// <param name="entityId">The <see cref="Guid" /> of the <see cref="Note" /></param>
        /// <returns>A URL</returns>
        public override async Task<SearchResultDto> GetSearchResult(Guid entityId)
        {
            var note = await this.EntityDbSet.Where(x => x.Id == entityId)
                .Include(x => x.EntityContainer).FirstOrDefaultAsync();

            if (note == null)
            {
                return null;
            }

            return new SearchResultDto()
            {
                ObjectKind = nameof(Note),
                BaseUrl = $"Project/{note.EntityContainer.Id}/Note/{note.Id}",
                DisplayText = note.Content,
                Location = ((Project)note.EntityContainer).ProjectName
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
    }
}
