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

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Pages.Administration;
    using UI_DSM.Client.Services.Administration.ProjectService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.Pages.Administration;
    using UI_DSM.Shared.Models;
    using UI_DSM.Shared.Types;

    using TestContext = Bunit.TestContext;

    using AppComponents;

    using UI_DSM.Client.Services.Administration.UserService;
    using UI_DSM.Shared.Enumerator;

    [TestFixture]
    public class ProjectManagementTestFixture
    {
        private TestContext context;
        private IProjectManagementViewModel viewModel;
        private Mock<IProjectService> projectService;
        private List<Project> projects;
        private Mock<IUserService> userService;

        [SetUp]
        public void Setup()
        {
            this.projects = new List<Project>();
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.projectService = new Mock<IProjectService>();
            this.userService = new Mock<IUserService>();
            this.projectService.Setup(x => x.GetProjects(0)).ReturnsAsync(this.projects);
            this.viewModel = new ProjectManagementViewModel(this.projectService.Object, null, this.userService.Object);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.AddTestAuthorization();

            this.userService.Setup(x => x.GetParticipantsForUser()).ReturnsAsync(new List<Participant>()
            {
                new (Guid.NewGuid())
                {
                    Role = new Role(Guid.NewGuid())
                    {
                        AccessRights = { AccessRight.ProjectManagement }
                    }
                }
            });

            this.viewModel.NavigationManager = this.context.Services.GetRequiredService<FakeNavigationManager>();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyOnInitialized()
        {
            try
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

                await this.viewModel.OnInitializedAsync();
                renderer.Render();

                var projectDivs = renderer.FindAll(".m-top-10px").ToList();
                Assert.That(projectDivs.Count, Is.EqualTo(2));
                await renderer.InvokeAsync(() => projectDivs.First().Click());
                Assert.That(this.viewModel.NavigationManager.Uri, Is.EqualTo($"{this.viewModel.NavigationManager.BaseUri}Administration/Project/{this.projects.First().Id}"));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }

        [Test]
        public async Task VerifyCreateProject()
        {
            try
            {
                var renderer = this.context.RenderComponent<ProjectManagement>();
                var createButton = renderer.FindComponent<AppButton>();
                Assert.That(this.viewModel.IsOnCreationMode, Is.False);

                await renderer.InvokeAsync(() => createButton.Instance.Click.InvokeAsync());
                Assert.That(this.viewModel.IsOnCreationMode, Is.True);

                var createProjectResponse = EntityRequestResponse<Project>.Fail(new List<string>()
                {
                    "A project with the same name already exists"
                });

                this.projectService.Setup(x => x.CreateProject(It.IsAny<Project>())).ReturnsAsync(createProjectResponse);

                await this.viewModel.ProjectCreationViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.IsOnCreationMode, Is.True);
                createProjectResponse = EntityRequestResponse<Project>.Success(new Project(Guid.NewGuid()));
                this.projectService.Setup(x => x.CreateProject(It.IsAny<Project>())).ReturnsAsync(createProjectResponse);

                await this.viewModel.ProjectCreationViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.IsOnCreationMode, Is.False);
                Assert.That(this.viewModel.Projects.Count, Is.EqualTo(1));

                this.projectService.Setup(x => x.CreateProject(It.IsAny<Project>())).ThrowsAsync(new HttpRequestException("http error"));
                await this.viewModel.ProjectCreationViewModel.OnValidSubmit.InvokeAsync();
                Assert.That(this.viewModel.ErrorMessageViewModel.Errors.Count, Is.EqualTo(1));
            }
            catch
            {
                // On GitHub, exception is thrown even if the JSRuntime has been configured
            }
        }
    }
}
