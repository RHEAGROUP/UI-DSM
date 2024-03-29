﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.ReviewObjectiveService
{
    using System.Text;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    /// <summary>
    ///     The <see cref="ReviewObjectiveService" /> provide capability to manage <see cref="ReviewObjective" />
    /// </summary>
    [Route("Project/{0}/Review/{1}/ReviewObjective")]
    public class ReviewObjectiveService : EntityServiceBase<ReviewObjective, ReviewObjectiveDto>, IReviewObjectiveService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectiveService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public ReviewObjectiveService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Gets all <see cref="ReviewObjective" />s contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">The <see cref="Entity.Id" /> of the <see cref="Review" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewObjective" /></returns>
        public async Task<List<ReviewObjective>> GetReviewsObjectivesOfReview(Guid projectId, Guid reviewId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                return await this.GetEntities(deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets a <see cref="ReviewObjective" /> contained inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <param name="deepLevel">The deep level to get associated entities from the server</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="ReviewObjective" /></returns>
        public async Task<ReviewObjective> GetReviewObjectiveOfReview(Guid projectId, Guid reviewId, Guid reviewObjectiveId, int deepLevel = 0)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);
                return await this.GetEntity(reviewObjectiveId, deepLevel);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="ReviewObjective" /> inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjective">The <see cref="ReviewObjectiveCreationDto" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        public async Task<EntityRequestResponse<ReviewObjective>> CreateReviewObjective(Guid projectId, Guid reviewId, ReviewObjectiveCreationDto reviewObjective)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);

                var content = this.jsonService.Serialize(reviewObjective);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await this.HttpClient.PostAsync(this.CreateUri(Path.Combine(this.MainRoute, "CreateTemplate"), 1), bodyContent);
                var entityRequest =  this.jsonService.Deserialize<EntityRequestResponseDto>(await response.Content.ReadAsStreamAsync());
                return HandleEntityRequestResponse(entityRequest);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="ReviewObjective" /> inside a <see cref="Review" />
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewId">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectives">The <see cref="IEnumerable{ReviewObjectiveCreationDto}" />to create</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        public async Task<EntitiesRequestResponses<ReviewObjective>> CreateReviewObjectives(Guid projectId, Guid reviewId, IEnumerable<ReviewObjectiveCreationDto> reviewObjectives)
        {
            try
            {
                this.ComputeMainRoute(projectId, reviewId);

                var content = this.jsonService.Serialize(reviewObjectives);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await this.HttpClient.PostAsync(this.CreateUri(Path.Combine(this.MainRoute, "CreateTemplates"), 0), bodyContent);
                var entityRequest = this.jsonService.Deserialize<EntityRequestResponseDto>(await response.Content.ReadAsStreamAsync());
                return HandleEntitiesRequestResponse(entityRequest);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Updates a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" />to update</param>
        /// <returns>A <see cref="Task" /> with the <see cref="EntityRequestResponse{ReviewObjective}" /></returns>
        public async Task<EntityRequestResponse<ReviewObjective>> UpdateReviewObjective(Guid projectId, ReviewObjective reviewObjective)
        {
            this.VerifyEntityAndContainer<Review>(reviewObjective);

            try
            {
                this.ComputeMainRoute(projectId, reviewObjective.EntityContainer.Id);
                return await this.UpdateEntity(reviewObjective, 0);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Deletes a <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" /> to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteReviewObjective(Guid projectId, ReviewObjective reviewObjective)
        {
            this.VerifyEntityAndContainer<Review>(reviewObjective);

            try
            {
                this.ComputeMainRoute(projectId, reviewObjective.EntityContainer.Id);
                return await this.DeleteEntity(reviewObjective);
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Gets, all <see cref="ReviewObjectiveCreationDto" />s from the json file
        ///     and filters them based on the given <see cref="ReviewObjectiveKind" />
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /> of the <see cref="Review" /></param>
        /// <param name="reviewId">The <see cref="Entity.Id" /> of the <see cref="Review" /></param>
        /// </summary>
        /// <returns>A <see cref="Task" /> with the <see cref="List{ReviewObjectiveCreationDto}" /></returns>
        public async Task<List<ReviewObjectiveCreationDto>> GetAvailableTemplates(Guid projectId, Guid reviewId)
        {
            this.ComputeMainRoute(projectId, reviewId);
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/GetAvailableTemplates");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            var content = this.jsonService.Deserialize<List<ReviewObjectiveCreationDto>>(await response.Content.ReadAsStreamAsync());
            return content;
        }

        /// <summary>
        ///     Gets, for all <see cref="ReviewObjective" />, the number of open <see cref="ReviewTask" /> and <see cref="Comment" />
        ///     related to the <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="projectId">The <see cref="Guid"/> of the <see cref="Project"/></param>
        /// <param name="reviewId">The <see cref="Guid"/> of the <see cref="Review"/></param>
        /// <returns>A <see cref="Task" /> with a <see cref="Dictionary{Guid, ComputedProjectProperties}" /></returns>
        public async Task<Dictionary<Guid, AdditionalComputedProperties>> GetOpenTasksAndComments(Guid projectId, Guid reviewId)
        {
            this.ComputeMainRoute(projectId, reviewId);
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/OpenTasksAndComments");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            return this.jsonService.Deserialize<Dictionary<Guid, AdditionalComputedProperties>>(await response.Content.ReadAsStreamAsync());
        }
    }
}
