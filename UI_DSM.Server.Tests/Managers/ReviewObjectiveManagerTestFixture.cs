﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveManagerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Managers
{
    using Microsoft.EntityFrameworkCore;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.AnnotationManager;
    using UI_DSM.Server.Managers.ParticipantManager;
    using UI_DSM.Server.Managers.ReviewCategoryManager;
    using UI_DSM.Server.Managers.ReviewObjectiveManager;
    using UI_DSM.Server.Managers.ReviewTaskManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class ReviewObjectiveManagerTestFixture
    {
        private ReviewObjectiveManager manager;
        private Mock<DatabaseContext> context;
        private Mock<IParticipantManager> participantManager;
        private Mock<IReviewTaskManager> reviewTaskManager;
        private Mock<IAnnotationManager> annotationManager;
        private Mock<IReviewCategoryManager> reviewCategoryManager;
        private Mock<DbSet<ReviewObjective>> reviewObjectiveDbSet;
        private Mock<DbSet<Review>> reviewDbSet;
        private Mock<DbSet<Comment>> commentDbSet;

        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.context.CreateDbSetForContext(out this.reviewObjectiveDbSet, out this.reviewDbSet);
            this.context.CreateDbSetForContext(out this.commentDbSet);
            this.context.Setup(x => x.Comments).Returns(this.commentDbSet.Object);
            this.participantManager = new Mock<IParticipantManager>();
            this.reviewTaskManager = new Mock<IReviewTaskManager>();
            this.annotationManager = new Mock<IAnnotationManager>();
            this.reviewCategoryManager = new Mock<IReviewCategoryManager>();

            this.manager = new ReviewObjectiveManager(this.context.Object, this.participantManager.Object, this.reviewTaskManager.Object,
                this.annotationManager.Object, this.reviewCategoryManager.Object);

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

            var reviewObjectives = new List<ReviewObjective>()
            {
                new(Guid.NewGuid())
                {
                    CreatedOn = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1)),
                    Description = "A review objective",
                    ReviewObjectiveNumber = 1,
                    Status = StatusKind.Closed,
                },
                new(Guid.NewGuid())
                {
                    CreatedOn = DateTime.UtcNow,
                    Description = "Another review objective",
                    ReviewObjectiveNumber = 1,
                    Status = StatusKind.Open,
                }
            };

            this.reviewObjectiveDbSet.UpdateDbSetCollection(reviewObjectives);

            var allEntities = await this.manager.GetEntities(1);
            Assert.That(allEntities.ToList(), Has.Count.EqualTo(2));

            var invalidGuid = Guid.NewGuid();
            var foundReview = await this.manager.FindEntity(invalidGuid);
            Assert.That(foundReview, Is.Null);

            var foundEntities = await this.manager.FindEntities(reviewObjectives.Select(x => x.Id));
            Assert.That(foundEntities.ToList(), Has.Count.EqualTo(2));

            var deepEntity = await this.manager.GetEntity(reviewObjectives.First().Id, 1);
            Assert.That(deepEntity.ToList(), Has.Count.EqualTo(1));

            var review = new Review(Guid.NewGuid())
            {
                ReviewObjectives = { reviewObjectives.First() }
            };

            var containedEntities = await this.manager.GetContainedEntities(review.Id);
            Assert.That(containedEntities.ToList(), Has.Count.EqualTo(1));
        }

        [Test]
        public async Task VerifyCreateEntity()
        {
            var reviewObjective = new ReviewObjective();

            var operationResult = await this.manager.CreateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
            reviewObjective.Description = "Review Description";

            operationResult = await this.manager.CreateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    ReviewObjectives =
                    {
                        reviewObjective
                    }
                }
            };

           this.reviewDbSet.UpdateDbSetCollection(reviews);

            await this.manager.CreateEntity(reviewObjective);
            this.context.Verify(x => x.Add(reviewObjective), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.CreateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyUpdateEntity()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid());

            var operationResult = await this.manager.UpdateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
            reviewObjective.Description = "Review new Description";

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    ReviewObjectives =
                    {
                        reviewObjective
                    }
                }
            };

            this.reviewDbSet.UpdateDbSetCollection(reviews);
            this.reviewObjectiveDbSet.UpdateDbSetCollection(reviews.First().ReviewObjectives);
            await this.manager.UpdateEntity(reviewObjective);
            this.context.Verify(x => x.Update(reviewObjective), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.UpdateEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);

            reviewObjective.ReviewTasks.Add(new ReviewTask(Guid.NewGuid())
            {
                Status = StatusKind.Done
            });

            await this.manager.UpdateStatus(reviewObjective.Id);
            Assert.That(reviewObjective.Status, Is.EqualTo(StatusKind.Done));

            reviewObjective.ReviewTasks.Add(new ReviewTask(Guid.NewGuid())
            {
                Status = StatusKind.Open
            });

            await this.manager.UpdateStatus(reviewObjective.Id);
            Assert.That(reviewObjective.Status, Is.EqualTo(StatusKind.Open));

            await this.manager.UpdateStatus(reviewObjective.Id);
            Assert.That(reviewObjective.Status, Is.EqualTo(StatusKind.Open));
        }

        [Test]
        public async Task VerifyDelete()
        {
            var reviewObjective = new ReviewObjective();
            var operationResult = await this.manager.DeleteEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    ReviewObjectives =
                    {
                        reviewObjective
                    }
                }
            };

            this.reviewDbSet.UpdateDbSetCollection(reviews);
            await this.manager.DeleteEntity(reviewObjective);
            this.context.Verify(x => x.Remove(reviewObjective), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default)).ThrowsAsync(new InvalidOperationException());
            operationResult = await this.manager.DeleteEntity(reviewObjective);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyResolveProperties()
        {
            var participant = new Participant(Guid.NewGuid());
            this.participantManager.Setup(x => x.FindEntity(participant.Id)).ReturnsAsync(participant);

            var tasks = new List<ReviewTask>
            {
                new(Guid.NewGuid()),
                new(Guid.NewGuid()),
                new(Guid.NewGuid())
            };

            this.reviewTaskManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(tasks);

            var annotations = new List<Annotation>()
            {
                new Comment(Guid.NewGuid()),
                new Feedback(Guid.NewGuid()),
                new Note(Guid.NewGuid())
            };

            this.annotationManager.Setup(x => x.FindEntities(It.IsAny<List<Guid>>())).ReturnsAsync(annotations);

            var reviewObjective = new ReviewObjective();
            await this.manager.ResolveProperties(reviewObjective, new UserEntityDto());

            var reviewDto = new ReviewObjectiveDto()
            {
                ReviewTasks = tasks.Select(x => x.Id).ToList(),
                Annotations = annotations.Select(x => x.Id).ToList()
            };

            await this.manager.ResolveProperties(reviewObjective, reviewDto);
           
            Assert.Multiple(() =>
            {
                Assert.That(reviewObjective.ReviewTasks, Has.Count.EqualTo(3));
                Assert.That(reviewObjective.Annotations, Has.Count.EqualTo(3));
            });
        }

        [Test]
        public async Task VerifyCreateEntityBasedOnTemplate()
        {
            Project project = new(Guid.NewGuid());
            
            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    Title = "Review Title",
                    EntityContainer = project,
                }
            };

            this.reviewDbSet.UpdateDbSetCollection(reviews);

            var template = new ReviewObjective()
            {
                Id = Guid.NewGuid(),
                ReviewObjectiveKind = ReviewObjectiveKind.Prr,
                ReviewObjectiveKindNumber = 1,
                Description = "Review Objective Description",
            };

            var operationResult = await this.manager.CreateEntityBasedOnTemplate(template, reviews[0]);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public async Task VerifyCreateEntityBasedOnTemplates()
        {
            Project project = new(Guid.NewGuid());
            
            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    Title = "Review Title",
                    EntityContainer = project,
                }
            };

            this.reviewDbSet.UpdateDbSetCollection(reviews);

            IEnumerable<ReviewObjective> templates = new List<ReviewObjective>()
            {
                new ()
                {
                    Id = Guid.NewGuid(),
                    ReviewObjectiveKind = ReviewObjectiveKind.Prr,
                    ReviewObjectiveKindNumber = 1,
                    Description = "Review Objective Description",
                }
            };

            var operationResult = await this.manager.CreateEntityBasedOnTemplates(templates, reviews[0]);
            Assert.That(operationResult.Succeeded, Is.False);
        }

        [Test]
        public void GetReviewObjectiveCreationForReview()
        {
            var reviewObjectivePrr1 = new ReviewObjective()
            {
                Id = Guid.NewGuid(),
                ReviewObjectiveKind = ReviewObjectiveKind.Prr,
                ReviewObjectiveKindNumber = 1
            };

            var reviewObjectivePrr2 = new ReviewObjective()
            {
                Id = Guid.NewGuid(),
                ReviewObjectiveKind = ReviewObjectiveKind.Prr,
                ReviewObjectiveKindNumber = 2
            };

            var reviewObjectiveSrr2 = new ReviewObjective()
            {
                Id = Guid.NewGuid(),
                ReviewObjectiveKind = ReviewObjectiveKind.Srr,
                ReviewObjectiveKindNumber = 2
            };

            var reviews = new List<Review>()
            {
                new(Guid.NewGuid())
                {
                    ReviewObjectives =
                    {
                        reviewObjectivePrr1,
                        reviewObjectiveSrr2,
                        reviewObjectivePrr2
                    }
                }
            };

            this.reviewDbSet.UpdateDbSetCollection(reviews);
            this.reviewObjectiveDbSet.UpdateDbSetCollection(reviews.First().ReviewObjectives);

            var reviewObjectivesFirstReview = this.manager.GetReviewObjectiveCreationForReview(reviews.First().Id);

            Assert.Multiple(() =>
            {
                Assert.That(reviewObjectivesFirstReview.ToList(), Has.Count.EqualTo(3));
            });
        }

        [Test]
        public async Task VerifyGetOpenTasksAndComments()
        {
            var participant = new Participant(Guid.NewGuid())
            {
                User = new UserEntity()
                {
                    UserName = "user"
                }
            };

            var project = new Project(Guid.NewGuid());
            var review = new Review(Guid.NewGuid());
            project.Reviews.Add(review);
            project.Reviews[0].ReviewObjectives.AddRange(CreateEntity<ReviewObjective>(3));
            project.Reviews[0].ReviewItems.AddRange(CreateEntity<ReviewItem>(3));

            this.participantManager.Setup(x => x.GetParticipantForProject(project.Id, participant.User.UserName))
                .ReturnsAsync(participant);

            foreach (var reviewReviewObjective in project.Reviews.SelectMany(x => x.ReviewObjectives))
            {
                reviewReviewObjective.ReviewTasks.AddRange(CreateEntity<ReviewTask>(4));

                reviewReviewObjective.ReviewTasks[0].IsAssignedTo.Add(participant);
            }

            project.Reviews[0].ReviewItems[0].Annotations.AddRange(CreateEntity<Comment>(8));

            this.reviewDbSet.UpdateDbSetCollection(project.Reviews);
            this.reviewObjectiveDbSet.UpdateDbSetCollection(project.Reviews.First().ReviewObjectives);

            var guids = project.Reviews.First().ReviewObjectives.Select(x => x.Id).ToList();
            var computedProjectProperties = await this.manager.GetOpenTasksAndComments(guids, project.Id, participant.User.UserName);

            var expectedComputed = new AdditionalComputedProperties
            {
                OpenCommentCount = 0,
                TaskCount = 1
            };

            Assert.Multiple(() =>
            {
                Assert.That(computedProjectProperties[review.ReviewObjectives.First().Id], Is.EqualTo(expectedComputed));
                Assert.That(computedProjectProperties.Keys, Has.Count.EqualTo(3));
            });
        }

        private static IEnumerable<TEntity> CreateEntity<TEntity>(int amountOfEntities) where TEntity : Entity, new()
        {
            var entities = new List<TEntity>();

            for (var entityCount = 0; entityCount < amountOfEntities; entityCount++)
            {
                entities.Add(new TEntity()
                {
                    Id = Guid.NewGuid()
                });
            }

            return entities;
        }

        [Test]
        public async Task VerifyGetSearchResult()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                EntityContainer = new Review(Guid.NewGuid())
                {
                    EntityContainer = new Project(Guid.NewGuid())
                }
            };

            var result = await this.manager.GetSearchResult(reviewObjective.Id);
            Assert.That(result, Is.Null);

            this.reviewObjectiveDbSet.UpdateDbSetCollection(new List<ReviewObjective> { reviewObjective });
            result = await this.manager.GetSearchResult(reviewObjective.Id);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public async Task VerifyGetExtraEntitiesToUnindex()
        {
            var reviewObjective = new ReviewObjective(Guid.NewGuid())
            {
                ReviewTasks = { new ReviewTask() },
                Annotations =
                {
                    new Comment()
                    {
                        Replies = { new Reply() }
                    }
                }
            };

            var result = await this.manager.GetExtraEntitiesToUnindex(reviewObjective.Id);
            Assert.That(result, Is.Empty);

            this.reviewObjectiveDbSet.UpdateDbSetCollection(new List<ReviewObjective>{reviewObjective});
            result = await this.manager.GetExtraEntitiesToUnindex(reviewObjective.Id);
            Assert.That(result.ToList(), Has.Count.EqualTo(3));
        }
    }
}
