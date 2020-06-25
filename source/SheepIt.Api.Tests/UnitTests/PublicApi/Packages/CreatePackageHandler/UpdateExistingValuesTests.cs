using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SheepIt.Api.Model.Packages;

namespace SheepIt.Api.Tests.UnitTests.PublicApi.Packages.CreatePackageHandler
{
    public class UpdateExistingValuesTests
    {
        [Test]
        public void Should_Add_New_Variable()
        {
            // PREPARE
            var existingVariables = new VariableValues[0];

            var newVariables = new[]
            {
                new VariableValues
                {
                    Name = "NewValue1",
                    DefaultValue = "DefaultValue1",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key", "value" }
                    }
                }
            };
            
            // RUN
            var result = Api.PublicApi.Packages.CreatePackageHandler
                .UpdateExistingValues(existingVariables, newVariables);

            // ASSERT
            result.Should().BeEquivalentTo(newVariables);
        }
        
        [Test]
        public void Should_Left_Previous_Variable()
        {
            // PREPARE
            var existingVariables = new[]
            {
                new VariableValues
                {
                    Name = "PreviousValue1",
                    DefaultValue = "DefaultValue1",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key", "value" }
                    }
                }
            };

            var newVariables = new VariableValues[0];

            // RUN
            var result = Api.PublicApi.Packages.CreatePackageHandler
                .UpdateExistingValues(existingVariables, newVariables);

            // ASSERT
            result.Should().BeEquivalentTo(existingVariables);
        }
        
        [Test]
        public void Should_Concat_New_And_Previous_Variable()
        {
            // PREPARE
            var existingVariables = new[]
            {
                new VariableValues
                {
                    Name = "PreviousValue1",
                    DefaultValue = "DefaultValue1",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key1", "value1" }
                    }
                }
            };

            var newVariables = new[]
            {
                new VariableValues
                {
                    Name = "PreviousValue2",
                    DefaultValue = "DefaultValue2",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key2", "value2" }
                    }
                }
            };

            // RUN
            var result = Api.PublicApi.Packages.CreatePackageHandler
                .UpdateExistingValues(existingVariables, newVariables);

            // ASSERT
            var expected = new[]
            {
                new VariableValues
                {
                    Name = "PreviousValue1",
                    DefaultValue = "DefaultValue1",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key1", "value1" }
                    }
                },
                new VariableValues
                {
                    Name = "PreviousValue2",
                    DefaultValue = "DefaultValue2",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key2", "value2" }
                    }
                }
            };
            
            result.Should().BeEquivalentTo(expected);
        }
        
        [Test]
        public void Should_Merge_Variables_And_Values()
        {
            // PREPARE
            var existingVariables = new[]
            {
                new VariableValues
                {
                    Name = "NewValue1",
                    DefaultValue = "DefaultValue1",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key0", "value0" },
                        { "key1", "value1" }
                    }
                }
            };

            var newVariables = new[]
            {
                new VariableValues
                {
                    Name = "NewValue1",
                    DefaultValue = "DefaultValue2",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key1", "value1New" },
                        { "key2", "value2" }
                    }
                }
            };
            
            // RUN
            var result = Api.PublicApi.Packages.CreatePackageHandler
                .UpdateExistingValues(existingVariables, newVariables);

            // ASSERT
            var expected = new[]
            {
                new VariableValues
                {
                    Name = "NewValue1",
                    DefaultValue = "DefaultValue2",
                    ActualEnvironmentValues = new Dictionary<string, string>
                    {
                        { "key0", "value0" },
                        { "key1", "value1New" },
                        { "key2", "value2" }
                    }
                }
            };
            
            result.Should().BeEquivalentTo(expected);
        }
    }
}