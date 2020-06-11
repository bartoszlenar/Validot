namespace Validot.Tests.Functional.Documentation
{
    using System;
    using System.Collections.Generic;

    using FluentAssertions;

    using Validot.Settings;
    using Validot.Testing;
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class SettingsFuncTests
    {
        [Fact]
        public void Settings_ModificationViaMethod()
        {
            var settings = new ValidatorSettings();

            settings.ReferenceLoopProtection.Should().BeNull();

            settings.WithReferenceLoopProtection();

            settings.ReferenceLoopProtection.Should().BeTrue();
        }

        [Fact]
        public void Settings_Factory()
        {
            Specification<object> specification = s => s;

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithReferenceLoopProtection()
            );

            validator.Settings.ReferenceLoopProtection.Should().BeTrue();
        }

        [Fact]
        public void Settings_ChangingAfterCreation()
        {
            Specification<object> specification = s => s;

            var validator = Validator.Factory.Create(specification, settings => settings
                .WithReferenceLoopProtection()
            );

            validator.Settings.ReferenceLoopProtection.Should().BeTrue();

            Action action = () =>
            {
                validator.Settings.WithReferenceLoopProtectionDisabled();
            };

            action.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void WithTranslation()
        {
            var validator = Validator.Factory.Create<object>(s => s, settings => settings
                .WithTranslation("English", "Global.Error", "Error found")
                .WithTranslation("English", "Global.Required", "Value is required")
                .WithTranslation("Polish", "Global.Required", "Wartość wymagana")
            );

            validator.Settings.Translations["English"]["Global.Error"].Should().Be("Error found");
            validator.Settings.Translations["English"]["Global.Required"].Should().Be("Value is required");
            validator.Settings.Translations["Polish"]["Global.Required"].Should().Be("Wartość wymagana");
        }

        [Fact]
        public void WithTranslation_FullDictionary()
        {
            var validator = Validator.Factory.Create<object>(s => s, settings => settings
                .WithTranslation(new Dictionary<string, IReadOnlyDictionary<string, string>>()
                {
                    ["English"] = new Dictionary<string, string>()
                    {
                        ["Global.Error"] = "Error found",
                        ["Global.Required"] = "Value is required",
                    },
                    ["Polish"] = new Dictionary<string, string>()
                    {
                        ["Global.Required"] = "Wartość wymagana",
                    }
                })
            );

            validator.Settings.Translations["English"]["Global.Error"].Should().Be("Error found");
            validator.Settings.Translations["English"]["Global.Required"].Should().Be("Value is required");
            validator.Settings.Translations["Polish"]["Global.Required"].Should().Be("Wartość wymagana");
        }

        [Fact]
        public void WithTranslation_Dictionaries()
        {
            var validator = Validator.Factory.Create<object>(s => s, settings => settings
                .WithTranslation("English", new Dictionary<string, string>()
                {
                    ["Global.Error"] = "Error found",
                    ["Global.Required"] = "Value is required",
                })
                .WithTranslation("Polish", new Dictionary<string, string>()
                {
                    ["Global.Required"] = "Wartość wymagana",
                })
            );

            validator.Settings.Translations["English"]["Global.Error"].Should().Be("Error found");
            validator.Settings.Translations["English"]["Global.Required"].Should().Be("Value is required");
            validator.Settings.Translations["Polish"]["Global.Required"].Should().Be("Wartość wymagana");
        }

        [Fact]
        public void WithTranslation_OverwriteDefaults()
        {
            Specification<AuthorModel> specification = s => s
                .Member(m => m.Email, m => m
                    .NotEmpty()
                    .Email()
                )
                .Member(m => m.Name, m => m
                    .Required().WithMessage("Name is required")
                );

            var author = new AuthorModel()
            {
                Email = ""
            };

            var validator1 = Validator.Factory.Create(specification);

            validator1.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Email: Must not be empty",
                "Email: Must be a valid email address",
                "Name: Name is required");

            var validator2 = Validator.Factory.Create(specification, settings => settings
                .WithTranslation("English", "Name is required", "You must fill out the name")
                .WithTranslation("English", "Texts.NotEmpty", "Text value cannot be empty")
            );

            validator2.Validate(author).ToString().ShouldResultToStringHaveLines(
                ToStringContentType.Messages,
                "Email: Text value cannot be empty",
                "Email: Must be a valid email address",
                "Name: You must fill out the name");
        }
    }
}
