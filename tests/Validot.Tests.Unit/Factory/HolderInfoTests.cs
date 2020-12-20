namespace Validot.Tests.Unit.Factory
{
    using System;
    using FluentAssertions;
    using Validot.Factory;
    using Validot.Settings;
    using Validot.Testing;
    using Xunit;

    public class HolderInfoTests
    {
        [Fact]
        public void Should_Initialize()
        {
            _ = new HolderInfo(typeof(ObjectSpecificationHolder), typeof(CustomClass));
        }

        [Fact]
        public void Should_AssignTypes()
        {
            var holderInfo = new HolderInfo(typeof(ObjectSpecificationHolder), typeof(CustomClass));

            holderInfo.HolderType.Should().Be(typeof(ObjectSpecificationHolder));
            holderInfo.SpecifiedType.Should().Be(typeof(CustomClass));
        }

        [Fact]
        public void Should_ValidatorType_BeSetAs_IValidatorOfSpecifiedType()
        {
            var holderInfo = new HolderInfo(typeof(ObjectSpecificationHolder), typeof(CustomClass));

            holderInfo.ValidatorType.Should().Be(typeof(IValidator<CustomClass>));
        }

        [Fact]
        public void Should_HoldsSettings_Should_BeFalse_When_HolderIsNotSettingsHolder()
        {
            var holderInfo = new HolderInfo(typeof(ObjectSpecificationHolder), typeof(CustomClass));

            holderInfo.HoldsSettings.Should().BeFalse();
        }

        [Fact]
        public void Should_HoldsSettings_Should_BeTrue_When_HolderIsSettingsHolder()
        {
            var holderInfo = new HolderInfo(typeof(ObjectSpecificationAndSettingsHolder), typeof(CustomClass));

            holderInfo.HoldsSettings.Should().BeTrue();
        }

        [Fact]
        public void Should_ThrowException_When_HolderTypeIsNotSpecificationHolderForSpecifiedType()
        {
            Action action = () => _ = new HolderInfo(typeof(ObjectSpecificationHolder), typeof(int));

            var exception = action.Should().ThrowExactly<ArgumentException>().And;

            exception.ParamName.Should().Be("holderType");
            exception.Message.Should().StartWith("ObjectSpecificationHolder is not a holder for Int32 specification (doesn't implement ISpecificationHolder<Int32>)");
        }

        [Fact]
        public void Should_ThrowException_When_HolderTypeDoesntHaveDefaultConstructor()
        {
            Action action = () => _ = new HolderInfo(typeof(HolderWithoutDefaultConstructor), typeof(CustomClass));

            var exception = action.Should().ThrowExactly<ArgumentException>().And;

            exception.ParamName.Should().Be("holderType");
            exception.Message.Should().StartWith("HolderWithoutDefaultConstructor must have parameterless constructor.");
        }

        [Fact]
        public void Should_CreateValidator_ReturnValidatorInitializedWithSpecification()
        {
            var holderInfo = new HolderInfo(typeof(ObjectSpecificationHolder), typeof(CustomClass));

            var createdValidator = holderInfo.CreateValidator();

            createdValidator.Should().NotBeNull();
            createdValidator.Should().BeOfType<Validator<CustomClass>>();

            var validator = (Validator<CustomClass>)createdValidator;

            var case1 = new CustomClass()
            {
                CustomValue = "@b"
            };

            var case2 = new CustomClass()
            {
                CustomValue = "abcdef"
            };

            var case3 = new CustomClass()
            {
                CustomValue = "@bcdef"
            };

            var case4 = new CustomClass()
            {
                CustomValue = "a"
            };

            validator.Validate(case1).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "CustomValue: Min length is 3");

            validator.Validate(case2).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "CustomValue: Must contain @");

            validator.IsValid(case3).Should().BeTrue();

            validator.Validate(case4).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "CustomValue: Min length is 3",
                "CustomValue: Must contain @");
        }

        [Fact]
        public void Should_CreateValidator_ReturnValidatorInitializedWithSpecificationAndSettings()
        {
            var holderInfo = new HolderInfo(typeof(ObjectSpecificationAndSettingsHolder), typeof(CustomClass));

            var createdValidator = holderInfo.CreateValidator();

            createdValidator.Should().NotBeNull();
            createdValidator.Should().BeOfType<Validator<CustomClass>>();

            var validator = (Validator<CustomClass>)createdValidator;

            var case1 = new CustomClass()
            {
                CustomValue = "@b"
            };

            var case2 = new CustomClass()
            {
                CustomValue = "abcdef"
            };

            var case3 = new CustomClass()
            {
                CustomValue = "@bcdef"
            };

            var case4 = new CustomClass()
            {
                CustomValue = "a"
            };

            validator.Validate(case1).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "CustomValue: Must not contain @");

            validator.Validate(case2).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "CustomValue: MUST HAVE MAX 3 CHARACTERS");

            validator.Validate(case3).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "CustomValue: MUST HAVE MAX 3 CHARACTERS",
                "CustomValue: Must not contain @");

            validator.IsValid(case4).Should().BeTrue();
        }

        [Fact]
        public void Should_CreateValidator_ReturnValidatorInitializedWithSpecification_When_HolderHoldsMultipleSpecifications()
        {
            var intHolderInfo = new HolderInfo(typeof(MultipleSpecificationHolder), typeof(int));

            var createdIntValidator = intHolderInfo.CreateValidator();

            createdIntValidator.Should().NotBeNull();
            createdIntValidator.Should().BeOfType<Validator<int>>();

            var intValidator = (Validator<int>)createdIntValidator;

            intValidator.IsValid(5).Should().BeTrue();
            intValidator.IsValid(9).Should().BeTrue();

            intValidator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Value range 1-10 is allowed");

            intValidator.Validate(11).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Value range 1-10 is allowed");

            var stringHolderInfo = new HolderInfo(typeof(MultipleSpecificationHolder), typeof(string));

            var createdStringValidator = stringHolderInfo.CreateValidator();

            createdStringValidator.Should().NotBeNull();
            createdStringValidator.Should().BeOfType<Validator<string>>();

            var stringValidator = (Validator<string>)createdStringValidator;

            stringValidator.IsValid("abc").Should().BeTrue();
            stringValidator.IsValid("abcdefgh").Should().BeTrue();

            stringValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Length range 1-10 is allowed");

            stringValidator.Validate("01234567890987654321").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Length range 1-10 is allowed");
        }

        [Fact]
        public void Should_CreateValidator_ReturnValidatorInitializedWithSpecificationAndSettings_When_HolderHoldsMultipleSpecifications()
        {
            var intHolderInfo = new HolderInfo(typeof(MultipleSpecificationAndSettingsHolder), typeof(int));

            var createdIntValidator = intHolderInfo.CreateValidator();

            createdIntValidator.Should().NotBeNull();
            createdIntValidator.Should().BeOfType<Validator<int>>();

            var intValidator = (Validator<int>)createdIntValidator;

            intValidator.IsValid(5).Should().BeTrue();
            intValidator.IsValid(9).Should().BeTrue();

            intValidator.Validate(0).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Only values 1-10 are valid");

            intValidator.Validate(11).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Only values 1-10 are valid");

            var stringHolderInfo = new HolderInfo(typeof(MultipleSpecificationAndSettingsHolder), typeof(string));

            var createdStringValidator = stringHolderInfo.CreateValidator();

            createdStringValidator.Should().NotBeNull();
            createdStringValidator.Should().BeOfType<Validator<string>>();

            var stringValidator = (Validator<string>)createdStringValidator;

            stringValidator.IsValid("abc").Should().BeTrue();
            stringValidator.IsValid("abcdefgh").Should().BeTrue();

            stringValidator.Validate("").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Only values with length 1-10 are valid");

            stringValidator.Validate("01234567890987654321").ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Only values with length 1-10 are valid");
        }

        internal class HolderWithoutDefaultConstructor : ISpecificationHolder<CustomClass>
        {
            public HolderWithoutDefaultConstructor(string a)
            {
                _ = a;
            }

            public Specification<CustomClass> Specification { get; set; }
        }

        internal class ObjectSpecificationHolder : ISpecificationHolder<CustomClass>
        {
            public Specification<CustomClass> Specification { get; } = s => s
                .Member(m => m.CustomValue, m => m
                    .MinLength(3)
                    .WithMessage("Min length is 3")
                    .And()
                    .Contains("@")
                    .WithMessage("Must contain @"));
        }

        internal class ObjectSpecificationAndSettingsHolder : ISpecificationHolder<CustomClass>, ISettingsHolder
        {
            public Specification<CustomClass> Specification { get; set; } = s => s
                .Member(m => m.CustomValue, m => m
                    .MaxLength(3)
                    .WithMessage("Max length is 3")
                    .And()
                    .NotContains("@")
                    .WithMessage("Must not contain @"));

            public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s
                .WithTranslation("English", "Max length is 3", "MUST HAVE MAX 3 CHARACTERS");
        }

        internal class MultipleSpecificationHolder : ISpecificationHolder<int>, ISpecificationHolder<string>
        {
            Specification<int> ISpecificationHolder<int>.Specification { get; } = s => s.BetweenOrEqualTo(1, 10).WithMessage("Value range 1-10 is allowed");

            Specification<string> ISpecificationHolder<string>.Specification { get; } = s => s.LengthBetween(1, 10).WithMessage("Length range 1-10 is allowed");
        }

        internal class MultipleSpecificationAndSettingsHolder : ISpecificationHolder<int>, ISpecificationHolder<string>, ISettingsHolder
        {
            Specification<int> ISpecificationHolder<int>.Specification { get; } = s => s.BetweenOrEqualTo(1, 10).WithMessage("Value range 1-10 is allowed");

            Specification<string> ISpecificationHolder<string>.Specification { get; } = s => s.LengthBetween(1, 10).WithMessage("Length range 1-10 is allowed");

            public Func<ValidatorSettings, ValidatorSettings> Settings { get; } = s => s
                .WithTranslation("English", "Value range 1-10 is allowed", "Only values 1-10 are valid")
                .WithTranslation("English", "Length range 1-10 is allowed", "Only values with length 1-10 are valid");
        }

        internal class CustomClass
        {
            public string CustomValue { get; set; }
        }
    }
}