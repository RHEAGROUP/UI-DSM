﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IAboutService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Services.AboutService
{
    using UI_DSM.Server.Context;
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Interface definition for <see cref="AboutService" />
    /// </summary>
    public interface IAboutService
    {
        /// <summary>
        ///     Gets this system information
        /// </summary>
        /// <param name="databaseContext">The <see cref="DatabaseContext"/></param>
        /// <returns>The <see cref="SystemInformationDto" /></returns>
        SystemInformationDto GetSystemInformation(DatabaseContext databaseContext);
    }
}
