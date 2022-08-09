﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="UserDetailsTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.Administration.UserManagement
{
    using Bunit;

    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    using UI_DSM.Client.Components.Administration.UserManagement;
    using UI_DSM.Client.ViewModels.Components.Administration.UserManagement;
    using UI_DSM.Shared.DTO.UserManagement;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class UserDetailsTestFixture
    {
        private TestContext context;
        private IUserDetailsViewModel viewModel;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.Services.AddDevExpressBlazor();
            this.viewModel = new UserDetailsViewModel();
        }

        [Test]
        public void VerifyRender()
        {
            this.viewModel.User = new UserDto()
            {
                UserName = "admin",
                IsAdmin = true,
                UserId = Guid.NewGuid().ToString()
            };

            var renderer = this.context.RenderComponent<UserDetails>(parameters => 
                parameters.Add(p => p.ViewModel, this.viewModel));

            var inputTexts = renderer.FindComponents<InputText>();

            Assert.That(inputTexts[0].Instance.Value, Is.EqualTo(this.viewModel.User.UserId));
            Assert.That(inputTexts[1].Instance.Value, Is.EqualTo(this.viewModel.User.UserName));
        }
    }
}
