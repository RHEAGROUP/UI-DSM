﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="FunctionalTraceabilityToProductViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     ViewModel for the <see cref="Client.Components.NormalUser.Views.FunctionalTraceabilityToProductView" />
    ///     component
    /// </summary>
    public class FunctionalTraceabilityToProductViewViewModel : HaveTechnologyViewViewModel, IFunctionalTraceabilityToProductViewViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="rowsFilter">The <see cref="IFilterViewModel" /> for rows</param>
        /// <param name="columnsFilter">The <see cref="IFilterViewModel" /> for columns</param>
        public FunctionalTraceabilityToProductViewViewModel(IReviewItemService reviewItemService, IFilterViewModel rowsFilter, IFilterViewModel columnsFilter)
            : base(reviewItemService, rowsFilter, columnsFilter)
        {
        }

        /// <summary>
        ///     The header name of the column
        /// </summary>
        protected override string HeaderName => this.IsOnTechnologyView ? "source prod(technology)/target func" : "source prod/target func";

        /// <summary>
        ///     The name of the category
        /// </summary>
        protected override string TraceCategoryName => ThingExtension.ImplementCategoryName;

        /// <summary>
        ///     Apply a filtering on rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public override void FilterRows(Dictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            FilterElementBaseRows(selectedFilters, this.TraceabilityTableViewModel.Rows.OfType<ProductRowViewModel>());
        }

        /// <summary>
        ///     Apply a filtering on columns
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public override void FilterColumns(Dictionary<ClassKind, List<FilterRow>> selectedFilters)
        {
            FilterElementBaseRows(selectedFilters, this.TraceabilityTableViewModel.Columns.OfType<FunctionRowViewModel>());
        }

        /// <summary>
        ///     Initialize this view model properties
        /// </summary>
        /// <param name="things">A collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <param name="participant"></param>
        /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeProperties(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await base.InitializeProperties(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);

            var functions = this.Things.OfType<ElementUsage>()
                .Where(x => x.IsFunction())
                .ToList();

            var products = this.Things.OfType<ElementUsage>()
                .Where(x => x.IsProduct())
                .ToList();

            var relationships = this.Things.OfType<BinaryRelationship>()
                .Where(x => x.IsCategorizedBy(this.TraceCategoryName))
                .ToList();

            var filteredThings = new List<Thing>(functions);
            filteredThings.AddRange(products);
            filteredThings.AddRange(relationships);

            var reviewItems = await this.ReviewItemService.GetReviewItemsForThings(this.ProjectId, this.ReviewId,
                filteredThings.Select(x => x.Iid));

            var rows = new List<IHaveThingRowViewModel>(products
                .Select(x =>  new ProductRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid)))
                .OrderBy(x => x.Container.Name).ThenBy(x => x.Name));

            var columns = new List<IHaveThingRowViewModel>(functions
                .Select(x => new FunctionRowViewModel(x, reviewItems.FirstOrDefault(ri => ri.ThingId == x.Iid)))
                .OrderBy(x => x.Container.Name).ThenBy(x => x.Name));
            
            var reviewItemsForRelationships = reviewItems.Where(x => relationships.Any(rel => x.ThingId == rel.Iid)).ToList();

            this.TraceabilityTableViewModel.InitializeProperties(rows, columns);
            this.PopulateRelationships(rows, columns, reviewItemsForRelationships);
            InitializesFilterForElementBaseRows(this.RowsFilterViewModel, rows.OfType<ProductRowViewModel>());
            InitializesFilterForElementBaseRows(this.ColumnsFilterViewModel, columns.OfType<FunctionRowViewModel>());
        }
    }
}
