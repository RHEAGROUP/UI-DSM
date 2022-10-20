﻿// --------------------------------------------------------------------------------------------------------
// <copyright file="ThingExtensionTestFixture.cs" company="RHEA System S.A.">
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

namespace UI_DSM.Client.Tests.Extension
{
    using CDP4Common.CommonData;
    using CDP4Common.EngineeringModelData;
    using CDP4Common.SiteDirectoryData;
    using CDP4Common.Types;

    using CDP4Dal;

    using NUnit.Framework;

    using UI_DSM.Client.Extensions;

    [TestFixture]
    public class ThingExtensionTestFixture
    {
        [Test]
        public void VerifyGetSimpleParameterValue()
        {
            var requirement = new Requirement()
            {
                ParameterValue =
                {
                    new SimpleParameterValue()
                    {
                        ParameterType = new TextParameterType()
                        {
                            Name = "Justification"
                        },
                        Value = new ValueArray<string>(new List<string>{"A justificiation"})
                    },
                    new SimpleParameterValue()
                    {
                        ParameterType = new TextParameterType()
                        {
                            Name = "type"
                        },
                    }
                }
            };

            Assert.Multiple(() =>
            {
                Assert.That(requirement.GetSimpleParameterValue("justification"), Is.Not.Empty);
                Assert.That(requirement.GetSimpleParameterValue("type"), Is.Null);
                Assert.That(requirement.GetSimpleParameterValue("something"), Is.Null);
            });
        }

        [Test]
        public void VerifyGetCategories()
        {
            var categories = new List<Category>
            {
                new ()
                {
                    IsDeprecated = true,
                    Name = "DeprecatedCategory"
                },
                new ()
                {
                    Name = "UndeprecatedCategory"
                }
            };

            var requirement = new Requirement()
            {
                Category = categories
            };

            Assert.Multiple(() =>
            {
                Assert.That(requirement.GetCategories().ToList(), Has.Count.EqualTo(1));
                Assert.That(requirement.GetCategories(true).ToList(), Has.Count.EqualTo(2));
            });
        }

        [Test]
        public void VerifyGetParametricConstraints()
        {
            var requirement = new Requirement()
            {
                ParametricConstraint =
                {
                    new ParametricConstraint()
                    {
                        Expression =
                        {
                            new AndExpression(),
                            new AndExpression(),
                        }
                    },
                    new ParametricConstraint()
                    {
                        Expression =
                        {
                            new OrExpression()
                        }
                    }
                }
            };

            Assert.That(requirement.GetParametricConstraints().ToList(), Has.Count.EqualTo(3));
        }

        [Test]
        public void VerifyGetOwner()
        {
            var requirement = new Requirement()
            {
                Owner = new DomainOfExpertise()
                {
                    ShortName = "SYS"
                }
            };

            Assert.That(requirement.GetOwnerShortName(), Is.EqualTo("SYS"));
        }

        [Test]
        public void VerifyGetSingleDefinition()
        {
            var requirement = new Requirement();
            const string englishContent = "A content";
            const string frenchContent = "Le contenu d'une définition";
            Assert.That(requirement.GetSingleDefinition(), Is.Null);

            requirement.Definition.Add(new Definition()
            {
                LanguageCode = "en",
                Content = englishContent
            });

            requirement.Definition.Add(new Definition()
            {
                LanguageCode = "fr",
                Content = frenchContent
            });

            Assert.Multiple(() =>
            {
                Assert.That(requirement.GetSingleDefinition(), Is.EqualTo(englishContent));
                Assert.That(requirement.GetSingleDefinition("fr"), Is.EqualTo(frenchContent));
                Assert.That(requirement.GetSingleDefinition("nl"), Is.EqualTo(englishContent));
            });
        }

        [Test]
        public async Task VerifyGetRelatedThingName()
        {
            var categories = new List<CDP4Common.DTO.Category>
            {
                new(Guid.NewGuid(), 0)
                {
                    IsDeprecated = true,
                    Name = "derives"
                },
                new(Guid.NewGuid(), 0)
                {
                    Name = "derives"
                },
                new(Guid.NewGuid(), 0)
                {
                    IsDeprecated = true,
                    Name = "req"
                },
                new(Guid.NewGuid(), 0)
                {
                    Name = "req"
                }
            };

            var reqCategory = categories.Where(x => x.Name == "req").Select(x => x.Iid).ToList();
            var relationCategory = categories.Where(x => x.Name == "derives").Select(x => x.Iid).ToList();

            var requirementSourceDto = new CDP4Common.DTO.Requirement(Guid.NewGuid(), 0)
            {
                Name = "1",
                Category = reqCategory
            };

            var requirementTargetDto = new CDP4Common.DTO.Requirement(Guid.NewGuid(), 0)
            {
                Name = "2",
                Category = reqCategory
            };

            var relationShipDto = new CDP4Common.DTO.BinaryRelationship(Guid.NewGuid(),0)
            {
                Category = relationCategory,
                Source = requirementSourceDto.Iid,
                Target = requirementTargetDto.Iid
            };

            var assembler = new Assembler(new Uri("http://localhost"));
            
            var things = new List<CDP4Common.DTO.Thing>(categories)
            {
                relationShipDto,
                requirementSourceDto,
                requirementTargetDto
            };

            await assembler.Synchronize(things);
            
            var requirements = assembler.Cache.Values
                .Where(x => x.Value.ClassKind == ClassKind.Requirement)
                .Select(x => x.Value)
                .ToList();

            Assert.Multiple(() =>
            {
                Assert.That(requirements[0].GetRelatedThingName("de", ClassKind.Requirement), Is.Empty);
                Assert.That(requirements[0].GetRelatedThingName("derives", ClassKind.ElementDefinition), Is.Empty);
                Assert.That(requirements[0].GetRelatedThingName("derives", ClassKind.Requirement), Is.EqualTo(requirementTargetDto.Name));
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement), Is.Empty);
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, false), Is.EqualTo(requirementSourceDto.Name));
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, false), Is.EqualTo(requirementSourceDto.Name));
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, false), Is.EqualTo(requirementSourceDto.Name));
                Assert.That(requirements[0].GetRelatedThingName("derives", ClassKind.ElementDefinition, "req"), Is.Empty);
                Assert.That(requirements[0].GetRelatedThingName("derives", ClassKind.Requirement, "req"), Is.EqualTo(requirementTargetDto.Name));
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, "req"), Is.Empty);
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, "req", false), Is.EqualTo(requirementSourceDto.Name));
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, "req", false), Is.EqualTo(requirementSourceDto.Name));
                Assert.That(requirements[1].GetRelatedThingName("derives", ClassKind.Requirement, "req", false), Is.EqualTo(requirementSourceDto.Name));
            });
        }
    }
}
