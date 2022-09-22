﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDetails.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This components provide <see cref="Project" /> reviews
    /// </summary>
    public partial class ProjectReview
    {
        /// <summary>
        ///     The <see cref="IProjectReviewViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IProjectReviewViewModel ViewModel { get; set; }
    }
}