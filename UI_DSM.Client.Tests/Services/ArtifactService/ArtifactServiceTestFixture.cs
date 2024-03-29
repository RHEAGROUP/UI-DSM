﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ArtifactServiceTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Services.ArtifactService
{
    using System.Net;

    using NUnit.Framework;

    using RichardSzalay.MockHttp;

    using UI_DSM.Client.Services;
    using UI_DSM.Client.Services.ArtifactService;
    using UI_DSM.Client.Services.JsonService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ArtifactServiceTestFixture
    {
        private ArtifactService service;
        private MockHttpMessageHandler httpMessageHandler;
        private IJsonService jsonService;
        private List<EntityDto> entitiesDto;

        [SetUp]
        public void Setup()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            var httpClient = this.httpMessageHandler.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/api");

            ServiceBase.RegisterService<ArtifactService>();

            this.jsonService = JsonSerializerHelper.CreateService();
            this.service = new ArtifactService(httpClient, this.jsonService);
           
            this.entitiesDto = new List<EntityDto>
            {
                new ModelDto(Guid.NewGuid())
                {
                    FileName = "file.zip"
                }
            };

            EntityHelper.RegisterEntities();
        }

        [Test]
        public async Task VerifyUploadModel()
        {
            var projectId = Guid.NewGuid();
            var fileName = $"{Guid.NewGuid()}.zip";
            var modelName = "A Model - Iteration 1";

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Artifact/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.UploadModel(projectId, fileName, modelName);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new Model().GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.UploadModel(projectId, fileName, modelName);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.UploadModel(projectId, fileName, modelName), Throws.Exception);
        }

        [Test]
        public async Task VerifyUploadBudget()
        {
            var projectId = Guid.NewGuid();
            var fileName = $"{Guid.NewGuid()}.zip";
            var guid = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage();

            var entityRequestResponse = new EntityRequestResponseDto()
            {
                IsRequestSuccessful = false
            };

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));
            var request = this.httpMessageHandler.When(HttpMethod.Post, $"/Project/{projectId}/Artifact/Create");
            request.Respond(_ => httpResponse);

            var requestResponse = await this.service.UploadBudget(projectId, fileName, guid);
            Assert.That(requestResponse.IsRequestSuccessful, Is.False);

            entityRequestResponse.IsRequestSuccessful = true;

            entityRequestResponse.Entities = new BudgetTemplate().GetAssociatedEntities().ToDtos();

            httpResponse.Content = new StringContent(this.jsonService.Serialize(entityRequestResponse));

            requestResponse = await this.service.UploadBudget(projectId, fileName, guid);

            Assert.Multiple(() =>
            {
                Assert.That(requestResponse.IsRequestSuccessful, Is.True);
                Assert.That(requestResponse.Entity, Is.Not.Null);
            });

            httpResponse.Content = new StringContent(string.Empty);

            Assert.That(async () => await this.service.UploadBudget(projectId, fileName, guid), Throws.Exception);
        }

        [Test]
        public async Task VerifyGetArtifacts()
        {
            var projectId = Guid.NewGuid();
            var httpResponse = new HttpResponseMessage();
            httpResponse.StatusCode = HttpStatusCode.NotFound;

            var request = this.httpMessageHandler.When($"/Project/{projectId}/Artifact");
            request.Respond(_ => httpResponse);

            Assert.That(async () => await this.service.GetArtifactsOfProject(projectId), Throws.Exception);
            httpResponse.StatusCode = HttpStatusCode.OK;

            httpResponse.Content = new StringContent(this.jsonService.Serialize(this.entitiesDto));
            var reviews = await this.service.GetArtifactsOfProject(projectId);
            Assert.That(reviews, Has.Count.EqualTo(1));
            httpResponse.Content = new StringContent(string.Empty);
            Assert.That(async () => await this.service.GetArtifactsOfProject(projectId), Throws.Exception);
        }
    }
}
