namespace Validot.Tests.Functional.Readme
{
    using System;
    using System.Linq;

    using Xunit;

    using Validot;

    using FluentAssertions;

    public class QuickStartTest
    {
        public class UserModel
        {
            public UserModel(string email = null, string name = null, int age = 0)
            {
                Email = email;
                Name = name;
                Age = age;
            }

            public string Email { get; set; }

            public string Name { get; set; }

            public int Age { get; set; }
        }

        [Fact]
        public void QuickStart()
        {
            Specification<UserModel> specification = _ => _
                .Member(m => m.Email, m => m
                    .Email().WithExtraCode("ERR_EMAIL")
                    .MaxLength(100)
                )
                .Member(m => m.Name, m => m
                    .Optional()
                    .LengthBetween(8, 100)
                    .Rule(name => name.All(char.IsLetterOrDigit)).WithMessage("Must contain only letter or digits")
                )
                .Rule(m => m.Age >= 18 || m.Name != null)
                    .WithName("Name")
                    .WithMessage("Required for underaged user")
                    .WithExtraCode("ERR_NAME");

            var validator = Validator.Factory.Create(specification);

            var model = new UserModel(email: "inv@lidv@lue", age: 14);

            var result = validator.Validate(model);

            var messagesString = result.ToMessagesString();

            var expectedMessagesString = string.Join(Environment.NewLine, new[]
            {
                "Email: Must be a valid email address",
                "Name: Required for underaged user",
                ""
            });

            messagesString.Should().Be(expectedMessagesString);

            var codesList = result.ToCodesList();

            codesList.Should().ContainInOrder("ERR_EMAIL", "ERR_NAME");

            result.AnyErrors.Should().BeTrue();
        }
    }
}
