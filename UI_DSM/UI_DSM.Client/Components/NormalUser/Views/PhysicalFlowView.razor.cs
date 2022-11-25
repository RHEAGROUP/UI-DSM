﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="PhysicalFlowView.razor.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Components.NormalUser.Views
{
    using System.Threading.Tasks;

    using Blazor.Diagrams.Core;
    using Blazor.Diagrams.Core.Models;
    using Blazor.Diagrams.Core.Models.Base;

    using Microsoft.AspNetCore.Components.Web;

    using UI_DSM.Client.Components.Widgets;
    using UI_DSM.Client.Model;
    using UI_DSM.Client.ViewModels.Components.NormalUser.Views;
    using UI_DSM.Shared.Enumerator;

    /// <summary>
    /// Component for the <see cref="View.PhysicalFlowView"/>
    /// </summary>
    public partial class PhysicalFlowView : GenericBaseView<IInterfaceViewViewModel>, IReusableView, IDisposable
    {
        /// <summary>
        /// Gets or sets the diagram component.
        /// </summary>
        public Diagram Diagram { get; set; }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            this.Diagram = new Diagram();
            this.Diagram.Options.AllowMultiSelection = false;
            this.Diagram.Options.DefaultNodeComponent = typeof(DiagramNodeWidget);
            this.Diagram.RegisterModelComponent<DiagramLink, DiagramLinkWidget>();                       

            this.Diagram.MouseUp += Diagram_MouseUp;
            this.Diagram.MouseDoubleClick += Diagram_MouseDoubleClick;
        }

        /// <summary>
        /// MouseUp event for the diagram component
        /// </summary>
        /// <param name="model">the model clicked (NodeModel,PortModel or LinkModel)</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseUp(Model model, MouseEventArgs args)
        {
            //Right button
            if (args.Button == 0)
            {
                this.ViewModel.SetSelectedModel(model);
            }
        }

        /// <summary>
        /// MouseDoubleClick event for the diagram component
        /// </summary>
        /// <param name="model">the model clicked (NodeModel,PortModel or LinkModel)</param>
        /// <param name="args">the args of the event</param>
        private void Diagram_MouseDoubleClick(Model model, MouseEventArgs args)
        {
            //Right button
            if (args.Button == 0 && model is NodeModel nodeModel)
            {
                this.ViewModel.SetCentralNodeModel(nodeModel);
            }
        }

        /// <summary>
        ///     Tries to copy components from another <see cref="BaseView" />
        /// </summary>
        /// <param name="otherView">The other <see cref="BaseView" /></param>
        /// <returns>Value indicating if it could copy components</returns>
        public async Task<bool> CopyComponents(BaseView otherView)
        {
            this.ViewModel.OnCentralNodeChanged -= this.OnCentralNodeChanged;
            if (otherView is not GenericBaseView<IInterfaceViewViewModel> interfaceView)
            {
                return false;
            }

            this.ViewModel = interfaceView.ViewModel;
            await this.HasChanged();

            var firstCenterProduct = this.ViewModel.SelectedFirstProductByCloserSelectedItem(this.ViewModel.SelectedElement);
            this.ViewModel.CreateCentralNodeAndNeighbours(firstCenterProduct);
            this.OnCentralNodeChanged();

            this.ViewModel.OnCentralNodeChanged += this.OnCentralNodeChanged;

            return true;
        }

        /// <summary>
        /// Method that handles the on central node changed event.
        /// </summary>
        public void OnCentralNodeChanged()
        {
            this.Diagram.Links.Clear();
            this.Diagram.Nodes.Clear();
            this.ViewModel.ProductNodes.ForEach(node => this.Diagram.Nodes.Add(node));
            this.ViewModel.InterfacesLinks.ForEach(link => this.Diagram.Links.Add(link));
            this.StateHasChanged();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Diagram.MouseUp -= this.Diagram_MouseUp;
            this.Diagram.MouseDoubleClick -= this.Diagram_MouseDoubleClick;
            this.ViewModel.OnCentralNodeChanged -= this.OnCentralNodeChanged;
        }
    }
}
