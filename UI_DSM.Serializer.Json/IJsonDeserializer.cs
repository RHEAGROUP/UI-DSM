﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="IJsonDeserializer.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Serializer.Json
{
    using GP.SearchService.SDK.Definitions;

    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     Interface definition for <see cref="JsonDeserializer" />
    /// </summary>
    public interface IJsonDeserializer
    {
        /// <summary>
        ///     Deserializes the JSON stream to an <see cref="IEnumerable{EntityDto}" />
        /// </summary>
        /// <returns>A collection of <see cref="EntityDto" /></returns>
        IEnumerable<EntityDto> Deserialize(Stream stream);

        /// <summary>
        ///     Deserialize a <see cref="Stream" /> into a <see cref="EntityRequestResponseDto" />
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /></param>
        /// <returns>The <see cref="EntityRequestResponseDto" /></returns>
        EntityRequestResponseDto DeserializeEntityRequestResponseDto(Stream stream);

        /// <summary>
        ///     Deserializes a <see cref="Stream"/> into a collection of <see cref="CommonBaseSearchDto" />
        /// </summary>
        /// <param name="stream">The <see cref="Stream" /></param>
        /// <returns>A collection of <see cref="CommonBaseSearchDto" /></returns>
        IEnumerable<CommonBaseSearchDto> DeserializeISearchDto(Stream stream);
    }
}
