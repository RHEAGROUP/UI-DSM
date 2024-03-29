﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="RelationshipRowViewModel.cs" company="RHEA System S.A.">
//  Copyright (c) 2022 RHEA System S.A.
// 
//  Author: Antoine Théate, Sam Gerené, Alex Vorobiev, Alexander van Delft, Martin Risseeuw, Nabil Abbar
// 
//  This file is part of UI-DSM.
//  The UI-DSM web application is used to review an ECSS-E-TM-10-25 model.
// 
//  The UI-DSM application is provided to the community under the Apache License 2.0.
// </copyright>
// --------------------------------------------------------------------------------------------------------

namespace UI_DSM.Client.ViewModels.Components.NormalUser.Views.RowViewModel
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="BinaryRelationship" />
    /// </summary>
    public class RelationshipRowViewModel : HaveThingRowViewModel<BinaryRelationship>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RelationshipRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="BinaryRelationship" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        /// <param name="sourceRow">The <see cref="IHaveThingRowViewModel" /> for the source</param>
        /// <param name="targetRow">The <see cref="IHaveThingRowViewModel" /> for the target</param>
        public RelationshipRowViewModel(BinaryRelationship thing, ReviewItem reviewItem,
            IHaveThingRowViewModel sourceRow, IHaveThingRowViewModel targetRow) : base(thing, reviewItem)
        {
            this.SourceRow = sourceRow;
            this.TargetRow = targetRow;
            this.SourceKind = $"Source {this.GetRowKind(sourceRow)}";
            this.TargetKind = $"Target {this.GetRowKind(targetRow)}";
        }

        /// <summary>
        /// Gets the source kind
        /// </summary>
        public string SourceKind { get; }

        /// <summary>
        /// Gets the target kind
        /// </summary>
        public string TargetKind { get; }

        /// <summary>
        /// Gets the row kind of the <see cref="IHaveThingRowViewModel"/>
        /// </summary>
        /// <param name="row">The <see cref="IHaveThingRowViewModel"/></param>
        /// <returns>The row kind</returns>
        private string GetRowKind(IHaveThingRowViewModel row)
        {
	        return row.GetType().Name.Split("Row")[0];
        }

        /// <summary>
        ///     The <see cref="IHaveThingRowViewModel" /> for the target
        /// </summary>
        public IHaveThingRowViewModel TargetRow { get; }

        /// <summary>
        ///     The <see cref="IHaveThingRowViewModel" /> for the source
        /// </summary>
        public IHaveThingRowViewModel SourceRow { get; }

        /// <summary>
        ///     Gets the Id of the current <see cref="RelationshipRowViewModel" />
        /// </summary>
        public override string Id => $"{this.SourceRow.Id} -> {this.TargetRow.Id}";

        /// <summary>
        ///     The <see cref="Guid" /> of the Source <see cref="Thing" />
        /// </summary>
        public Guid SourceId => this.Thing.Source.Iid;

        /// <summary>
        ///     The <see cref="Guid" /> of the Target <see cref="Thing" />
        /// </summary>
        public Guid TargetId => this.Thing.Target.Iid;

        /// <summary>
        ///     The name of the source <see cref="Thing" />
        /// </summary>
        public string SourceName => this.Thing.Source.GetName();

        /// <summary>
        ///     The name of the Target <see cref="Thing" />
        /// </summary>
        public string TargetName => this.Thing.Target.GetName();

        /// <summary>
        ///     The name of the Source owner
        /// </summary>
        public string SourceOwner => this.Thing.Source.GetOwnerShortName();

        /// <summary>
        ///     The name of the Target owner
        /// </summary>
        public string TargetOwner => this.Thing.Target.GetOwnerShortName();
    }
}
