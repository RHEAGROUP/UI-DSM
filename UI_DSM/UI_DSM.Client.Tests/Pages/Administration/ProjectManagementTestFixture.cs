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

namespace UI_DSM.Client.Tests.Pages.Administration
{
    using Bunit;
    using Bunit.TestDoubles;

    using DevExpress.Blazor;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.ViewModels.Pages.Administration;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class ProjectManagementTestFixture
    {
        private TestContext context;
        private IProjectManagementViewModel viewModel;
        private Mock<IProjectService> projectService;
        private List<Project> projects;

        [SetUp]
        public void Setup()
        {
            this.projects = new List<Project>();
            this.context = new TestContext();
            this.context.Services.AddDevExpressBlazor();
            this.projectService = new Mock<IProjectService>();
            this.projectService.Setup(x => x.GetProjects()).ReturnsAsync(this.projects);
            this.viewModel = new ProjectManagementViewModel(this.projectService.Object, null);
            this.context.Services.AddSingleton(this.viewModel);
            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<FakeNavigationManager>();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public void VerifyOnInitialized()
        {
            var renderer = this.context.RenderComponent<ProjectManagement>();

            var noProjectFound = renderer.Find("#noProjectFound");
            Assert.That(noProjectFound.TextContent.Contains("No project found"), Is.True);

            this.projects.Add(new Project(Guid.NewGuid())
            {
                ProjectName = "Project 1"
            });

            this.projects.Add(new Project(Guid.NewGuid())
            {
                ProjectName = "Project 2"
            });

            this.viewModel.OnInitializedAsync();
            renderer.Render();

            var projectDivs = renderer.FindAll(".m-top-10px");
            Assert.That(projectDivs.Count, Is.EqualTo(2));
            renderer.InvokeAsync(() => projectDivs.First().Click());
            Assert.That(this.viewModel.NavigationManager.Uri, Is.EqualTo($"{this.viewModel.NavigationManager.BaseUri}Administration/Project/{this.projects.First().Id}"));
        }

        [Test]
        public void VerifyCreateProject()
        {
            var renderer = this.context.RenderComponent<ProjectManagement>();
            var createButton = renderer.FindComponent<DxButton>();
            Assert.That(this.viewModel.CreationPopupVisible, Is.False);

            renderer.InvokeAsync(() => createButton.Instance.Click.InvokeAsync());
            Assert.That(this.viewModel.CreationPopupVisible, Is.True);

            var createProjectResponse = new EntityRequestResponse<Project>
            {
                Errors = new List<string>()
                {
                    "A project with the same name already exists"
                }
            };

            this.projectService.Setup(x => x.CreateProject(It.IsAny<Project>())).ReturnsAsync(createProjectResponse);

            renderer.InvokeAsync(() => this.viewModel.ProjectCreationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.CreationPopupVisible, Is.True);
            createProjectResponse.IsRequestSuccessful = true;
            createProjectResponse.Errors.Clear();
            createProjectResponse.Entity = new Project(Guid.NewGuid());
            renderer.InvokeAsync(() => this.viewModel.ProjectCreationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.CreationPopupVisible, Is.False);
            Assert.That(this.viewModel.Projects.Count, Is.EqualTo(1));

            this.projectService.Setup(x => x.CreateProject(It.IsAny<Project>())).ThrowsAsync(new HttpRequestException("http error"));
            renderer.InvokeAsync(() => this.viewModel.ProjectCreationViewModel.OnValidSubmit.InvokeAsync());
            Assert.That(this.viewModel.ErrorMessageViewModel.Errors.Count, Is.EqualTo(1));
        }
    }
}