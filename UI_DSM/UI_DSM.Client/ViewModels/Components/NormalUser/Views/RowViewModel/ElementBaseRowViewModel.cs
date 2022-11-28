﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ElementBaseRowViewModel.cs" company="RHEA System S.A.">
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
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;

    using UI_DSM.Client.Extensions;
    using UI_DSM.Shared.Extensions;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     Row view model to display content for a <see cref="ElementBase" />
    /// </summary>
    public class ElementBaseRowViewModel : HaveThingRowViewModel<ElementBase>
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ElementBaseRowViewModel" /> class.
        /// </summary>
        /// <param name="thing">The <see cref="ElementBase" /></param>
        /// <param name="reviewItem">The associated <see cref="HaveThingRowViewModel{TThing}.ReviewItem" /></param>
        public ElementBaseRowViewModel(ElementBase thing, ReviewItem reviewItem) : base(thing, reviewItem)
        {
            this.InitializesProperties();
        }

        /// <summary>
        ///     Gets the name of the <see cref="ElementDefinition" />
        /// </summary>
        public string Name => this.Thing.Name;

        /// <summary>
        ///     Gets the shortname of the <see cref="ElementDefinition" />
        /// </summary>
        public string ShortName => this.Thing.ShortName;

        /// <summary>
        ///     Gets the owner of the <see cref="ElementDefinition" />
        /// </summary>
        public string Owner => this.Thing.GetOwnerShortName();

        /// <summary>
        ///     A collection of all applied <see cref="Category" />
        /// </summary>
        public IEnumerable<string> Categories => this.Thing.GetCategories();

        /// <summary>
        ///     The collection of <see cref="Category" /> separated by a ','
        /// </summary>
        public string CommaCategories { get; private set; }

        /// <summary>
        ///     A collection of pair of <see cref="string" /> to identify the <see cref="Parameter" /> name and value
        /// </summary>
        public List<(string, string)> ParameterValues { get; private set; }

        /// <summary>
        ///     The name of the container of the <see cref="ElementBase" />
        /// </summary>
        public string Container => this.Thing.GetElementContainer();

        /// <summary>
        ///     The <see cref="Guid" /> of the container of the <see cref="ElementBase" />
        /// </summary>
        public Guid ContainerId => this.Thing.Container?.Iid ?? Guid.Empty;

        /// <summary>
        ///     Gets the <see cref="Guid" /> of the relation <see cref="ElementDefinition" />
        /// </summary>
        public Guid ElementDefinitionId { get; private set; }

        /// <summary>
        ///     Gets the Id of the current <see cref="IHaveThingRowViewModel" />
        /// </summary>
        public override string Id => this.Thing.Name;

        /// <summary>
        ///     Verifies that the current <see cref="ElementBase" /> has the current <see cref="Option" /> has excluded
        /// </summary>
        /// <param name="option">The <see cref="Option" /></param>
        /// <returns>True if the <see cref="Option" /> is excluded</returns>
        public bool HasOptionExcluded(Option option)
        {
            return this.Thing is ElementUsage usage && usage.ExcludeOption.Any(x => x.Iid == option.Iid);
        }

        /// <summary>
        ///     Initializes this row view model properties
        /// </summary>
        private void InitializesProperties()
        {
            this.ParameterValues = this.Thing.GetAllParameterValues(null, null);
            this.ElementDefinitionId = this.Thing is ElementUsage usage ? usage.ElementDefinition.Iid : this.ThingId;
            this.CommaCategories = this.Categories.AsCommaSeparated();
        }
    }
}
