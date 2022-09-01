﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ParticipantCreation.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Components.Administration.ParticipantManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Component that gave the possibility to create new <see cref="Participant" />
    /// </summary>
    public partial class ParticipantCreation
    {
        /// <summary>
        ///     The <see cref="IParticipantCreationViewModel" />
        /// </summary>
        [Parameter]
        public IParticipantCreationViewModel ViewModel { get; set; }
    }
}
