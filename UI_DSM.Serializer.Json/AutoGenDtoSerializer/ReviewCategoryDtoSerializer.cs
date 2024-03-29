// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewCategoryDtoSerializer.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
//
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft
//
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
//
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

// --------------------------------------------------------------------------------------------------------
// ------------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!------------
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Serializer.Json
{
    using System.Text.Json;

    using UI_DSM.Shared.DTO.Models;

    /// <summary>
    ///     The purpose of the <see cref="ReviewCategoryDtoSerializer" /> is to provide serialization capabilities
    /// </summary>
    internal static class ReviewCategoryDtoSerializer
    {
        /// <summary>
        ///     Serializes an instance of <see cref="ReviewCategoryDto" /> using an <see cref="Utf8JsonWriter"/>
        /// </summary>
        /// <param name="obj">The <see cref="ReviewCategoryDto" /> to serialize</param>
        /// <param name="writer"> The target <see cref="Utf8JsonWriter" /></param>
        internal static void Serialize(object obj, Utf8JsonWriter writer)
        {
            if (obj is not ReviewCategoryDto dto)
            {
                throw new ArgumentException("The object shall be an ReviewCategoryDto", nameof(obj));
            }

            writer.WriteStartObject();

            writer.WritePropertyName("@type");
            writer.WriteStringValue("ReviewCategoryDto");

            writer.WritePropertyName("reviewCategoryName");
            writer.WriteStringValue(dto.ReviewCategoryName);

            writer.WritePropertyName("description");
            writer.WriteStringValue(dto.Description);

            writer.WritePropertyName("tagColor");
            writer.WriteStringValue(dto.TagColor);

            writer.WritePropertyName("id");
            writer.WriteStringValue(dto.Id);

            writer.WriteEndObject();
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------