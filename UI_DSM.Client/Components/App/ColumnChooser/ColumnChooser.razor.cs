﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ColumnChooser.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ColumnChooser
{
    using Microsoft.AspNetCore.Components;

    using ReactiveUI;

    using UI_DSM.Client.ViewModels.App.ColumnChooser;

    /// <summary>
    ///     Component that allow to choose columns to show/hide
    /// </summary>
    public partial class ColumnChooser<TItem> : IDisposable
    {
        /// <summary>
        ///     A collection of <see cref="IDisposable" />
        /// </summary>
        private readonly List<IDisposable> disposables = new();

        /// <summary>
        ///     The <see cref="IColumnChooserViewModel{TItem}" />
        /// </summary>
        [Parameter]
        public IColumnChooserViewModel<TItem> ViewModel { get; set; }

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
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.disposables.Add(this.WhenAnyValue(x => x.ViewModel.ColumnChooserVisible)
                .Subscribe(_ => this.InvokeAsync(this.StateHasChanged)));
        }
    }
}
