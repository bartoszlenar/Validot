namespace Validot.Tests.Functional.Documentation
{
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class ParameterCommandsFuncTests
    {
        [Fact]
        public void ParameterCommands()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Name, m => m.NotWhiteSpace().MaxLength(100))
                .WithCondition(m => !string.IsNullOrEmpty(m.Name))
                .WithPath("AuthorName")
                .WithCode("AUTHOR_NAME_ERROR")

                .Member(m => m.Email, m => m.Email())
                .WithMessage("Invalid email!")
                .WithExtraCode("EMAIL_ERROR")

                .Rule(m => m.Email != m.Name)
                .WithCondition(m => m.Email != null && m.Name != null)
                .WithPath("Email")
                .WithMessage("Name can't be same as Email");

            _ = Validator.Factory.Create(authorSpecification);
        }
    }
}
