﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IHaveTechnologyViewViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views
{
    /// <summary>
    /// Interface definition for <see cref="HaveTechnologyViewViewModel"/>
    /// </summary>
    public interface IHaveTechnologyViewViewModel : IHaveTraceabilityTableViewModel
    {
        /// <summary>
        ///     Value indicating that the current view should display Technology
        /// </summary>
        bool IsOnTechnologyView { get; set; }

        /// <summary>
        ///     Perfoms the switch between view
        /// </summary>
        void OnTechnologyViewChange();
    }
}
