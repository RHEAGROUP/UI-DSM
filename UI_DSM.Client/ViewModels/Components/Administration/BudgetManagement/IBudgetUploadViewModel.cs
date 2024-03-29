﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IBudgetUploadViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.BudgetManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.Model;

    /// <summary>
    ///     Interface definition for <see cref="BudgetUploadViewModel" />
    /// </summary>
    public interface IBudgetUploadViewModel
    {
        /// <summary>
        ///     The <see cref="BudgetUploadModel" />
        /// </summary>
        BudgetUploadModel BudgetData { get; set; }

        /// <summary>
        ///     <see cref="EventCallback" /> to be called to upload the selected file
        /// </summary>
        EventCallback OnSubmit { get; set; }
    }
}
