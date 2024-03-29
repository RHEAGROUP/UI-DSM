﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.DiagrammingConfigurationService
{
    using System.Text;

    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     The <see cref="DiagrammingConfigurationService" /> provide capability to manage diagram layout
    /// </summary>
    [Route("Layout")]
    public class DiagrammingConfigurationService : ServiceBase, IDiagrammingConfigurationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DiagrammingConfigurationService" /> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="ServiceBase.HttpClient" /></param>
        /// <param name="jsonService">The <see cref="IJsonService" /></param>
        public DiagrammingConfigurationService(HttpClient httpClient, IJsonService jsonService) : base(httpClient, jsonService)
        {
        }

        /// <summary>
        ///     Saves <see cref="ReviewTask" /> diagram configuration
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the configuration</param>
        /// <param name="diagram">The <see cref="DiagramDto" /></param>
        /// <returns>A <see cref="Task" /> </returns>
        public async Task<(bool result, List<string> errors)> SaveDiagramLayout(Guid projectId, Guid reviewTaskId, string configurationName, DiagramDto diagram)
        {
            try
            {
                var content = this.jsonService.Serialize(diagram);
                var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");
                var response = await this.HttpClient.PostAsync($"{this.MainRoute}/{projectId}/{reviewTaskId}/{configurationName}/Save", bodyContent);

                if (!response.IsSuccessStatusCode)
                {
                    return (false, this.jsonService.Deserialize<List<string>>(await response.Content.ReadAsStreamAsync()));
                }

                return (response.IsSuccessStatusCode, new List<string>());
            }
            catch (Exception exception)
            {
                throw new HttpRequestException(exception.Message);
            }
        }

        /// <summary>
        ///     Loads diagram configurations name
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <returns>A <see cref="Task" /> with the <see cref="List{T}" /></returns>
        public async Task<List<string>> LoadDiagramLayoutConfigurationNames(Guid projectId, Guid reviewTaskId)
        {
            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/{projectId}/{reviewTaskId}/Load");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }

            var content = this.jsonService.Deserialize<List<string>>(await response.Content.ReadAsStreamAsync());
            return content;
        }

        /// <summary>
        ///     Loads <see cref="ReviewTask" /> diagram configuration
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the selected configuration</param>
        /// <returns>A <see cref="Task" /> with the <see cref="DiagramDto" /></returns>
        public async Task<DiagramDto> LoadDiagramLayoutConfiguration(Guid projectId, Guid reviewTaskId, string configurationName)
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                return null;
            }

            var response = await this.HttpClient.GetAsync($"{this.MainRoute}/{projectId}/{reviewTaskId}/{configurationName}/Load");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = this.jsonService.Deserialize<DiagramDto>(await response.Content.ReadAsStreamAsync());
            return content;
        }

        /// <summary>
        ///     Deletes a diagram configuration
        /// </summary>
        /// <param name="projectId">The <see cref="Entity.Id" /> of the <see cref="Project" /></param>
        /// <param name="reviewTaskId">The <see cref="Entity.Id" /> of the <see cref="ReviewTask" /></param>
        /// <param name="configurationName">The name of the selected configuration</param>
        /// <returns>A <see cref="Task" /> with the result of the deletion</returns>
        public async Task<(bool success, string error)> DeleteDiagramLayoutConfiguration(Guid projectId, Guid reviewTaskId, string configurationName)
        {
            if (string.IsNullOrEmpty(configurationName))
            {
                return (false, "Empty configuration name");
            }

            var response = await this.HttpClient.DeleteAsync($"{this.MainRoute}/{projectId}/{reviewTaskId}/{configurationName}");

            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }

            var content = this.jsonService.Deserialize<string>(await response.Content.ReadAsStreamAsync());
            return (false, content);
        }
    }
}
