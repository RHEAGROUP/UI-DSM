﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveTraceabilityTableViewModel.cs" company="RHEA System S.A.">
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

    using ReactiveUI;

    using UI_DSM.Client.Components.App.TraceabilityTable;
    using UI_DSM.Client.Extensions;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.Services.ReviewItemService;
    using UI_DSM.Client.ViewModels.App.Filter;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for any View that have a <see cref="TraceabilityTable" /> component
    /// </summary>
    public abstract class HaveTraceabilityTableViewModel : BaseViewViewModel, IHaveTraceabilityTableViewModel
    {
        /// <summary>
        ///     A <see cref="Dictionary{TKey,TValue}" /> of available <see cref="RelationshipRowViewModel" />
        /// </summary>
        private readonly Dictionary<Guid, Dictionary<Guid, RelationshipRowViewModel>> AvailableRelationships = new();

        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     Backing field for <see cref="IsViewSettingsVisible" />
        /// </summary>
        private bool isViewSettingsVisible;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseViewViewModel" /> class.
        /// </summary>
        /// <param name="reviewItemService">The <see cref="IReviewItemService" /></param>
        /// <param name="rowsFilter">The <see cref="IFilterViewModel" /> for rows</param>
        /// <param name="columnsFilter">The <see cref="IFilterViewModel" /> for columns</param>
        protected HaveTraceabilityTableViewModel(IReviewItemService reviewItemService, IFilterViewModel rowsFilter, IFilterViewModel columnsFilter)
            : base(reviewItemService)
        {
            this.InitializeTable();
            this.RowsFilterViewModel = rowsFilter;
            this.ColumnsFilterViewModel = columnsFilter;
        }

        /// <summary>
        ///     Value asserting if the settings for the view should be visible
        /// </summary>
        public bool IsViewSettingsVisible
        {
            get => this.isViewSettingsVisible;
            set => this.RaiseAndSetIfChanged(ref this.isViewSettingsVisible, value);
        }

        /// <summary>
        ///     The header name of the column
        /// </summary>
        protected abstract string HeaderName { get; }

        /// <summary>
        ///     The name of the category
        /// </summary>
        protected abstract string TraceCategoryName { get; }

        /// <summary>
        ///     The <see cref="IFilterViewModel" /> for rows
        /// </summary>
        public IFilterViewModel RowsFilterViewModel { get; }

        /// <summary>
        ///     The <see cref="IFilterViewModel" /> for columns
        /// </summary>
        public IFilterViewModel ColumnsFilterViewModel { get; }

        /// <summary>
        ///     The <see cref="ITraceabilityTableViewModel" />
        /// </summary>
        public ITraceabilityTableViewModel TraceabilityTableViewModel { get; private set; }

        /// <summary>
        ///     Apply a filtering on rows
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public abstract void FilterRows(Dictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Apply a filtering on columns
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        public abstract void FilterColumns(Dictionary<ClassKind, List<FilterRow>> selectedFilters);

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Tries to set the <see cref="IBaseViewViewModel.SelectedElement" /> to the previous selected item
        /// </summary>
        /// <param name="selectedItem">The previously selectedItem</param>
        public override void TrySetSelectedItem(object selectedItem)
        {
            if (selectedItem is IHaveThingRowViewModel row)
            {
                var existingRow = this.GetAvailablesRows().OfType<IHaveThingRowViewModel>().FirstOrDefault(x => x.ThingId == row.ThingId);
                this.TraceabilityTableViewModel.SelectedElement = existingRow;
            }
        }

        /// <summary>
        ///     Gets a collection of all availables <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <returns>The collection of <see cref="IHaveThingRowViewModel" /></returns>
        public override List<IHaveAnnotatableItemRowViewModel> GetAvailablesRows()
        {
            var rows = new List<IHaveAnnotatableItemRowViewModel>();
            rows.AddRange(this.TraceabilityTableViewModel.Rows);
            rows.AddRange(this.TraceabilityTableViewModel.Columns);

            foreach (var availableRelationship in this.AvailableRelationships)
            {
                rows.AddRange(availableRelationship.Value
                    .Select(relationshipRowViewModel => relationshipRowViewModel.Value));
            }

            return rows;
        }

        /// <summary>
        ///     Updates all <see cref="IHaveAnnotatableItemRowViewModel" />
        /// </summary>
        /// <param name="annotatableItems">A collection of <see cref="AnnotatableItem" /></param>
        public override void UpdateAnnotatableRows(List<AnnotatableItem> annotatableItems)
        {
            var reviewItems = annotatableItems.OfType<ReviewItem>();
            this.TraceabilityTableViewModel.Rows.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));
            this.TraceabilityTableViewModel.Columns.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));

            foreach (var availableRelationship in this.AvailableRelationships)
            {
                var rows = availableRelationship.Value
                    .Select(relationshipRowViewModel => relationshipRowViewModel.Value).ToList();

                rows.ForEach(x => x.UpdateReviewItem(reviewItems.FirstOrDefault(ri => ri.ThingId == x.ThingId)));
            }
        }

        /// <summary>
        ///     Populates the <see cref="AvailableRelationships" />
        /// </summary>
        /// <param name="rows">All possible <see cref="IHaveThingRowViewModel" /> rows</param>
        /// <param name="columns">All possible <see cref="IHaveThingRowViewModel" /> columns</param>
        /// <param name="relationshipReviewItems">
        ///     A collection of <see cref="ReviewItem" /> for <see cref="BinaryRelationship" />
        /// </param>
        protected void PopulateRelationships(List<IHaveThingRowViewModel> rows, List<IHaveThingRowViewModel> columns, List<ReviewItem> relationshipReviewItems)
        {
            this.AvailableRelationships.Clear();

            var sources = new Dictionary<Guid, IEnumerable<Thing>>();
            var targets = new Dictionary<Guid, IEnumerable<Thing>>();

            foreach (var row in rows)
            {
                sources[row.ThingId] = GetLinkeableThings(row);
            }

            foreach (var column in columns)
            {
                targets[column.ThingId] = GetLinkeableThings(column);
            }

            foreach (var source in sources)
            {
                foreach (var sourceThing in source.Value)
                {
                    this.TryAddRelationships(source.Key, sourceThing, targets, relationshipReviewItems);
                }
            }
        }

        /// <summary>
        ///     Filters a collection of <see cref="RequirementRowViewModel" />
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        /// <param name="collectionToFilter">The collection</param>
        protected static void FilterRequirementRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters, IEnumerable<RequirementRowViewModel> collectionToFilter)
        {
            var selectedOwners = selectedFilters[ClassKind.DomainOfExpertise];
            var selectedSpecification = selectedFilters[ClassKind.RequirementsSpecification];

            foreach (var requirementRowViewModel in collectionToFilter)
            {
                requirementRowViewModel.IsVisible = selectedOwners.Any(x => x.DefinedThing.Iid == requirementRowViewModel.Thing.Owner.Iid)
                                                    && selectedSpecification.Any(x => x.DefinedThing.Iid == requirementRowViewModel.Thing.Container?.Iid);
            }
        }

        /// <summary>
        ///     Filters a collection of <see cref="ElementBaseRowViewModel" />
        /// </summary>
        /// <param name="selectedFilters">The selected filters</param>
        /// <param name="collectionToFilter">The collection</param>
        protected static void FilterElementBaseRows(IReadOnlyDictionary<ClassKind, List<FilterRow>> selectedFilters, IEnumerable<ElementBaseRowViewModel> collectionToFilter)
        {
            var selectedOwners = selectedFilters[ClassKind.DomainOfExpertise];
            var selectedContainers = selectedFilters[ClassKind.ElementDefinition];

            foreach (var elementBaseRowViewModel in collectionToFilter)
            {
                elementBaseRowViewModel.IsVisible = selectedOwners.Any(x => x.DefinedThing.Iid == elementBaseRowViewModel.Thing.Owner.Iid)
                                                    && selectedContainers.Any(x => x.DefinedThing.Iid == elementBaseRowViewModel.ContainerId);
            }
        }

        /// <summary>
        ///     Verifies that a <see cref="IHaveThingRowViewModel" /> is valid
        /// </summary>
        /// <param name="row">A <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The result of the verification</returns>
        protected virtual bool IsValidRow(IHaveThingRowViewModel row)
        {
            return true;
        }

        /// <summary>
        ///     Initializes the <paramref name="filterViewModel" /> for <see cref="RequirementRowViewModel" /> rows
        /// </summary>
        /// <param name="filterViewModel">The <see cref="IFilterViewModel" /></param>
        /// <param name="rows">The collection of <see cref="RequirementRowViewModel" /></param>
        protected static void InitializesFilterForRequirementRows(IFilterViewModel filterViewModel, IEnumerable<RequirementRowViewModel> rows)
        {
            var filters = new List<FilterModel>();
            rows = rows.ToList();

            var availableOwners = new List<DefinedThing>(rows
                .Select(x => x.Thing.Owner).DistinctBy(x => x.Iid));

            filters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Owner",
                Values = availableOwners
            });

            var availableRequirementsSpecification = new List<DefinedThing>(rows
                .Select(x => x.Thing.Container as RequirementsSpecification)
                .Where(x => x != null)
                .DistinctBy(x => x.Iid));

            filters.Add(new FilterModel
            {
                ClassKind = ClassKind.RequirementsSpecification,
                DisplayName = "Specification",
                Values = availableRequirementsSpecification
            });

            filterViewModel.InitializeProperties(filters);
        }

        /// <summary>
        ///     Initializes the <paramref name="filterViewModel" /> for <see cref="ElementBaseRowViewModel" /> rows
        /// </summary>
        /// <param name="filterViewModel">The <see cref="IFilterViewModel" /></param>
        /// <param name="rows">The collection of <see cref="ElementBaseRowViewModel" /></param>
        protected static void InitializesFilterForElementBaseRows(IFilterViewModel filterViewModel, IEnumerable<ElementBaseRowViewModel> rows)
        {
            var filters = new List<FilterModel>();
            rows = rows.ToList();

            var availableOwners = new List<DefinedThing>(rows
                .Select(x => x.Thing.Owner).DistinctBy(x => x.Iid));

            filters.Add(new FilterModel
            {
                ClassKind = ClassKind.DomainOfExpertise,
                DisplayName = "Owner",
                Values = availableOwners
            });

            var availableContainers = new List<DefinedThing>(rows.Where(x => x.Thing.Container is ElementDefinition)
                .Select(x => x.Thing.Container as DefinedThing).DistinctBy(x => x!.Iid));

            filters.Add(new FilterModel
            {
                ClassKind = ClassKind.ElementDefinition,
                DisplayName = "Container",
                Values = availableContainers
            });

            filterViewModel.InitializeProperties(filters);
        }

        /// <summary>
        ///     Tries to add all <see cref="RelationshipRowViewModel" /> for the provide <see cref="sourceThing" />
        /// </summary>
        /// <param name="sourceKey">The <see cref="Guid" /> of the <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="sourceThing">The <see cref="Thing" /></param>
        /// <param name="targets">All possible linkeable items</param>
        /// <param name="relationshipReviewItems">
        ///     A collection of <see cref="ReviewItem" /> for <see cref="BinaryRelationship" />
        /// </param>
        private void TryAddRelationships(Guid sourceKey, Thing sourceThing, Dictionary<Guid, IEnumerable<Thing>> targets, IReadOnlyCollection<ReviewItem> relationshipReviewItems)
        {
            foreach (var target in targets)
            {
                foreach (var targetThing in target.Value)
                {
                    this.TryAddRelationship(sourceThing.GetLinkTo(targetThing.Iid, this.TraceCategoryName), sourceKey, target.Key, relationshipReviewItems);
                }
            }
        }

        /// <summary>
        ///     Adds the <see cref="BinaryRelationship" /> inside the <see cref="AvailableRelationships" /> if exists
        /// </summary>
        /// <param name="relationship">A possible <see cref="BinaryRelationship" /></param>
        /// <param name="rowId">The <see cref="Guid" /> of the <see cref="IHaveThingRowViewModel" /> row</param>
        /// <param name="columnId">The <see cref="Guid" /> of the <see cref="IHaveThingRowViewModel" /> column</param>
        /// <param name="relationshipReviewItems">
        ///     A collection of <see cref="ReviewItem" /> for <see cref="BinaryRelationship" />
        /// </param>
        private void TryAddRelationship(BinaryRelationship relationship, Guid rowId, Guid columnId, IEnumerable<ReviewItem> relationshipReviewItems)
        {
            if (relationship != null)
            {
                if (!this.AvailableRelationships.ContainsKey(rowId))
                {
                    this.AvailableRelationships[rowId] = new Dictionary<Guid, RelationshipRowViewModel>();
                }

                this.AvailableRelationships[rowId][columnId] =
                    new RelationshipRowViewModel(relationship, relationshipReviewItems.FirstOrDefault(x => x.ThingId == relationship.Iid)
                        , this.TraceabilityTableViewModel.Rows.First(x => x.ThingId == rowId),
                        this.TraceabilityTableViewModel.Columns.First(x => x.ThingId == columnId));
            }
        }

        /// <summary>
        ///     Gets <see cref="Thing" />s that are linkeable inside a <see cref="IHaveThingRowViewModel" />
        /// </summary>
        /// <param name="row">The <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>A collection of <see cref="Thing" /></returns>
        private static IEnumerable<Thing> GetLinkeableThings(IHaveThingRowViewModel row)
        {
            return row switch
            {
                ElementBaseRowViewModel elementBaseRow => new List<Thing> { elementBaseRow.Thing },
                RequirementRowViewModel requirementRow => new List<Thing> { requirementRow.Thing },
                _ => Enumerable.Empty<Thing>()
            };
        }

        /// <summary>
        ///     Get the <see cref="RelationshipRowViewModel" /> that can exists between two <see cref="IHaveThingRowViewModel" />
        /// </summary>
        /// <param name="row">The row <see cref="IHaveThingRowViewModel" /></param>
        /// <param name="column">The column <see cref="IHaveThingRowViewModel" /></param>
        /// <returns>The <see cref="RelationshipRowViewModel" /> if exists</returns>
        private RelationshipRowViewModel GetRelationship(IHaveThingRowViewModel row, IHaveThingRowViewModel column)
        {
            if (row == null || column == null)
            {
                return null;
            }

            if (this.AvailableRelationships.TryGetValue(row.ThingId, out var possibleRelationships))
            {
                return possibleRelationships.TryGetValue(column.ThingId, out var relationshipRow) ? relationshipRow : null;
            }

            return null;
        }

        /// <summary>
        ///     Initializes the <see cref="TraceabilityTableViewModel" /> properties
        /// </summary>
        private void InitializeTable()
        {
            this.TraceabilityTableViewModel = new TraceabilityTableViewModel(this.HeaderName, this.GetRelationship, this.IsValidRow);

            this.disposables.Add(this.WhenAnyValue(x => x.TraceabilityTableViewModel.SelectedElement)
                .Subscribe(this.UpdateSelectedElement));
        }

        /// <summary>
        ///     Update the current selected element
        /// </summary>
        /// <param name="selectedElement">A <see cref="IHaveThingRowViewModel" /></param>
        private void UpdateSelectedElement(IHaveThingRowViewModel selectedElement)
        {
            this.SelectedElement = selectedElement;
        }
    }
}
