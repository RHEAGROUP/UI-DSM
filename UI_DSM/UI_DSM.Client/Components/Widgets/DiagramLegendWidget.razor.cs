﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="DiagramLegendWidget.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.Widgets
{
    using Microsoft.AspNetCore.Components;

	/// <summary>
	/// Partial class for the component <see cref="DiagramLegendWidget"/>
	/// </summary>
    public partial class DiagramLegendWidget
	{
		/// <summary>
		/// Gets or sets if the legend is expaned
		/// </summary>
		[Parameter]
		public bool IsExpanded { get; set; }		
	}
}
