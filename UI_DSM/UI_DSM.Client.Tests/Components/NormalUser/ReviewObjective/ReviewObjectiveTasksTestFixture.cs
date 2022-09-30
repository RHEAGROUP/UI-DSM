﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectCreationTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.ReviewObjective
{
    using AppComponents;
    
    using Bunit;
    using Bunit.TestDoubles;
    
    using DevExpress.Blazor;
    
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ReviewObjective;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewObjective;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ReviewObjectiveTasksTestFixture
    {
        private TestContext context;
        private IReviewObjectiveTasksViewModel viewModel;
        private IErrorMessageViewModel errorMessage;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.errorMessage = new ErrorMessageViewModel();
            this.context.ConfigureDevExpressBlazor();
            this.viewModel = new ReviewObjectiveTasksViewModel(null)
            {
                ReviewObjective = new ReviewObjective()
            };
            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<FakeNavigationManager>();
        }

        [TearDown]
        public void TearDown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<ReviewObjectiveTasks>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
                parameters.AddCascadingValue(this.errorMessage);
            });

            var reviewObjectiveTaskItem = renderer.FindComponents<TaskItem>();
            Assert.That(reviewObjectiveTaskItem.Count(), Is.EqualTo(0));

            this.viewModel.ReviewObjective.Title = "Review Objective 1";
            this.viewModel.ReviewObjective.ReviewTasks.Add(new ReviewTask(Guid.NewGuid()));

            renderer.Render();
            var reviewObjectiveTaskItem1 = renderer.FindComponents<TaskItem>();
            Assert.That(reviewObjectiveTaskItem1.Count, Is.EqualTo(1));
        }
    }
}
