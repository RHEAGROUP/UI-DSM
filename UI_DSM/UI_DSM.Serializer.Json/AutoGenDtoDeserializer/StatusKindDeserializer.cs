// --------------------------------------------------------------------------------------------------------
// <copyright file="StatusKindDeserializer.cs" company="RHEA System S.A.">
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
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    ///     The purpose of the <see cref="StatusKindDeserializer" /> is to provide deserialization capabilities for enum
    /// </summary>
    internal static class StatusKindDeserializer
    {
        /// <summary>
        ///     Deserializes a string value to a <see cref="StatusKind"/>
        /// </summary>
        /// <param name="value">The string representation of the <see cref="StatusKind"/></param>
        /// <returns>The value of the <see cref="StatusKind"/></returns>
        internal static StatusKind Deserialize(string value)
        {
            return value switch
            {
                "OPEN" => StatusKind.Open,
                "DONE" => StatusKind.Done,
                "CLOSED" => StatusKind.Closed,
                _ => throw new ArgumentException($"{value} is not a valid StatusKind", nameof(value))
            };
        }
    }
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------