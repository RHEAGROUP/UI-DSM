﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IProjectDetailsViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ReviewObjectiveViewModel" />
    /// </summary>
    public interface IReviewObjectiveViewModel
    {
        /// <summary>
        ///     The <see cref="Review" />
        /// </summary>
        Review Review { get; set; }

        
        /// <summary>
        ///     Navigate to the page dedicated to the given <see cref="ReviewObjective" />
        /// </summary>
        /// <param name="reviewObjective">The <see cref="ReviewObjective" /></param>
        void GoToReviewObjectivePage(ReviewObjective reviewObjective);

        NavigationManager NavigationManager { get; set; }
    }
}
