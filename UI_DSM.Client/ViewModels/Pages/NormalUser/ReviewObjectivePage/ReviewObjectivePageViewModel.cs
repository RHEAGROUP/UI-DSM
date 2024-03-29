﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ReviewObjectivePageViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.Pages.NormalUser.ReviewObjectivePage
{
    using DynamicData;

    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Authorization;

    using ReactiveUI;

    using UI_DSM.Client.Components.NormalUser.ReviewTask;
    using UI_DSM.Client.Services.Administration.ParticipantService;
    using UI_DSM.Client.Services.ReviewObjectiveService;
    using UI_DSM.Client.Services.ReviewTaskService;
    using UI_DSM.Client.ViewModels.Components;
    using UI_DSM.Client.ViewModels.Components.NormalUser.ReviewTask;
    using UI_DSM.Shared.DTO.Common;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="ReviewObjectivePageViewModel" /> page
    /// </summary>
    public class ReviewObjectivePageViewModel : ReactiveObject, IReviewObjectivePageViewModel
    {
        /// <summary>
        ///     The <see cref="IParticipantService" />
        /// </summary>
        private readonly IParticipantService participantService;

        /// <summary>
        ///     The <see cref="IReviewObjectiveService" />
        /// </summary>
        private readonly IReviewObjectiveService reviewObjectiveService;

        /// <summary>
        ///     The <see cref="IReviewObjectiveService" />
        /// </summary>
        private readonly IReviewTaskService reviewTaskService;

        /// <summary>
        ///     The <see cref="AuthenticationStateProvider" />
        /// </summary>
        private readonly AuthenticationStateProvider stateProvier;

        /// <summary>
        ///     Backing field for <see cref="IsOnAssignmentMode" />
        /// </summary>
        private bool isOnAssignmentMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReviewObjectivePageViewModel" /> class.
        /// </summary>
        /// <param name="reviewObjectiveService">The <see cref="IReviewObjectiveService" /></param>
        /// <param name="participantService">The <see cref="IParticipantService" /></param>
        /// <param name="reviewTaskService">The <see cref="IReviewTaskService" /></param>
        /// <param name="stateProvider">The <see cref="AuthenticationStateProvider" /></param>
        public ReviewObjectivePageViewModel(IReviewObjectiveService reviewObjectiveService, IParticipantService participantService,
            IReviewTaskService reviewTaskService, AuthenticationStateProvider stateProvider)
        {
            this.reviewObjectiveService = reviewObjectiveService;
            this.participantService = participantService;
            this.reviewTaskService = reviewTaskService;
            this.stateProvier = stateProvider;

            this.TaskAssignmentViewModel = new TaskAssignmentViewModel
            {
                OnValidSubmit = new EventCallbackFactory().Create(this, this.AssignParticipant)
            };
        }

        /// <summary>
        ///     The name of the logged user
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     A <see cref="Dictionary{Guid, AdditionalComputedProperties}" /> for the <see cref="Comment" /> count
        /// </summary>
        public Dictionary<Guid, AdditionalComputedProperties> CommentsCount { get; set; }

        /// <summary>
        ///     The <see cref="Project" /> id
        /// </summary>
        public Guid ProjectId { get; set; }

        /// <summary>
        ///     The <see cref="Review" /> id
        /// </summary>
        public Guid ReviewId { get; set; }

        /// <summary>
        ///     The <see cref="IReviewObjectivePageViewModel.ReviewObjective" /> of the page
        /// </summary>
        public ReviewObjective ReviewObjective { get; private set; }

        /// <summary>
        ///     The <see cref="Participant" />s of the project
        /// </summary>
        public List<Participant> ProjectParticipants { get; set; } = new();

        /// <summary>
        ///     Value indicating the user is currently assigning a <see cref="Participant" /> to a task
        /// </summary>
        public bool IsOnAssignmentMode
        {
            get => this.isOnAssignmentMode;
            set => this.RaiseAndSetIfChanged(ref this.isOnAssignmentMode, value);
        }

        /// <summary>
        ///     The <see cref="IErrorMessageViewModel" />
        /// </summary>
        public IErrorMessageViewModel ErrorMessageViewModel { get; } = new ErrorMessageViewModel();

        /// <summary>
        ///     The <see cref="ITaskAssignmentViewModel" />
        /// </summary>
        public ITaskAssignmentViewModel TaskAssignmentViewModel { get; private set; }

        /// <summary>
        ///     Opens the <see cref="TaskAssignment" /> as a popup
        /// </summary>
        public void OpenTaskAssignmentPopup(ReviewTask selectedReviewTask)
        {
            this.TaskAssignmentViewModel.SelectedParticipants = new List<Participant>(selectedReviewTask.IsAssignedTo);
            this.ErrorMessageViewModel.Errors.Clear();
            this.IsOnAssignmentMode = true;
            this.SelectedReviewTask = selectedReviewTask;
        }

        /// <summary>
        ///     The current <see cref="Participant" />
        /// </summary>
        public Participant Participant { get; set; } = new();

        /// <summary>
        ///     The selected <see cref="ReviewTask" />
        /// </summary>
        public ReviewTask SelectedReviewTask { get; set; } = new();

        /// <summary>
        ///     Method invoked when the component is ready to start, having received its
        ///     initial parameters from its parent in the render tree.
        ///     Override this method if you will perform an asynchronous operation and
        ///     want the component to refresh when that operation is completed.
        /// </summary>
        /// <param name="projectGuid">The <see cref="Guid" /> of the <see cref="Project" /></param>
        /// <param name="reviewGuid">The <see cref="Guid" /> of the <see cref="Review" /></param>
        /// <param name="reviewObjectiveId">The <see cref="Guid" /> of the <see cref="ReviewObjective" /></param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        public async Task OnInitializedAsync(Guid projectGuid, Guid reviewGuid, Guid reviewObjectiveId)
        {
            this.ReviewObjective = await this.reviewObjectiveService.GetReviewObjectiveOfReview(projectGuid, reviewGuid, reviewObjectiveId, 1);
            this.ProjectParticipants = (await this.participantService.GetParticipantsOfProject(projectGuid)).Where(x => x.Role.AccessRights.Contains(AccessRight.ReviewTask)).ToList();
            this.Participant = await this.participantService.GetCurrentParticipant(projectGuid);
            this.CommentsCount = await this.reviewTaskService.GetCommmentsCount(projectGuid, reviewGuid, reviewObjectiveId);

            var state = await this.stateProvier.GetAuthenticationStateAsync();

            if (state.User.Identity is { IsAuthenticated: true })
            {
                this.UserName = state.User.Identity.Name;
            }
        }

        /// <summary>
        ///     Tries to assign a <see cref="Participant" />s to a task
        /// </summary>
        /// <returns>A <see cref="Task" /></returns>
        private async Task AssignParticipant()
        {
            try
            {
                this.SelectedReviewTask.IsAssignedTo = this.TaskAssignmentViewModel.SelectedParticipants;
                var reviewTaskResult = await this.reviewTaskService.UpdateReviewTask(this.ProjectId, this.ReviewId, this.SelectedReviewTask);
                this.ErrorMessageViewModel.Errors.Clear();

                if (reviewTaskResult.Errors.Any())
                {
                    this.ErrorMessageViewModel.Errors.AddRange(reviewTaskResult.Errors);
                }

                if (reviewTaskResult.IsRequestSuccessful)
                {
                    this.TaskAssignmentViewModel.SelectedParticipants = new List<Participant>();
                }

                this.IsOnAssignmentMode = !reviewTaskResult.IsRequestSuccessful;
            }
            catch (Exception exception)
            {
                this.ErrorMessageViewModel.Errors.Clear();
                this.ErrorMessageViewModel.Errors.Add(exception.Message);
            }
        }
    }
}
