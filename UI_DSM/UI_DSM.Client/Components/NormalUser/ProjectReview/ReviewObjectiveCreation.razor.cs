﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectiveCreation.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.ProjectReview
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.ProjectReview;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     This component is used to create new <see cref="ReviewObjective" />(s)
    /// </summary>
    public partial class ReviewObjectiveCreation
    {
        /// <summary>
        ///     The <see cref="ReviewObjectiveCreationDto" />
        /// </summary>
        public ReviewObjectiveCreationDto ReviewObjectiveCreationDto { get; set; }

        List<ReviewObjectiveCreationDto> AvailableReviewObjectiveCreationDtoPrr { get; set; }

        List<ReviewObjectiveCreationDto> AvailableReviewObjectiveCreationDtoSrr { get; set; }


        /// <summary>
        ///     The <see cref="IReviewObjectiveCreationViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IReviewObjectiveCreationViewModel ViewModel { get; set; }

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await this.ViewModel.OnInitializedAsync();
            AvailableReviewObjectiveCreationDtoPrr = this.ViewModel.AvailableReviewObjectiveCreationDto.Where(x => x.Kind == ReviewObjectiveKind.Prr).ToList();
            AvailableReviewObjectiveCreationDtoSrr = this.ViewModel.AvailableReviewObjectiveCreationDto.Where(x => x.Kind == ReviewObjectiveKind.Srr).ToList();
        }
    }
}
