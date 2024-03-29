﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskPage.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Pages.NormalUser.ReviewTaskPage
{
    using System.Reactive.Linq;

    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.Components;
    using UI_DSM.Client.Components.App.AnnotationLinker;
    using UI_DSM.Client.Components.App.Comments;
    using UI_DSM.Client.Components.App.SelectedItemCard;
    using UI_DSM.Client.Components.NormalUser.Views;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewTaskPage;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Page to proceed to a <see cref="ReviewTask" />
    /// </summary>
    public partial class ReviewTaskPage : IDisposable
    {
        /// <summary>
        ///     The collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The collection of <see cref="IDisposable" /> linked to a <see cref="BaseView" />
        /// </summary>
        private readonly List<IDisposable> viewDisposables = new();

        /// <summary>
        ///     Gets or sets if the <see cref="Components.App.LoadingComponent.LoadingComponent" /> is loading
        /// </summary>
        [Parameter]
        public bool IsLoading { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the project
        /// </summary>
        [Parameter]
        public string ProjectId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the review
        /// </summary>
        [Parameter]
        public string ReviewId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="ReviewObjective" />
        /// </summary>
        [Parameter]
        public string ReviewObjectiveId { get; set; }

        /// <summary>
        ///     The <see cref="Guid" /> of the <see cref="ReviewTask" />
        /// </summary>
        [Parameter]
        public string ReviewTaskId { get; set; }

        /// <summary>
        /// The <see cref="Guid"/> of the <see cref="Comment"/> to focus on, as parameter
        /// </summary>
        [Parameter]
        [SupplyParameterFromQuery]
        public string CommentId { get; set; }

        /// <summary>
        /// The <see cref="Guid"/> of the <see cref="Comment"/> to focus on
        /// </summary>
        private Guid commentId;

        /// <summary>
        ///     The <see cref="IReviewTaskPageViewModel" />
        /// </summary>
        [Inject]
        public IReviewTaskPageViewModel ViewModel { get; set; }

        /// <summary>
        ///     The <see cref="DynamicComponent" />
        /// </summary>
        public DynamicComponent BaseView { get; set; }

        /// <summary>
        ///     The currently selected item in the BaseView
        /// </summary>
        public object SelectedItem { get; set; }

        /// <summary>
        ///     The <see cref="SelectedItemCard" />
        /// </summary>
        public SelectedItemCard SelectedItemCard { get; set; }

        /// <summary>
        ///     The <see cref="Comments" />
        /// </summary>
        public Comments Comments { get; set; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.disposables.ForEach(x => x.Dispose());
        }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        protected override void OnInitialized()
        {
            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ReviewTask)
                .Subscribe(async _ => await this.OnSelectedItemChanged(this.ViewModel.ReviewTask)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.CurrentBaseView)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.SelectedView)
                .Where(x => x != null)
                .Subscribe(_ => this.OnSelectedViewChange()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.SelectedModel)
                .Where(x => x != null)
                .Subscribe(_ => this.OnSelectedModelChange()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ViewSelectorVisible)
                .Subscribe(async _ => await this.OnViewSelectorChanged()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ModelSelectorVisible)
                .Subscribe(async _ => await this.OnModelSelectorChanged()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ConfirmCancelDialog.IsVisible)
                .Where(x => !x).Subscribe(async _ => await this.OnConfirmClosed()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.DoneConfirmCancelPopup.IsVisible)
                .Where(x => !x).Subscribe(async _ => await this.OnConfirmClosed()));

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.IsLinkerVisible)
                .Subscribe(async x => await this.OnLinkerVisibilityChanged(x)));
        }

        /// <summary>
        ///     Method invoked when the component has received parameters from its parent in
        ///     the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnParametersSetAsync()
        {
            this.IsLoading = true;
            this.ViewModel.Reset();
            this.BaseView = new DynamicComponent();

            await this.ViewModel.OnInitializedAsync(new Guid(this.ProjectId), new Guid(this.ReviewId),
                new Guid(this.ReviewObjectiveId), new Guid(this.ReviewTaskId));

            if (Guid.TryParse(this.CommentId, out var parsedId))
            {
                this.commentId = parsedId;
            }

            await base.OnParametersSetAsync();
            this.IsLoading = false;
        }

        /// <summary>
        ///     Method invoked after each time the component has been rendered. Note that the component does
        ///     not automatically re-render after the completion of any returned <see cref="T:System.Threading.Tasks.Task" />, because
        ///     that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        ///     Set to <c>true</c> if this is the first time
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> has been invoked
        ///     on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="T:System.Threading.Tasks.Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        ///     The <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRender(System.Boolean)" /> and
        ///     <see cref="M:Microsoft.AspNetCore.Components.ComponentBase.OnAfterRenderAsync(System.Boolean)" /> lifecycle methods
        ///     are useful for performing interop, or interacting with values received from <c>@ref</c>.
        ///     Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        ///     once.
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (this.BaseView?.Instance is BaseView baseView && this.ViewModel.ShouldInitializeBaseView && baseView.GetType() == this.ViewModel.CurrentBaseView)
            {
                this.ViewModel.ShouldInitializeBaseView = false;
                this.DisposeViewDisposables();
                var projectId = new Guid(this.ProjectId);
                var reviewId = new Guid(this.ReviewId);
                var reviewTaskId = new Guid(this.ReviewTaskId);

                await this.Comments.InitializesProperties(projectId, reviewId, this.ViewModel.CurrentView,
                    this.ViewModel.Participant, this.ViewModel.OnLinkCallback, this.ViewModel.ReviewTask);

                this.SelectedItemCard.InitializeViewModel(this.ViewModel.Participant);

                if (baseView is not IReusableView reusableView || !await reusableView.CopyComponents(this.ViewModel.CurrentBaseViewInstance))
                {
                    await baseView.InitializeViewModel(this.ViewModel.Things, projectId, reviewId, reviewTaskId, this.ViewModel.GetPrefilters(), 
                        this.ViewModel.ReviewObjective.AdditionnalColumnsVisibleAtStart, this.ViewModel.Participant);

                    baseView.TrySetSelectedItem(this.SelectedItem);
                }

                if (this.SelectedItem is IHaveThingRowViewModel row)
                {
                    await baseView.TryNavigateToItem(row.Id);
                }

                this.ViewModel.CurrentBaseViewInstance = baseView;
                this.viewDisposables.Add(baseView.SelectedItemObservable.Subscribe(async x => await this.OnSelectedItemChanged(x)));

                this.viewDisposables.Add(this.Comments.ViewModel.Comments.CountChanged
                    .Subscribe(async _ => await this.OnCommentsCountChanged(baseView)));
            }

            await base.OnAfterRenderAsync(firstRender);
        }

        /// <summary>
        ///     Handle the change of visible of the <see cref="AnnotationLinker" /> component
        /// </summary>
        /// <param name="value">The new visibility</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnLinkerVisibilityChanged(bool value)
        {
            if (!value && this.ViewModel.CurrentBaseViewInstance != null)
            {
                await this.ViewModel.CurrentBaseViewInstance.HasChanged();
                this.Comments.ViewModel.SelectedItem = null;
                this.Comments.ViewModel.SelectedItem = this.SelectedItem;
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Handle the count Changed event on the Comments collection
        /// </summary>
        /// <param name="baseView">The <see cref="BaseView" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnCommentsCountChanged(BaseView baseView)
        {
            if (baseView is PhysicalFlowView physicalFlowView)
            {
                physicalFlowView.SelectedElementChangedComments(this.SelectedItem, this.Comments.ViewModel.Comments.Count > 0);
            }

            if (this.commentId != Guid.Empty && this.Comments.ViewModel.Comments.Items.Any())
            {
                var reviewItems = this.Comments.ViewModel.Comments.Items.FirstOrDefault(x => x.Id == this.commentId)
                    ?.AnnotatableItems.OfType<ReviewItem>().ToList();

                if (reviewItems != null && reviewItems.Any())
                {
                    var availableRow = baseView.GetAvailablesRows()
                        .FirstOrDefault(x => reviewItems.Any(r => x.AnnotatableItemId == r.Id));

                    if (availableRow is IHaveThingRowViewModel asThingRow)
                    {
                        baseView.TrySetSelectedItem(asThingRow);
                        await baseView.TryNavigateToItem(asThingRow.Id);
                    }
                }

                this.commentId = Guid.Empty;
            }

            await baseView.HasChanged();
        }

        /// <summary>
        ///     Handles the close event of the <see cref="ConfirmCancelPopup" /> component
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnConfirmClosed()
        {
            if (this.ViewModel.CurrentBaseViewInstance != null)
            {
                await this.ViewModel.CurrentBaseViewInstance.HasChanged();
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Handle the change of the selected model
        /// </summary>
        private void OnSelectedModelChange()
        {
            this.ViewModel.ModelSelectorVisible = false;
        }

        /// <summary>
        ///     Handles the change of the Model selector visibility
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnModelSelectorChanged()
        {
            BaseView reusableView = null;

            if (this.ViewModel.SelectedModel != null && this.ViewModel.CurrentModel.Id != this.ViewModel.SelectedModel.Id)
            {
                if (this.BaseView.Instance is BaseView reusable)
                {
                    reusableView = reusable;
                    reusableView.IsLoading = true;
                }

                var projectId = new Guid(this.ProjectId);
                var reviewId = new Guid(this.ReviewId);
                var reviewTaskId = new Guid(this.ReviewTaskId);
                await this.ViewModel.UpdateModel(this.ViewModel.SelectedModel);

                if (this.ViewModel.CurrentBaseViewInstance != null)
                {
                    await this.ViewModel.CurrentBaseViewInstance.InitializeViewModel(this.ViewModel.Things, projectId, reviewId, reviewTaskId, this.ViewModel.GetPrefilters(),
                        this.ViewModel.ReviewObjective.AdditionnalColumnsVisibleAtStart, this.ViewModel.Participant);

                    this.ViewModel.CurrentBaseViewInstance.TrySetSelectedItem(this.SelectedItem);
                }

                if (reusableView != null)
                {
                    reusableView.IsLoading = false;
                }
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Dispose all <see cref="IDisposable" /> linked to the current view
        /// </summary>
        private void DisposeViewDisposables()
        {
            this.viewDisposables.ForEach(x => x.Dispose());
            this.viewDisposables.Clear();
        }

        /// <summary>
        ///     Handle the changed of the selected view
        /// </summary>
        private void OnSelectedViewChange()
        {
            this.ViewModel.ViewSelectorVisible = false;
        }

        /// <summary>
        ///     Handles the change of the ViewSelector visibility
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnViewSelectorChanged()
        {
            if (this.ViewModel.SelectedView != null && this.ViewModel.CurrentView != this.ViewModel.SelectedView.View)
            {
                this.ViewModel.UpdateView(this.ViewModel.SelectedView.View);
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Handle the changed of the baseView Selected Item
        /// </summary>
        /// <param name="newSelectedItem">The new selected Item</param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnSelectedItemChanged(object newSelectedItem)
        {
            if (this.SelectedItemCard == null)
            {
                return;
            }

            newSelectedItem ??= this.ViewModel?.ReviewTask;

            this.SelectedItem = newSelectedItem;
            this.SelectedItemCard.ViewModel.SelectedItem = this.SelectedItem;
            this.Comments.ViewModel.SelectedItem = this.SelectedItem;

            if (this.ViewModel?.CurrentBaseViewInstance != null)
            {
                this.Comments.ViewModel.AvailableRows = this.ViewModel?.CurrentBaseViewInstance.GetAvailablesRows();
            }

            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Opens the view selector
        /// </summary>
        private void OpenViewSelector()
        {
            this.ViewModel.ViewSelectorVisible = !this.ViewModel.ViewSelectorVisible;
        }

        /// <summary>
        ///     Handle the selection of a new <see cref="View" />
        /// </summary>
        /// <param name="newView">The new <see cref="View" /></param>
        /// <returns>A <see cref="Task" /></returns>
        private async Task OnViewSelect(View newView)
        {
            this.ViewModel.UpdateView(newView, true);
            await this.InvokeAsync(this.StateHasChanged);
        }

        /// <summary>
        ///     Opens the model selector
        /// </summary>
        private void OpenModelSelector()
        {
            this.ViewModel.ModelSelectorVisible = !this.ViewModel.ModelSelectorVisible;
        }

        /// <summary>
        ///     Gets the <see cref="Model" /> name
        /// </summary>
        /// <returns>The model name</returns>
        private string GetModelName()
        {
            return this.ViewModel.CurrentModel == null ? "No model available" : this.ViewModel.CurrentModel.ModelName;
        }

        /// <summary>
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        private Task TryNavigateToItem(string itemName)
        {
            return this.ViewModel.CurrentBaseViewInstance != null ? this.ViewModel.CurrentBaseViewInstance.TryNavigateToItem(itemName) : Task.CompletedTask;
        }

        /// <summary>
        /// Clears the current selected element
        /// </summary>
        private void ClearSelection()
        {
            this.ViewModel.CurrentBaseViewInstance?.TrySetSelectedItem(null);
        }
    }
}
