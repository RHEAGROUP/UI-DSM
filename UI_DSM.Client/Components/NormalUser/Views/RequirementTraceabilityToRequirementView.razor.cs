﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="RequirementTraceabilityToRequirementView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component for the <see cref="View.RequirementTraceabilityToRequirementView" />
    /// </summary>
    public partial class RequirementTraceabilityToRequirementView
    {
        /// <summary>
        ///     Handle the fact that something has changed and needs to update the view
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        public override Task HasChanged()
        {
            this.ViewModel.UpdateAnnotatableRows(this.ViewModel.GetAvailablesRows().OfType<IHaveThingRowViewModel>()
                .Select(x => x.ReviewItem).ToList<AnnotatableItem>());

            return base.HasChanged();
        }
    }
}
