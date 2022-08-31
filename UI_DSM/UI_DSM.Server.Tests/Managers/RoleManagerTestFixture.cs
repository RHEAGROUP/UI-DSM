﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="RoleManagerTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Server.Tests.Managers
{
    using Moq;

    using Npgsql;

    using NUnit.Framework;

    using UI_DSM.Server.Context;
    using UI_DSM.Server.Managers.RoleManager;
    using UI_DSM.Server.Tests.Helpers;
    using UI_DSM.Shared.Enumerator;
    using UI_DSM.Shared.Models;

    [TestFixture]
    public class RoleManagerTestFixture
    {
        private RoleManager manager;
        private Mock<DatabaseContext> context;
        private List<Role> data;
        
        [SetUp]
        public void Setup()
        {
            this.context = new Mock<DatabaseContext>();
            this.manager = new RoleManager(this.context.Object);
            
            this.data = new List<Role>()
            {
                new(Guid.NewGuid())
                {
                    RoleName = "Project administrator",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ManageParticipant, AccessRight.CreateTask, AccessRight.DeleteTask, AccessRight.UpdateTask, AccessRight.ReviewTask
                    }
                },
                new(Guid.NewGuid())
                {
                    RoleName = "Reviewer",
                    AccessRights = new List<AccessRight>()
                    {
                        AccessRight.ReviewTask
                    }
                }
            };
        }

        [Test]
        public async Task VerifyGetRoles()
        {
            var dbSet = DbSetMockHelper.CreateMock(this.data);
            var invalidGuid = Guid.NewGuid();
            dbSet.Setup(x => x.FindAsync(invalidGuid)).ReturnsAsync((Role)null);
            dbSet.Setup(x => x.FindAsync(this.data.Last().Id)).ReturnsAsync(this.data.Last());
            this.context.Setup(x => x.UiDsmRoles).Returns(dbSet.Object);
           
            Assert.Multiple(() =>
            {
               Assert.That(this.manager.GetEntities().Result.Count(), Is.EqualTo(this.data.Count));
               Assert.That(this.manager.GetEntity(invalidGuid).Result, Is.Empty);
               Assert.That(this.manager.GetEntity(this.data.Last().Id).Result, Is.Not.Null);
            });

            foreach (var role in this.data)
            {
                dbSet.Setup(x => x.FindAsync(role.Id)).ReturnsAsync(role);
            }

            var foundEntities = await this.manager.FindEntities(this.data.Select(x => x.Id));
            Assert.That(foundEntities.Count(), Is.EqualTo(this.data.Count));
        }

        [Test]
        public async Task VerifyCreateRole()
        {
            var dbSet = DbSetMockHelper.CreateMock(this.data);
            this.context.Setup(x => x.UiDsmRoles).Returns(dbSet.Object);

            var newRole = new Role()
            {
                RoleName = "Task Manager",
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.CreateTask
                }
            };

            var creationResult = await this.manager.CreateEntity(newRole);
            Assert.That(creationResult.Errors, Is.Empty);

            newRole.RoleName = this.data.First().RoleName;

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new PostgresException("unique constraint", "High", "High", "23505"));

            creationResult = await this.manager.CreateEntity(newRole);
            Assert.That(creationResult.Errors.First(), Does.Contain("already used"));

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            creationResult = this.manager.CreateEntity(newRole).Result;
            Assert.That(creationResult.Errors, Is.Not.Empty);
        }

        [Test]
        public async Task VerifyUpdateRole()
        {
            var dbSet = DbSetMockHelper.CreateMock(this.data);
            this.context.Setup(x => x.UiDsmRoles).Returns(dbSet.Object);

            var role = new Role(this.data.First().Id)
            {
                AccessRights = new List<AccessRight>()
                {
                    AccessRight.ManageParticipant
                },
                RoleName = "New Role name"
            };

            var updateResult =await this.manager.UpdateEntity(role);
            Assert.That(updateResult.Errors, Is.Empty);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new PostgresException("unique constraint", "High", "High", "23505"));

            role.RoleName = this.data.First().RoleName;
            updateResult = await this.manager.UpdateEntity(role);
            Assert.That(updateResult.Errors.First(), Does.Contain("already used"));

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            updateResult = await this.manager.UpdateEntity(role);
            Assert.That(updateResult.Errors, Is.Not.Empty);
        }

        [Test]
        public void VerifyDeleteRole()
        {
            var role = new Role();
            _ = this.manager.DeleteEntity(role).Result;

            this.context.Verify(x => x.Remove(It.IsAny<Role>()), Times.Once);

            this.context.Setup(x => x.SaveChangesAsync(default))
                .ThrowsAsync(new InvalidOperationException());

            var creationResult = this.manager.DeleteEntity(role).Result;
            Assert.That(creationResult.Succeeded, Is.False);
        }
    }
}
