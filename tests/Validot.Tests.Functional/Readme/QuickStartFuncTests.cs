namespace Validot.Tests.Functional.Readme
{
    using System.Linq;
    using FluentAssertions;
    using Validot;
    using Validot.Testing;
    using Xunit;

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
                    .Email()
                    .WithExtraCode("ERR_EMAIL")
                    .And()
                    .MaxLength(100)
                )
                .Member(m => m.Name, m => m
                    .Optional()
                    .And()
                    .LengthBetween(8, 100)
                    .And()
                    .Rule(name => name.All(char.IsLetterOrDigit))
                    .WithMessage("Must contain only letter or digits")
                )
                .And()
                .Rule(m => m.Age >= 18 || m.Name != null)
                .WithPath("Name")
                .WithMessage("Required for underaged user")
                .WithExtraCode("ERR_NAME");

            var validator = Validator.Factory.Create(specification);

            var model = new UserModel(email: "inv@lidv@lue", age: 14);

            var result = validator.Validate(model);

            result.AnyErrors.Should().BeTrue();

            result.MessageMap["Email"].Single().Should().Be("Must be a valid email address");

            result.Codes.Should().Contain("ERR_EMAIL", "ERR_NAME");

            var messagesString = result.ToString();

            messagesString.ShouldResultToStringHaveLines(
                ToStringContentType.MessagesAndCodes,
                "ERR_EMAIL, ERR_NAME",
                "",
                "Email: Must be a valid email address",
                "Name: Required for underaged user");
        }
    }
}
