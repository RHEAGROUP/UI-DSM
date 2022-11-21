﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementBreakdownStructureViewTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Components.NormalUser.Views
{
    using Bunit;

    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using Microsoft.Extensions.DependencyInjection;

    using Moq;

    using NUnit.Framework;

    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Models;

    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class RequirementBreakdownStructureViewTestFixture
    {
        private TestContext context;
        private IRequirementBreakdownStructureViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new RequirementBreakdownStructureViewViewModel(this.reviewItemService.Object);
            this.context.ConfigureDevExpressBlazor();
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddTransient<IFilterViewModel, FilterViewModel>();
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public async Task VerifyComponent()
        {
            var renderer = this.context.RenderComponent<RequirementBreakdownStructureView>();

            var things = new List<Thing>();
            var requirementsSpecificiation = new RequirementsSpecification();
            var group = new RequirementsGroup();
            requirementsSpecificiation.Group.Add(group);
            
            requirementsSpecificiation.Requirement.Add(new Requirement(Guid.NewGuid(), null,null)
            {
                Group = group,
                Definition = 
                {
                    new Definition()
                    {
                        Content = "A definition"
                    }
                }
            });

            requirementsSpecificiation.Requirement.Add(new Requirement(Guid.NewGuid(), null, null));

            things.Add(requirementsSpecificiation);

            var reviewItems = new List<ReviewItem> 
            {
                new ()
                {
                    ThingId = requirementsSpecificiation.Requirement.First().Iid
                }
            };

            this.reviewItemService.Setup(x => x.GetReviewItemsForThings(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<IEnumerable<Guid>>(), It.IsAny<int>()))
                .ReturnsAsync(reviewItems);

            await renderer.InvokeAsync(() => renderer.Instance.InitializeViewModel(things, Guid.NewGuid(), Guid.NewGuid()));

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.Things, Is.Not.Empty);
                Assert.That(this.viewModel.SelectedElement, Is.Null);
            });

            this.viewModel.SelectedElement = group;

            Assert.Multiple(() =>
            {
                Assert.That(this.viewModel.SelectedElement, Is.Not.Null);
                Assert.That(renderer.Instance.SelectedItemObservable, Is.Not.Null);
                Assert.That(() => this.context.RenderComponent<RequirementVerificationControlView>(), Throws.Nothing);
            });
        }
    }
}
