﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="CommentCardViewModel.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.ViewModels.App.CommentCard
{
    using Microsoft.AspNetCore.Components;

    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    /// <summary>
    ///     View model for the <see cref="Client.Components.App.CommentCard" /> component
    /// </summary>
    public class CommentCardViewModel : ICommentCardViewModel
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommentCardViewModel" /> class.
        /// </summary>
        /// <param name="comment">The <see cref="Comment" /></param>
        /// <param name="currentParticipant">The logged <see cref="Participant" /></param>
        /// <param name="onContentEditCallback">The <see cref="EventCallback{TValue}" /> for content edition</param>
        /// <param name="onDeleteCallback">The <see cref="EventCallback{TValue}" /> for delete</param>
        /// <param name="onUpdateStatusCallback">The <see cref="EventCallback" /> to update the <see cref="StatusKind" /></param>
        public CommentCardViewModel(Comment comment, Participant currentParticipant,
            EventCallback<Comment> onContentEditCallback, EventCallback<Comment> onDeleteCallback, EventCallback<Comment> onUpdateStatusCallback)
        {
            this.Comment = comment;
            this.CurrentParticipant = currentParticipant;
            this.OnContentEditCallback = onContentEditCallback;
            this.OnDeleteCallback = onDeleteCallback;
            this.OnUpdateStatusCallback = onUpdateStatusCallback;
        }

        /// <summary>
        ///     The currently logged <see cref="Participant" />
        /// </summary>
        private Participant CurrentParticipant { get; set; }

        /// <summary>
        ///     Value asserting if the current <see cref="Participant" /> is allow to edit the <see cref="Comment" />
        /// </summary>
        public bool IsAllowedToEdit => this.Comment.Author.Id == this.CurrentParticipant.Id;

        /// <summary>
        ///     The <see cref="EventCallback" /> to update the status of the <see cref="ICommentCardViewModel.Comment" />
        /// </summary>
        public EventCallback<Comment> OnUpdateStatusCallback { get; set; }

        /// <summary>
        ///     The <see cref="Comment" />
        /// </summary>
        public Comment Comment { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to edit the <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnContentEditCallback { get; set; }

        /// <summary>
        ///     The <see cref="EventCallback{TValue}" /> when the user wants to delete the <see cref="Comment" />
        /// </summary>
        public EventCallback<Comment> OnDeleteCallback { get; set; }

        /// <summary>
        ///     Update the status of the <see cref="ICommentCardViewModel.Comment" />
        /// </summary>
        /// <param name="status">The new status</param>
        /// <returns>A <see cref="Task" /></returns>
        public async Task UpdateStatus(StatusKind status)
        {
            if (status != this.Comment.Status)
            {
                this.Comment.Status = status;
                await this.OnUpdateStatusCallback.InvokeAsync(this.Comment);
            }
        }
    }
}