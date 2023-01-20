﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ISearchService.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.SearchService
{
    using UI_DSM.Shared.DTO.Common;

    /// <summary>
    ///     Interface definition for <see cref="SearchService" />
    /// </summary>
    public interface ISearchService
    {
        /// <summary>
        ///     Gets search result
        /// </summary>
        /// <param name="searchKey">The keyword to search after</param>
        /// <returns>A <see cref="Task" /> with the collection of <see cref="SearchResultDto" /> response</returns>
        Task<IEnumerable<SearchResultDto>> SearchAfter(string searchKey);
    }
}
