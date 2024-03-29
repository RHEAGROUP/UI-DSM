﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="BudgetView.razor.cs" company="RHEA System S.A.">
//  Copyright (c) 2023 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for the <see cref="View.BudgetView" />
    /// </summary>
    public partial class BudgetView : GenericBaseView<IBudgetViewViewModel>
    {
        /// <summary>
        ///     The <see cref="IJSRuntime" />
        /// </summary>
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            this.IsLoading = true;
            await base.OnInitializedAsync();
            await this.SendDotNetInstanceToJS();
        }

        /// <summary>
        ///     Initialize the correspondant ViewModel for this component
        /// </summary>
        /// <param name="things">The collection of <see cref="Thing" /></param>
        /// <param name="projectId">The <see cref="Project" /> id</param>
        /// <param name="reviewId">The <see cref="Review" /> id</param>
        /// <param name="reviewTaskId">The <see cref="ReviewTask" /> id</param>
        /// <param name="prefilters">A collection of prefilters</param>
        /// <param name="additionnalColumnsVisibleAtStart">A collection of columns name that can be visible by default at start</param>
        /// <param name="participant">The current <see cref="Participant"/></param>
        /// /// <returns>A <see cref="Task" /></returns>
        public override async Task InitializeViewModel(IEnumerable<Thing> things, Guid projectId, Guid reviewId, Guid reviewTaskId, List<string> prefilters, List<string> additionnalColumnsVisibleAtStart, Participant participant)
        {
            await base.InitializeViewModel(things, projectId, reviewId, reviewTaskId, prefilters, additionnalColumnsVisibleAtStart, participant);
            this.IsLoading = false;
        }

        /// <summary>
        /// Links an instance of this class to the JS environment
        /// </summary>
        /// <returns>An awaitable <see cref="Task"/></returns>
        private async Task SendDotNetInstanceToJS()
        {
            var dotNetObjRef = DotNetObjectReference.Create(this);
            await this.JsRuntime.InvokeVoidAsync("window.ReportingViewerCustomization.setObjectRef", dotNetObjRef);
        }        
        
        /// <summary>
        /// Invokable method from the JS environment that handles clicking on a report element
        /// </summary>
        /// <param name="elementid">The Id of the selected <see cref="ElementBase"/></param>
        [JSInvokable]
        public void SetSelectedElement(string elementid)
        {
            this.ViewModel.TrySetSelectedItem(elementid);
        }

        /// <summary>
        ///     Tries to navigate to a corresponding item
        /// </summary>
        /// <param name="itemName">The name of the item to navigate to</param>
        /// <returns>A <see cref="Task" /></returns>
        public override Task TryNavigateToItem(string itemName)
        {
            return Task.CompletedTask;
        }
    }
}
