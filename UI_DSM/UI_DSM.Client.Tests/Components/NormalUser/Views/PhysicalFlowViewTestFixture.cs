﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="InterfaceViewTestFixture.cs" company="RHEA System S.A.">
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
    using Blazor.Diagrams.Core.Geometry;
    using Bunit;
    using Castle.Components.DictionaryAdapter;
    using CDP4Common.DTO;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.Types;
    using CDP4Dal;
    using DevExpress.Blazor.Popup.Internal;
    using Feather.Blazor.Icons;

    using Microsoft.Extensions.DependencyInjection;
   
    using Moq;
   
    using NUnit.Framework;
    using Radzen.Blazor;
    using System.Collections.Concurrent;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.Enumerator;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.Tests.Helpers;
    using UI_DSM.Client.ViewModels.App.ConnectionVisibilitySelector;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    using BinaryRelationship = CDP4Common.DTO.BinaryRelationship;
    using ElementDefinition = CDP4Common.DTO.ElementDefinition;
    using ElementUsage = CDP4Common.DTO.ElementUsage;
    using TestContext = Bunit.TestContext;

    [TestFixture]
    public class PhysicalFlowViewTestFixture
    {
        private IInterfaceViewViewModel viewModel;
        private Mock<IReviewItemService> reviewItemService;
        private TestContext context;

        private readonly Uri uri = new Uri("http://test.com");

        private IRenderedComponent<PhysicalFlowView> renderer;

        [SetUp]
        public void Setup()
        {
            this.context = new TestContext();
            this.context.ConfigureDevExpressBlazor();
            this.reviewItemService = new Mock<IReviewItemService>();
            this.viewModel = new InterfaceViewViewModel(this.reviewItemService.Object);
            this.context.Services.AddSingleton(this.viewModel);
            this.context.Services.AddTransient<IFilterViewModel, FilterViewModel>();
            this.context.Services.AddTransient<IConnectionVisibilitySelectorViewModel, ConnectionVisibilitySelectorViewModel>();
            this.context.JSInterop.Setup<Rectangle>("ZBlazorDiagrams.getBoundingClientRect", _ => true);

            var portCategoryId = Guid.NewGuid();
            var productCategoryId = Guid.NewGuid();
            var interfaceCategoryId = Guid.NewGuid();

            var categories = new List<Category>
            {
                new()
                {
                    Iid = portCategoryId,
                    Name = "ports"
                },
                new()
                {
                    Iid = interfaceCategoryId,
                    Name = "interfaces"
                },
                new()
                {
                    Iid = productCategoryId,
                    Name = "products"
                }
            };

            var portDefinition = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Port",
                Category =
                {
                    portCategoryId
                }
            };

            var notConnectedPort = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Name = "Port_ACC",
                ElementDefinition = portDefinition.Iid,
                Category =
                {
                    portCategoryId
                }
            };

            var sourcePort = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Name = "Port_ALL",
                ElementDefinition = portDefinition.Iid,
                InterfaceEnd = InterfaceEndKind.INPUT,
                Category =
                {
                    portCategoryId
                }
            };

            var targetPort = new ElementUsage()
            {
                Iid = Guid.NewGuid(),
                Name = "Port_BLL",
                ElementDefinition = portDefinition.Iid,
                InterfaceEnd = InterfaceEndKind.OUTPUT,
                Category =
                {
                    portCategoryId
                }
            };

            var accelorometerBox = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Accelerometer Box",
                Category = { productCategoryId },
                ContainedElement = { targetPort.Iid, notConnectedPort.Iid }
            };

            var powerGenerator = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Battery",
                Category = { productCategoryId },
                ContainedElement = { sourcePort.Iid }
            };

            var emptyProduct = new ElementDefinition()
            {
                Iid = Guid.NewGuid(),
                Name = "Accelerometer Box 2",
                Category = { productCategoryId },
            };

            var interfaceRelationShip = new BinaryRelationship()
            {
                Iid = Guid.NewGuid(),
                Category = { interfaceCategoryId },
                Source = sourcePort.Iid,
                Target = targetPort.Iid
            };

            var assembler = new Assembler(new Uri("http://localhost"));

            var things = new List<Thing>(categories)
            {
                sourcePort,
                targetPort,
                notConnectedPort,
                accelorometerBox,
                powerGenerator,
                interfaceRelationShip,
                emptyProduct
            };

            assembler.Synchronize(things).Wait();
            _ = assembler.Cache.Select(x => x.Value.Value);

            var projectId = Guid.NewGuid();
            var reviewId = Guid.NewGuid();

            this.reviewItemService.Setup(x => x.GetReviewItemsForThings(projectId, reviewId, It.IsAny<IEnumerable<Guid>>(), 0))
                .ReturnsAsync(new List<ReviewItem>
                {
                    new (Guid.NewGuid())
                    {
                        ThingId = interfaceRelationShip.Iid,
                        Annotations = { new Comment(Guid.NewGuid()) }
                    }
                });

            this.renderer = this.context.RenderComponent<PhysicalFlowView>();

            var pocos = assembler.Cache.Where(x => x.Value.IsValueCreated)
                .Select(x => x.Value)
                .Select(x => x.Value)
                .ToList();

            var reviewItem = new ReviewItem(Guid.NewGuid());

            this.Initialize(pocos, projectId, reviewId);
        }

        public async void Initialize(IEnumerable<CDP4Common.CommonData.Thing> pocos, Guid projectId, Guid reviewId)
        {
            await renderer.Instance.InitializeViewModel(pocos, projectId, reviewId);
        }

        [TearDown]
        public void Teardown()
        {
            this.context.CleanContext();
        }

        [Test]
        public void VerifyThatGetNeighboursWorks()
        {
            var product = this.renderer.Instance.ViewModel.Products.First();
            var result = this.viewModel.GetNeighbours(product);
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void VerifyThatModelCanBeSelected()
        {
            var productNode = this.viewModel.ProductNodes.First();
            Assert.That(this.viewModel.SelectedElement, Is.Null);
            this.viewModel.SetSelectedModel(productNode);
            Assert.That(this.viewModel.SelectedElement, Is.Not.Null);
        }

        [Test]
        public void VerifyThatANewNodeCanBeCreatedFromAProduct()
        {
            var product = this.renderer.Instance.ViewModel.Products.Last();
            var nodeModel = this.viewModel.CreateNewNodeFromProduct(product);
            Assert.That(nodeModel, Is.Not.Null);
        }
    }
}
