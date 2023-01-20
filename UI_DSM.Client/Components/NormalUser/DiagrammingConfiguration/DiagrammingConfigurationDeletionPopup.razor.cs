﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagrammingConfigurationDeletionPopup.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.DiagrammingConfiguration
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.NormalUser.DiagrammingConfiguration;

    /// <summary>
    ///     This component is used to load a diagramming configuration
    /// </summary>
    public partial class DiagrammingConfigurationDeletionPopup
    {
        /// <summary>
        ///     The <see cref="DiagrammingConfigurationDeletionPopupViewModel" /> for the component
        /// </summary>
        [Parameter]
        public IDiagrammingConfigurationDeletionPopupViewModel ViewModel { get; set; }
    }
}
