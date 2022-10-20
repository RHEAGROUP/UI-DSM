﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewTaskCard.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.App.ReviewTaskCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Pages.NormalUser.ReviewObjectivePage;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component to display <see cref="ReviewTask" /> into the <see cref="ReviewObjectivePage" />
    /// </summary>
    public partial class ReviewTaskCard
    {
        /// <summary>
        ///     The <see cref="ReviewTask" /> to display information
        /// </summary>
        [Parameter]
        public ReviewTask ReviewTask { get; set; }
    }
}