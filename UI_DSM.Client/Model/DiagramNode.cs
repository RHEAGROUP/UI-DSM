﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramNode.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar, Jaime Bernar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.Model
{
    using Blazor.Diagrams.Core.Models;

    /// <summary>
    ///     Derived class for holding data for the <see cref="Components.Widgets.DiagramNodeWidget" />
    /// </summary>
    public class DiagramNode : NodeModel, IDiagramModel
    {
        /// <summary>
        ///     Gets or sets the id of the <see cref="DiagramNode" />
        /// </summary>
        public Guid ThingId { get; set; }

        /// <summary>
        ///     Gets or sets if the node has been expanded
        /// </summary>
        public bool IsExpanded { get; set; }

        /// <summary>
        ///     Gets or sets the visibility of the node
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        ///     Gets or sets if the model has comments.
        /// </summary>
        public bool HasComments { get; set; }

        /// <summary>
        /// Value asserting that the element is selected
        /// </summary>
        public bool IsSelected { get; set; }
    }
}
