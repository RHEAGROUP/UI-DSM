﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramLink.cs" company="RHEA System S.A.">
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
    /// Derived class for holding data for the <see cref="Components.Widgets.DiagramLinkWidget.razor"/>
    /// </summary>
    public class DiagramLink : LinkModel, IDiagramModel
    {
        /// <summary>
        /// Gets or sets if the model has comments.
        /// </summary>
        public bool HasComments { get; set; }

        /// <summary>
        /// Creates a new instance of type <see cref="DiagramLink"/>
        /// </summary>
        /// <param name="sourcePort">the source of the link</param>
        /// <param name="targetPort">the target of the link</param>
        public DiagramLink(PortModel sourcePort, PortModel targetPort = null) : base(sourcePort, targetPort)
        {
        }
    }
}
