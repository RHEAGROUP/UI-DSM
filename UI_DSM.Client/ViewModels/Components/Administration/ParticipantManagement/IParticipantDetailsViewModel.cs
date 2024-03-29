﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IParticipantDetailsViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Components.Administration.ParticipantManagement
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Interface definition for <see cref="ParticipantDetailsViewModel" />
    /// </summary>
    public interface IParticipantDetailsViewModel
    {
        /// <summary>
        ///     The <see cref="Participant" />
        /// </summary>
        Participant Participant { get; set; }

        /// <summary>
        ///     A collection of available <see cref="Role" />
        /// </summary>
        IEnumerable<Role> AvailableRoles { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback" /> to call for data submit
        /// </summary>
        EventCallback OnValidSubmit { get; set; }

        /// <summary>
        ///     A collection of selected <see cref="CDP4Common.DTO.DomainOfExpertise" /> names
        /// </summary>
        IEnumerable<string> SelectedDomains { get; set; }

        /// <summary>
        ///     A collection of available <see cref="CDP4Common.SiteDirectoryData.DomainOfExpertise" /> names
        /// </summary>
        IEnumerable<string> AvailableDomains { get; set; }
    }
}
