﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectManagementTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Pages.NormalUser
{
    using Bunit;
    
    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Shared.Models;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ProjectPage;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Client.Pages.NormalUser.ProjectPage;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class NormalProjectPageTestFixture
    {
        private TestContext context;
        private INormalProjectPageViewModel viewModel;
        private IProjectReviewViewModel projectReviewViewModel;
        private Mock<IProjectService> projectService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.projectReviewViewModel = new ProjectReviewViewModel(null);
            this.projectService = new Mock<IProjectService>();
            this.viewModel = new NormalProjectPageViewModel(this.projectService.Object, this.projectReviewViewModel);
            this.context.Services.AddSingleton(this.viewModel);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyOnInitialized()
        {

            var projectGuid = Guid.NewGuid();
            this.projectService.Setup(x => x.GetProject(projectGuid, 0)).ReturnsAsync((Project)null);

            var renderer = this.context.RenderComponent<NormalProjectPage>(parameters =>
            {
                parameters.Add(p => p.ProjectId, projectGuid.ToString());
            });

            var noProjectFound = renderer.Find("div");
            Assert.That(noProjectFound.InnerHtml.Contains("Project not found"), Is.True);

            var project = new Project(projectGuid)
            {
                ProjectName = "Project"
            };

            this.projectService.Setup(x => x.GetProject(projectGuid, 1)).ReturnsAsync(project);

            await this.viewModel.OnInitializedAsync(projectGuid);
            renderer.Render();

            Assert.That(this.viewModel.ProjectReviewViewModel.Project, Is.Not.Null);

            var projectsComponent = renderer.FindComponents<ProjectReview>();
            Assert.That(projectsComponent.Count, Is.EqualTo(1));
        }
    }
}
