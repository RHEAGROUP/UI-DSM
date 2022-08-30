﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.Administration.ParticipantService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ParticipantServiceTestFixture
    {
        private ParticipantService service;
        private MockHttpMessageHandler httpMessageHandler;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<ParticipantService>();
            this.service = new ParticipantService(httpClient);
        }

        [Test]
        public async Task VerifyGetParticipants()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Participant");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetParticipantsOfProject(projectId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.Forbidden;
            Assert.That(async () => await this.service.GetParticipantsOfProject(projectId), Throws.Exception);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var roleGuid1 = Guid.NewGuid();
            var userGuid1 = Guid.NewGuid();
            var roleGuid2 = Guid.NewGuid();
            var userGuid2 = Guid.NewGuid();

            var entitiesDto = new List<EntityDto>
            {
                new ParticipantDto(Guid.NewGuid())
                {
                    Role = roleGuid1,
                    User = userGuid1
                },
                new RoleDto(roleGuid1)
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                },
                new UserDto(userGuid1)
                {
                    UserName = "admin", 
                    IsAdmin = true
                }, 
                new ParticipantDto(Guid.NewGuid())
                {
                    Role = roleGuid2,
                    User = userGuid2
                },
                new RoleDto(roleGuid2)
                {
                    RoleName = "Reviewer",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                },
                new UserDto(userGuid2)
                {
                    UserName = "user"
                }
            };

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(entitiesDto));

            var participants = await this.service.GetParticipantsOfProject(projectId);
            Assert.That(participants.Count, Is.EqualTo(2));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetParticipantsOfProject(projectId), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetParticipant()
        {
            var projectId = Guid.NewGuid();
            var guid = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When(HttpMethod.Get, $"/Project/{projectId}/Participant/{guid}");
            request.Respond(_ => httpResponse);
            var participant = await this.service.GetParticipantOfProject(projectId, guid);

            Assert.That(participant, Is.Null);

            httpResponse.StatusCode = HttpStatusCode.OK;

            var roleGuid1 = Guid.NewGuid();
            var userGuid1 = Guid.NewGuid();

            var entitiesDto = new List<EntityDto>
            {
                new ParticipantDto(guid)
                {
                    Role = roleGuid1,
                    User = userGuid1
                },
                new RoleDto(roleGuid1)
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                },
                new UserDto(userGuid1)
                {
                    UserName = "admin",
                    IsAdmin = true
                }
            };

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(entitiesDto));
            participant = await this.service.GetParticipantOfProject(projectId,guid);

            Assert.That(participant.Id, Is.EqualTo(guid));

            httpResponse.Content = new StringContent(string.Empty);
            participant = await this.service.GetParticipantOfProject(projectId, guid);

            Assert.That(participant, Is.Null);
        }

        [Test]
        public async Task VerifyCreateParticipant()
        {
            var participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var projectId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(entityRequestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Participant/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.CreateParticipant(projectId, participant);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = participant.GetAssociatedEntities().Select(x => x.ToDto()).ToList();

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(entityRequestResponse));

            requestResponse = await this.service.CreateParticipant(projectId, participant);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });
            
            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.CreateParticipant(projectId, participant), Throws.Exception);
        }

        [Test]
        public async Task VerifyDeleteParticipant()
        {
            var participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new RequestResponseDto() { IsRequestSuccessful = true };
            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(requestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Delete, $"/Project/{project.Id}/Participant/{participant.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.DeleteParticipant(participant), Throws.Exception);

            project.Participants.Add(participant);
            var result = await this.service.DeleteParticipant(participant);
            Assert.That(result.IsRequestSuccessful, Is.True);

            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.DeleteParticipant(participant), Throws.Exception);
        }

        [Test]
        public async Task VerifyUpdateParticipant()
        {
            var participant = new Participant()
            {
                Role = new Role(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant
                    }
                },
                User = new UserEntity(Guid.NewGuid())
                {
                    UserName = "user"
                }
            };

            var project = new Project(Guid.NewGuid());

            var httpResponse = new HttpResponseMessage();
            var requestResponse = new EntityRequestResponseDto() { IsRequestSuccessful = false };
            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(requestResponse));

            var request = this.httpMessageHandler.When(HttpMethod.Put, $"/Project/{project.Id}/Participant/{participant.Id}");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.UpdateParticipant(participant), Throws.Exception);
            project.Participants.Add(participant);

            var requestResult = await this.service.UpdateParticipant(participant);
            Assert.That(requestResult.IsRequestSuccessful, Is.False);

            requestResponse.IsRequestSuccessful = true;

            requestResponse.Entities = participant.GetAssociatedEntities().Select(x => x.ToDto()).ToList();

            httpResponse.Content = new StringContent(JsonSerializerHelper.SerializeObject(requestResponse));

            requestResult = await this.service.UpdateParticipant(participant);
            Assert.That(requestResult.IsRequestSuccessful, Is.True);
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.UpdateParticipant(participant), Throws.Exception);
        }
    }
}