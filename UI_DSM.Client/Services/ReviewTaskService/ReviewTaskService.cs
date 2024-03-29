﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewTaskService
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.Assembler;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ReviewTaskService" /> provide capability to manage <see cref="ReviewTask" />
    /// </summary>
    [Route("Project/{0}/Review/{1}/ReviewObjective/{2}/ReviewTask")]
    public class ReviewTaskService : EntityServiceBase<ReviewTask, ReviewTaskDto>, IReviewTaskService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityServiceBase{TEntity, TEntityDto}" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ReviewTaskService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="ReviewTask" />s contained inside a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">
        ///     The <see cref="Entity.Id" /> of the <see cref="Review" /> of the <see cref="ReviewObjective" />
        /// </param>
        /// <param name="reviewObjectiveId">
        ///     The <see cref="Entity.Id" /> of the <see cref="ReviewObjective" /> of
        ///     <see cref="ReviewTask" />s
        /// </param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewTask" /></returns>
        public async Task<List<ReviewTask>> GetTasksOfReviewObjectives(Guid projectId, Guid reviewId, Guid reviewObjectiveId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId, reviewObjectiveId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="ReviewTask" /> contained inside a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="taskReviewId">The <see cref="Guid" /> of the <see cref="ReviewTask" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewTask" /></returns>
        public async Task<ReviewTask> GetTaskOfReviewObjective(Guid projectId, Guid reviewId, Guid reviewObjectiveId, Guid taskReviewId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId, reviewObjectiveId);
                return await this.GetEntity(taskReviewId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewTask">The <see cref="ReviewTask" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        public async Task<EntityRequestResponse<ReviewTask>> UpdateReviewTask(Guid projectId, Guid reviewId, ReviewTask reviewTask)
        {
            this.VerifyEntityAndContainer<ReviewObjective>(reviewTask);

            try
            {
                this.ComputeMainRoute(projectId, reviewId, reviewTask.EntityContainer.Id);
                return await this.UpdateEntity(reviewTask, 1);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewTask">The <see cref="ReviewTask" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteReviewTask(Guid projectId, Guid reviewId, ReviewTask reviewTask)
        {
            this.VerifyEntityAndContainer<ReviewObjective>(reviewTask);

            try
            {
                this.ComputeMainRoute(projectId, reviewId, reviewTask.EntityContainer.Id);
                return await this.DeleteEntity(reviewTask);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets, for all <see cref="ReviewTask" /> inside a <see cref="ReviewObjective" />, the number of
        ///     <see cref="Comment" />
        ///     related to the <see cref="ReviewTask" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <returns>A <see cref="Task" /> with a <see cref="Dictionary{Guid, ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, AdditionalComputedProperties>> GetCommmentsCount(Guid projectId, Guid reviewId, Guid reviewObjectiveId)
        {
            this.ComputeMainRoute(projectId, reviewId, reviewObjectiveId);
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/CommentsCount");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            return this.jsonService.Deserialize<Dictionary<Guid, AdditionalComputedProperties>>(await response.Content.ReadAsStreamAsync());
        }

        /// <summary>
        ///     Gets all <see cref="ReviewTask" /> with related <see cref="Entity" /> and Container that are contained inside a
        ///     <see cref="Review" /> and could access to a <see cref="View" />
        /// </summary>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="view">the <see cref="View" /></param>
        /// <returns>A <see cref="Task" /> with a collection of <see cref="ReviewTask" /></returns>
        public async Task<List<ReviewTask>> GetReviewTasksForView(Guid projectId, Guid reviewId, View view)
        {
            this.ComputeMainRoute(projectId, reviewId, Guid.Empty);
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/View/{view}");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            var dtos = this.jsonService.Deserialize<IEnumerable<EntityDto>>(await response.Content.ReadAsStreamAsync());
            return Assembler.CreateEntities<ReviewTask>(dtos).ToList();
        }
    }
}
