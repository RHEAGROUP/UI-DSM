// --------------------------------------------------------------------------------------------------------
// <copyright file="ReplyDtoDeserializer.cs" company="RHEA System S.A.">
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
    ///     The purpose of the <see cref="ReplyDtoDeserializer" /> is to provide deserialization capabilities
    /// </summary>
    internal static class ReplyDtoDeserializer
    {
        /// <summary>
        ///     Deserializes an instance of <see cref="ReplyDto" /> using an <see cref="JsonElement"/>
        /// </summary>
        /// <param name="jsonElement">The <see cref="JsonElement"/> that contains the <see cref="ReplyDto"/> json object</param>
        /// <returns>An instance of <see cref="ReplyDto"/></returns>
        internal static ReplyDto Deserialize(JsonElement jsonElement)
        {
            if (!jsonElement.TryGetProperty("@type", out var type))
            {
                throw new InvalidOperationException("The @type property is not available, the ReplyDtoDeSerializer cannot be used to deserialize this JsonElement");
            }

            if (type.GetString() != "ReplyDto")
            {
                throw new InvalidOperationException($"The ReplyDtoDeserializer can only be used to deserialize objects of type ReplyDto, a  {type.GetString()} was provided");
            }

            var dto = new ReplyDto();

            if (jsonElement.TryGetProperty("id", out var idProperty))
            {
                var propertyValue = idProperty.GetString();

                if (propertyValue == null)
                {
                    throw new JsonException("The id property is not present, the ReplyDto cannot be deserialized");
                }

                dto.Id = Guid.Parse(propertyValue);
            }

            if (jsonElement.TryGetProperty("author", out var authorProperty))
            {
                var propertyValue = authorProperty.GetString();

                if (propertyValue != null)
                {
                    dto.Author = Guid.Parse(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("createdOn", out var createdOnProperty))
            {
                var propertyValue = createdOnProperty.GetString();

                if (propertyValue != null)
                {
                    dto.CreatedOn = DateTime.Parse(propertyValue);
                }
            }

            if (jsonElement.TryGetProperty("content", out var contentProperty))
            {
                dto.Content = contentProperty.GetString();
            }

            return dto;
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------