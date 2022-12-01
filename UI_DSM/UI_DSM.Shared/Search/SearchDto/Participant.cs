// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Participant.cs" company="RHEA System S.A.">
// Copyright (c) 2022 RHEA System S.A.
//
// Author: RHEA System S.A.
//
// The current software is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3 of the License, or (at your option) any later version.
//
// The current software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program; if not, write to the Free Software Foundation,
// Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------

namespace UI_DSM.Shared.Search.SearchDto
{
	using System;
	using System.Collections.Generic;
	using System.Text.Json.Serialization;
	using UI_DSM.Shared.Search;
	using UI_DSM.Shared.Search.SearchDto;
	using GP.SearchService.SDK.Definitions;

	/// <summary>
	/// The <see cref="Participant" /> is the <see cref="ISearchDto" /> for the Poco of the same name
	/// </summary>
	[SearchDto(nameof(Participant))]
	public partial class Participant : BaseSearchDto
	{

		/// <summary>
		/// Gets or sets the <see cref="UserEntity"/> <see cref="User"/>
		/// </summary>
		public UserEntity User { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="string"/> <see cref="ParticipantName"/>
		/// </summary>
		public string ParticipantName { get; set; }

		/// <summary>
		/// Gets or sets the <see cref="Role"/> <see cref="Role"/>
		/// </summary>
		public Role Role { get; set; }
	}
}

// ------------------------------------------------------------------------------------------------
// --------THIS IS AN AUTOMATICALLY GENERATED FILE. ANY MANUAL CHANGES WILL BE OVERWRITTEN!--------
// ------------------------------------------------------------------------------------------------
