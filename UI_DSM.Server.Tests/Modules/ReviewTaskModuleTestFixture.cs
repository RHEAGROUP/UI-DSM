﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskModuleTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Modules
{
    using System.Security.Claims;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Routing;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Managers;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Modules;
    using UI_DSM.Server.Services.SearchService;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Server.Types;
    using UI_DSM.Server.Validator;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewTaskModuleTestFixture
    {
        private ReviewTaskModule module;
        private Mock<IEntityManager<ReviewTask>> reviewTaskManager;
        private Mock<HttpContext> context;
        private Mock<HttpResponse> response;
        private Mock<HttpRequest> request;
        private Mock<IServiceProvider> serviceProvider;
        private Mock<IEntityManager<Review>> reviewManager;
        private Mock<IEntityManager<ReviewObjective>> reviewObjectiveManager;
        private Mock<IParticipantManager> participantManager;
        private Mock<ISearchService> searchService;
        private RouteValueDictionary routeValues;
        private Guid projectId;
        private Guid reviewId;
        private Guid reviewObjectiveId;

        [SetUp]
        public void Setup()
        {
            this.reviewTaskManager = new Mock<IEntityManager<ReviewTask>>();
            this.reviewTaskManager.As<IReviewTaskManager>();
            this.reviewTaskManager.As<IContainedEntityManager<ReviewTask>>();
            this.reviewObjectiveManager = new Mock<IEntityManager<ReviewObjective>>();
            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>();
            this.reviewObjectiveManager.As<IReviewObjectiveManager>();
            this.reviewManager = new Mock<IEntityManager<Review>>();
            this.reviewManager.As<IContainedEntityManager<Review>>();
            this.participantManager = new Mock<IParticipantManager>();
            this.searchService = new Mock<ISearchService>();

            ModuleTestHelper.Setup<ReviewTaskModule, ReviewTaskDto>(new ReviewTaskDtoValidator(), out this.context,
                out this.response, out this.request, out this.serviceProvider);

            this.routeValues = new RouteValueDictionary();
            this.request.Setup(x => x.RouteValues).Returns(this.routeValues);
            this.projectId = Guid.NewGuid();
            this.reviewId = Guid.NewGuid();
            this.reviewObjectiveId = Guid.NewGuid();
            this.routeValues["projectId"] = this.projectId.ToString();
            this.routeValues["reviewId"] = this.reviewId.ToString();
            this.routeValues["reviewObjectiveId"] = this.reviewObjectiveId.ToString();

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Review>))).Returns(this.reviewManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<ReviewObjective>))).Returns(this.reviewObjectiveManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IReviewObjectiveManager))).Returns(this.reviewObjectiveManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<Review>))).Returns(this.reviewManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IEntityManager<ReviewObjective>))).Returns(this.reviewObjectiveManager.Object);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.context.Setup(x => x.User).Returns(new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new (ClaimTypes.Name, "user")
            })));

            this.module = new ReviewTaskModule();
            Program.RegisterEntities();
        }

        [Test]
        public async Task VerifyGetEntities()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            List<Participant> participants = new()
            {
                participant,
            };

            var reviewTasks = new List<ReviewTask>()
            {
                new(Guid.NewGuid())
                {
                    Description = "A review",
                    Title = "Title review",
                    IsAssignedTo = participants,
                    TaskNumber = 1,
                    Status = StatusKind.Closed
                },
                new(Guid.NewGuid())
                {
                    Description = "An other review",
                    Title = "Title review 2 ",
                    TaskNumber = 2,
                    Status = StatusKind.Open
                }
            };

            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(null);

            await this.module.GetEntities(this.reviewTaskManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Once);
            this.serviceProvider.Setup(x => x.GetService(typeof(IParticipantManager))).Returns(this.participantManager.Object);

            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntities(this.reviewTaskManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(It.IsAny<Guid>(), "user")).ReturnsAsync(participant);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewObjectiveId, this.reviewId)).ReturnsAsync(false);

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Review>))).Returns(null);

            await this.module.GetEntities(this.reviewTaskManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 500, Times.Exactly(2));

            this.serviceProvider.Setup(x => x.GetService(typeof(IContainedEntityManager<Review>))).Returns(this.reviewManager.Object);
            await this.module.GetEntities(this.reviewTaskManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewObjectiveId, this.reviewId)).ReturnsAsync(true);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            await this.module.GetEntities(this.reviewTaskManager.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewTaskManager.As<IReviewTaskManager>().Setup(x => x.GetContainedEntities(this.reviewObjectiveId, 0))
                .ReturnsAsync(reviewTasks);

            await this.module.GetEntities(this.reviewTaskManager.Object, this.context.Object);
            this.reviewTaskManager.As<IReviewTaskManager>().Verify(x => x.GetContainedEntities(this.reviewObjectiveId, 0), Times.Once);
        }

        [Test]
        public async Task VerifyGetEntity()
        {
            var reviewTask = new ReviewTask(Guid.NewGuid());

            _ = new ReviewObjective(this.reviewId)
            {
                ReviewTasks = {  reviewTask }
            };

            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetEntity(this.reviewTaskManager.Object, reviewTask.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);
            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewObjectiveId, this.reviewId)).ReturnsAsync(false);

            this.reviewTaskManager.As<IContainedEntityManager<ReviewTask>>().Setup(x => 
                x.FindEntityWithContainer(reviewTask.Id)).ReturnsAsync(reviewTask);

            await this.module.GetEntity(this.reviewTaskManager.Object, reviewTask.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewObjectiveId, this.reviewId)).ReturnsAsync(true);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(false);

            await this.module.GetEntity(this.reviewTaskManager.Object, reviewTask.Id, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Exactly(2));

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            await this.module.GetEntity(this.reviewTaskManager.Object, reviewTask.Id, this.context.Object);

            this.reviewTaskManager.As<IContainedEntityManager<ReviewTask>>().Verify(x =>
                x.FindEntityWithContainer(reviewTask.Id), Times.Once);
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var dto = new ReviewTaskDto()
            {
                Description = "Description",
                Title = "Title"
            };

            await this.module.CreateEntity(this.reviewTaskManager.Object, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 405, Times.Once);
        }

        [Test]
        public async Task VerifyDeleteEntity()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var reviewTask = new ReviewTask(Guid.NewGuid());

            _ = new ReviewObjective(this.reviewObjectiveId)
            {
                ReviewTasks = { reviewTask }
            };

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.DeleteEntity(this.reviewTaskManager.Object, reviewTask.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            await this.module.DeleteEntity(this.reviewTaskManager.Object, reviewTask.Id, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 400, Times.Once);

            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewObjectiveId, this.reviewId)).ReturnsAsync(true);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);

            this.reviewTaskManager.As<IContainedEntityManager<ReviewTask>>()
                .Setup(x => x.FindEntityWithContainer(reviewTask.Id)).ReturnsAsync(reviewTask);

            this.reviewTaskManager.Setup(x => x.DeleteEntity(reviewTask))
                .ReturnsAsync(EntityOperationResult<ReviewTask>.Success(null));

            var deleteResponse = await this.module.DeleteEntity(this.reviewTaskManager.Object, reviewTask.Id, this.searchService.Object, this.context.Object);
            Assert.That(deleteResponse.IsRequestSuccessful, Is.True);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var reviewTask = new ReviewTask(Guid.NewGuid())
            {
                Description = "Description",
                Title = "Title",
            };

            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            _ = new ReviewObjective(this.reviewObjectiveId)
            {
                ReviewTasks = { reviewTask }
            };
            
            var dto = reviewTask.ToDto() as ReviewTaskDto;

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.UpdateEntity(this.reviewTaskManager.Object, reviewTask.Id, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);
            
            this.reviewObjectiveManager.As<IContainedEntityManager<ReviewObjective>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewObjectiveId, this.reviewId)).ReturnsAsync(true);

            this.reviewManager.As<IContainedEntityManager<Review>>()
                .Setup(x => x.EntityIsContainedBy(this.reviewId, this.projectId)).ReturnsAsync(true);
            
            this.reviewTaskManager.As<IContainedEntityManager<ReviewTask>>()
                .Setup(x => x.FindEntityWithContainer(reviewTask.Id)).ReturnsAsync(reviewTask);

            this.reviewTaskManager.Setup(x => x.UpdateEntity(reviewTask))
                .ReturnsAsync(EntityOperationResult<ReviewTask>.Success(reviewTask));

            await this.module.UpdateEntity(this.reviewTaskManager.Object, reviewTask.Id, dto, this.searchService.Object, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 200, Times.Once);
        }

        [Test]
        public async Task VerifyGetCommentsCount()
        {
            var reviewTasks = new List<ReviewTask>
            {
                new(Guid.NewGuid())
            };

            var guids = reviewTasks.Select(x => x.Id).ToList();
            
            var result = new Dictionary<Guid, AdditionalComputedProperties>
            {
                [guids[0]] = new()
                {
                    OpenCommentCount = 15,
                    TaskCount = 0
                }
            };

            this.reviewTaskManager.As<IReviewTaskManager>().Setup(x => x.GetCommentsCount(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(result);
            await this.module.GetCommentsCount(this.reviewTaskManager.As<IReviewTaskManager>().Object, Guid.NewGuid(), this.context.Object);
            this.reviewTaskManager.As<IReviewTaskManager>().Verify(x => x.GetCommentsCount(It.IsAny<IEnumerable<Guid>>()), Times.Once);
        }

        [Test]
        public async Task VerifyGetReviewTasksForView()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                Role = new Role(Guid.NewGuid()),
                User = new UserEntity(Guid.NewGuid())
            };

            var viewName = "aView";

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync((Participant)null);

            await this.module.GetReviewTasksForView(this.reviewTaskManager.As<IReviewTaskManager>().Object, this.projectId, this.reviewId, this.reviewObjectiveId, viewName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 401, Times.Once);

            this.participantManager.Setup(x => x.GetParticipantForProject(this.projectId, "user")).ReturnsAsync(participant);

            await this.module.GetReviewTasksForView(this.reviewTaskManager.As<IReviewTaskManager>().Object, this.projectId, this.reviewId, this.reviewObjectiveId, viewName, this.context.Object);
            this.response.VerifySet(x => x.StatusCode = 300, Times.Once);

            viewName = View.InterfaceView.ToString();
            await this.module.GetReviewTasksForView(this.reviewTaskManager.As<IReviewTaskManager>().Object, this.projectId, this.reviewId, this.reviewObjectiveId, viewName, this.context.Object);
            this.reviewTaskManager.As<IReviewTaskManager>().Verify(x => x.GetReviewTasksForView(this.projectId, this.reviewId, View.InterfaceView), Times.Once);
        }
    }
}
