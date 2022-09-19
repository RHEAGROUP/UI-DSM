﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="UserService.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Services.Administration.UserService
{
    using System.Text;

    using Microsoft.AspNetCore.Components;

    using Newtonsoft.Json;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.DTO.UserManagement;

    using JsonSerializer = System.Text.Json.JsonSerializer;

    /// <summary>
    ///     The <see cref="UserService" /> provide capability to manage users.
    /// </summary>
    [Route("User")]
    public class UserService : ServiceBase, IUserService
    {
        /// <summary>Initializes a new instance of the <see cref="UserService" /> class.</summary>
        /// <param name="httpClient">The <see cref="HttpClient" /></param>
        public UserService(HttpClient httpClient) : base(httpClient)
        {
        }

        /// <summary>
        ///     Provide a collection of registered <see cref="UserEntityDto" />
        /// </summary>
        /// <returns>A task where the result is a collection of <see cref="UserEntityDto" /></returns>
        public async Task<List<UserEntityDto>> GetUsers()
        {
            var response = await this.HttpClient.GetAsync(this.MainRoute);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(content);
            }

            var entities =  JsonConvert.DeserializeObject<IEnumerable<EntityDto>>(content, this.JsonSerializerOptions);
            return entities.Where(x => x.GetType() == typeof(UserEntityDto)).Cast<UserEntityDto>().ToList();
        }

        /// <summary>
        ///     Register a new user based on <see cref="RegistrationDto" />
        /// </summary>
        /// <param name="newUser">The <see cref="RegistrationDto" /></param>
        /// <returns>A <see cref="Task" /> with the result of the request</returns>
        public async Task<RegistrationResponseDto> RegisterUser(RegistrationDto newUser)
        {
            var content = JsonSerializer.Serialize(newUser);
            var bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            var registerResponse = await this.HttpClient.PostAsync(Path.Combine(this.MainRoute, "Register"), bodyContent);
            var registerContent = await registerResponse.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<RegistrationResponseDto>(registerContent, this.JsonSerializerOptions);
        }

        /// <summary>
        ///     Delete a registered user
        /// </summary>
        /// <param name="userEntityToDelete">The user to delete</param>
        /// <returns>A <see cref="Task" /> with the <see cref="RequestResponseDto" /></returns>
        public async Task<RequestResponseDto> DeleteUser(UserEntityDto userEntityToDelete)
        {
            if (!userEntityToDelete.IsAdmin)
            {
                var url = Path.Combine(this.MainRoute, userEntityToDelete.Id.ToString());
                var deleteResponse = await this.HttpClient.DeleteAsync(url);
                var deleteContent = await deleteResponse.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<RequestResponseDto>(deleteContent, this.JsonSerializerOptions);
            }

            return new RequestResponseDto
            {
                Errors = new List<string>
                {
                    "This action is forbidden"
                }
            };
        }
    }
}
