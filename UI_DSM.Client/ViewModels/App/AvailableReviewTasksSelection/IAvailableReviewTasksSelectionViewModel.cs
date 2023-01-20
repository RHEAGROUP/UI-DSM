﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IAvailableReviewTasksSelectionViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.AvailableReviewTasksSelection
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="AvailableReviewTasksSelectionViewModel" />
    /// </summary>
    public interface IAvailableReviewTasksSelectionViewModel
    {
        /// <summary>
        ///     All available <see cref="ReviewTask" />
        /// </summary>
        IEnumerable<ReviewTask> ReviewTasks { get; set; }

        /// <summary>
        ///     The selected <see cref="ReviewTask" />
        /// </summary>
        ReviewTask SelectedReviewTask { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call on submit
        /// </summary>
        EventCallback OnSubmit { get; set; }

        /// <summary>
        ///     The current <see cref="Comment" />
        /// </summary>
        Comment CurrentComment { get; set; }

        /// <summary>
        ///     The current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        IHaveThingRowViewModel Row { get; set; }
    }
}
