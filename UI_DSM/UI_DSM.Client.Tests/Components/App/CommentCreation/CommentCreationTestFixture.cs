﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentCreationTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.App.CommentCreation
{
    using Bunit;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Forms;

    using NUnit.Framework;

    using UI_DSM.Client.Components.App.CommentCreation;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.CommentCreation;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class CommentCreationTestFixture
    {
        private ICommentCreationViewModel viewModel;
        private TestContext context;
        private int eventCallbackCall;
        private IErrorMessageViewModel errorMessage;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.errorMessage = new ErrorMessageViewModel();

            this.viewModel = new CommentCreationViewModel()
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, () => this.eventCallbackCall++),
                Comment = new Comment()
            };
        }

        [TearDown]
        public void Teardown()
        {
            this.context.Dispose();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<CommentCreation>(parameters =>
            {
                parameters.Add(p => p.ViewModel, this.viewModel);
                parameters.AddCascadingValue(this.errorMessage);
            });

            var editForm = renderer.FindComponent<EditForm>();
            await renderer.InvokeAsync(editForm.Instance.OnSubmit.InvokeAsync);

            Assert.That(this.eventCallbackCall, Is.EqualTo(0));

            this.context.JSInterop.Mode = JSRuntimeMode.Strict;
            this.context.JSInterop.Setup<string>("QuillFunctions.getQuillHTML", _ => true).SetResult("A content");
            await renderer.InvokeAsync(editForm.Instance.OnSubmit.InvokeAsync);

            Assert.That(this.eventCallbackCall, Is.EqualTo(1));
        }
    }
}
