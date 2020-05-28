namespace Validot.Tests.Unit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FluentAssertions;

    using NSubstitute;

    using Validot.Settings;
    using Validot.Settings.Capacities;
    using Validot.Translations;
    using Validot.Validation;

    using Xunit;

    public class ValidatorTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new Validator<object>(_ => _);
        }

        [Fact]
        public void Should_ThrowException_When_NullSpecification()
        {
            Action action = () => _ = new Validator<object>(null);

            action.Should().ThrowExactly<ArgumentNullException>();
        }

        public class Settings
        {
            [Fact]
            public void Should_SetSettings_IfProvided()
            {
                var settings = new ValidatorSettings();

                settings.WithReferenceLoopProtectionDisabled();

                settings.WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["test1"] = new Dictionary<string, string>()
                    {
                        ["nested11"] = "n11",
                        ["nested12"] = "n12",
                    },
                    ["test2"] = new Dictionary<string, string>()
                    {
                        ["nested21"] = "n21",
                        ["nested22"] = "n22",
                    },
                });

                var validator = new Validator<object>(_ => _, settings);

                validator.Settings.Should().NotBeNull();

                validator.Settings.Should().BeSameAs(settings);
                validator.Settings.Translations.Should().BeSameAs(settings.Translations);
                validator.Settings.Translations.Should().HaveCount(2);
                validator.Settings.Translations["test1"].Should().HaveCount(2);
                validator.Settings.Translations["test1"]["nested11"].Should().Be("n11");
                validator.Settings.Translations["test1"]["nested12"].Should().Be("n12");
                validator.Settings.Translations["test2"].Should().HaveCount(2);
                validator.Settings.Translations["test2"]["nested21"].Should().Be("n21");
                validator.Settings.Translations["test2"]["nested22"].Should().Be("n22");
                validator.Settings.CapacityInfo.Should().BeSameAs(settings.CapacityInfo);
                validator.Settings.ReferenceLoopProtection.Should().BeFalse();
            }

            [Fact]
            public void Should_SetSettings_AsDefault_IfNotProvided()
            {
                var validator = new Validator<object>(_ => _);

                validator.Settings.Should().NotBeNull();
                validator.Settings.CapacityInfo.Should().BeOfType<DisabledCapacityInfo>();

                validator.Settings.Translations.Keys.Should().ContainSingle("English");

                var defaultEnglishTranslation = validator.Settings.Translations["English"];

                defaultEnglishTranslation.Should().NotBeSameAs(Translation.English);

                defaultEnglishTranslation.Should().HaveCount(Translation.English.Count);

                foreach (var pair in Translation.English)
                {
                    defaultEnglishTranslation[pair.Key].Should().Be(pair.Value);
                }
            }
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForErrorMap_Data), MemberType = typeof(ValidationTestData))]
        public void Should_HaveErrorMap(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldHaveErrorMap(errorCases);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForValidation_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldValidateAndHaveResult(model, false, errorCases, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForValidationWithFailFast_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate_AndFailFast(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldValidateAndHaveResult(model, true, errorCases, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForIsValid_Data), MemberType = typeof(ValidationTestData))]
        public void Should_IsValid_Return_True_If_NoErrors(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, bool expectedIsValid, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var validator = new Validator<ValidationTestData.TestClass>(specification);

            validator.ShouldHaveIsValueTrueIfNoErrors(model, expectedIsValid, exceptionCase);
        }

        [Theory]
        [MemberData(nameof(ValidationTestData.CasesForReferenceLoop_Data), MemberType = typeof(ValidationTestData))]
        public void Should_Validate_With_ReferenceLoopProtection(string name, bool referenceLoopProtectionEnabled, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases, ValidationTestData.ReferenceLoopExceptionCase exceptionCase)
        {
            _ = name;

            var settings = new ValidatorSettings();

            if (referenceLoopProtectionEnabled)
            {
                settings.WithReferenceLoopProtection();
            }
            else
            {
                settings.WithReferenceLoopProtectionDisabled();
            }

            var validator = new Validator<ValidationTestData.TestClass>(specification, settings);

            validator.ShouldValidateAndHaveResult(model, false, errorCases, exceptionCase);
        }

        [Fact]
        public void Factory_Should_NotBeNull()
        {
            Validator.Factory.Should().NotBeNull();
        }

        public class FeedingCapacityInfo
        {
            [Fact]
            public void Should_FeedCapacityInfo_After_Creation_WithDiscoveryContext()
            {
                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                _ = new Validator<ValidationTestData.TestClass>(s => s, settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);

                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());
            }

            [Fact]
            public void Should_NotFeedCapacityInfo_After_Creation_When_ShouldFeed_Is_False()
            {
                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(false);

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                _ = new Validator<ValidationTestData.TestClass>(s => s, settings);

                capacityInfo.DidNotReceiveWithAnyArgs().Feed(default);
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForErrorMap_Data), MemberType = typeof(ValidationTestData))]
            public void Should_FeedCapacityInfo_After_Creation_WithDiscoveryContext_And_ErrorMap(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
            {
                _ = name;

                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var counter = 0;

                capacityInfo
                    .When(x => x.Feed(NSubstitute.Arg.Any<DiscoveryContext>()))
                    .Do(info =>
                    {
                        var errorsHolder = info.ArgAt<IErrorsHolder>(0);

                        errorsHolder.ShouldMatchAmounts(errorCases);

                        counter++;
                    });

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                _ = new Validator<ValidationTestData.TestClass>(specification, settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);

                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                counter.Should().Be(1);
            }

            [Fact]
            public void Should_FeedCapacityInfo_After_Validating_WithValidationContext_And_ErrorsFound()
            {
                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                var validator = new Validator<ValidationTestData.TestClass>(s => s.Rule(m => false), settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                validator.Validate(new ValidationTestData.TestClass());

                capacityInfo.ReceivedWithAnyArgs(2).Feed(default);
                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<ValidationContext>());

                Received.InOrder(() =>
                {
                    capacityInfo.Received().Feed(NSubstitute.Arg.Any<DiscoveryContext>());
                    capacityInfo.Received().Feed(NSubstitute.Arg.Any<ValidationContext>());
                });
            }

            [Fact]
            public void Should_NotFeedCapacityInfo_After_Validating_When_NoErrorsFound()
            {
                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                var validator = new Validator<ValidationTestData.TestClass>(s => s.Rule(m => true), settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                validator.Validate(new ValidationTestData.TestClass());

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.DidNotReceive().Feed(NSubstitute.Arg.Any<ValidationContext>());
            }

            [Fact]
            public void Should_NotFeedCapacityInfo_After_Validating_When_ErrorsFound_And_FailFast_Is_True()
            {
                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                var validator = new Validator<ValidationTestData.TestClass>(s => s.Rule(m => false), settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                validator.Validate(new ValidationTestData.TestClass(), true);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.DidNotReceive().Feed(NSubstitute.Arg.Any<ValidationContext>());
            }

            [Theory]
            [InlineData(0)]
            [InlineData(5)]
            [InlineData(50)]
            public void Should_NotFeedCapacityInfo_After_Validating_When_ShouldFeed_Is_False(int validationsAllowed)
            {
                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                var validator = new Validator<ValidationTestData.TestClass>(s => s.Rule(m => false), settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                for (var i = 0; i < validationsAllowed + 10; ++i)
                {
                    if (i == validationsAllowed)
                    {
                        capacityInfo.ShouldFeed.Returns(false);
                    }

                    validator.Validate(new ValidationTestData.TestClass());
                }

                capacityInfo.Received(validationsAllowed).Feed(NSubstitute.Arg.Any<ValidationContext>());
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForFeed_Data), MemberType = typeof(ValidationTestData))]
            public void Should_FeedCapacityInfo_After_Validating_WithValidationContext_OnlyWhen_ErrorsFound(string name, Specification<ValidationTestData.TestClass> specification, ValidationTestData.TestClass model, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorMapCases, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorCases)
            {
                _ = name;

                var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                capacityInfo.ShouldFeed.Returns(true);

                var discoveryCounter = 0;
                var validationCounter = 0;

                capacityInfo
                    .When(x => x.Feed(NSubstitute.Arg.Any<DiscoveryContext>()))
                    .Do(info =>
                    {
                        var errorsHolder = info.ArgAt<IErrorsHolder>(0);

                        errorsHolder.ShouldMatchAmounts(errorMapCases);

                        discoveryCounter++;
                    });

                capacityInfo
                    .When(x => x.Feed(NSubstitute.Arg.Any<ValidationContext>()))
                    .Do(info =>
                    {
                        var errorsHolder = info.ArgAt<IErrorsHolder>(0);

                        errorsHolder.ShouldMatchAmounts(errorCases);

                        validationCounter++;
                    });

                var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                var validator = new Validator<ValidationTestData.TestClass>(specification, settings);

                capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                validator.Validate(model);

                discoveryCounter.Should().Be(1);

                if (errorCases is null || errorCases.Count == 0)
                {
                    validationCounter.Should().Be(0);
                    capacityInfo.DidNotReceive().Feed(NSubstitute.Arg.Any<ValidationContext>());
                    capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                }
                else
                {
                    validationCounter.Should().Be(1);
                    capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<ValidationContext>());
                    capacityInfo.ReceivedWithAnyArgs(2).Feed(default);
                }
            }

            [Theory]
            [MemberData(nameof(ValidationTestData.CasesForFeedMultipleTimes_Data), MemberType = typeof(ValidationTestData))]
            public void Should_FeedCapacityInfo_After_EachValidation_WithValidationContext_OnlyWhen_ErrorsFound(string name, Specification<ValidationTestData.TestClass> specification, IReadOnlyList<ValidationTestData.TestClass> models, IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>> errorMapCases, IReadOnlyList<IReadOnlyDictionary<string, IReadOnlyList<ValidationTestData.ErrorTestCase>>> errorCases)
            {
                 _ = name;

                 var capacityInfo = Substitute.For<IFeedableCapacityInfo>();

                 capacityInfo.ShouldFeed.Returns(true);

                 var discoveryCounter = 0;
                 int validationCounter = 0;
                 var modelIndex = 0;

                 capacityInfo
                     .When(x => x.Feed(NSubstitute.Arg.Any<DiscoveryContext>()))
                     .Do(info =>
                     {
                         var errorsHolder = info.ArgAt<IErrorsHolder>(0);

                         errorsHolder.ShouldMatchAmounts(errorMapCases);

                         discoveryCounter++;
                     });

                 capacityInfo
                     .When(x => x.Feed(NSubstitute.Arg.Any<ValidationContext>()))
                     .Do(info =>
                     {
                         var errorsHolder = info.ArgAt<IErrorsHolder>(0);

                         errorsHolder.ShouldMatchAmounts(errorCases[modelIndex]);

                         validationCounter++;
                     });

                 var settings = new ValidatorSettings().WithCapacityInfo(capacityInfo);

                 var validator = new Validator<ValidationTestData.TestClass>(specification, settings);

                 capacityInfo.ReceivedWithAnyArgs(1).Feed(default);
                 capacityInfo.Received(1).Feed(NSubstitute.Arg.Any<DiscoveryContext>());

                 for (modelIndex = 0; modelIndex < models.Count; ++modelIndex)
                 {
                     var isValid = errorCases[modelIndex] is null || errorCases[modelIndex].Count == 0;

                     var temp = validationCounter;

                     validator.Validate(models[modelIndex]);

                     if (isValid)
                     {
                         validationCounter.Should().Be(temp);
                     }
                     else
                     {
                         validationCounter.Should().Be(temp + 1);
                     }
                 }

                 discoveryCounter.Should().Be(1);

                 var casesWithErrorsCount = errorCases.Count(c => c?.Count > 0);

                 validationCounter.Should().Be(casesWithErrorsCount);
                 capacityInfo.Received(casesWithErrorsCount).Feed(NSubstitute.Arg.Any<ValidationContext>());
                 capacityInfo.ReceivedWithAnyArgs(casesWithErrorsCount + 1).Feed(default);
            }
        }
    }
}
