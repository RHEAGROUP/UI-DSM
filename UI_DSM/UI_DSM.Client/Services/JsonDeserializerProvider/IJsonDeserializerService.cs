﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IJsonDeserializerProvider.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Services.JsonDeserializerProvider
{
    /// <summary>
    ///     Interface definition for <see cref="JsonDeserializerService" />
    /// </summary>
    public interface IJsonDeserializerService
    {
        /// <summary>
        ///     Deserialize a <see cref="Stream" /> into a <see cref="T" /> object
        /// </summary>
        /// <typeparam name="T">An object</typeparam>
        /// <param name="stream">The <see cref="Stream" /> to deserialize</param>
        /// <returns>The <see cref="T" /> object</returns>
        T Deserialize<T>(Stream stream) where T : class;
    }
}
