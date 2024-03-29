﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="HaveThingRowViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using CDP4Common.CommonData;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface that defines that a row view model is linked to a <see cref="Thing" />
    /// </summary>
    public interface IHaveThingRowViewModel : IHaveAnnotatableItemRowViewModel
    {
        /// <summary>
        ///     The <see cref="UI_DSM.Shared.Models.ReviewItem" /> Id
        /// </summary>
        ReviewItem ReviewItem { get; }

        /// <summary>
        ///     Gets the <see cref="Thing" /> Id
        /// </summary>
        /// <returns>The <see cref="Guid" /></returns>
        Guid ThingId { get; }

        /// <summary>
        ///     Updates the current <see cref="ReviewItem" />
        /// </summary>
        /// <param name="item">The new <see cref="ReviewItem" /></param>
        void UpdateReviewItem(ReviewItem item);

        /// <summary>
        ///     Updates the current <see cref="Thing" />
        /// </summary>
        /// <param name="thing">The <see cref="Thing" /></param>
        /// <returns>True if the <see cref="Thing" /> as been updated</returns>
        bool UpdateThing(Thing thing);
    }
}
