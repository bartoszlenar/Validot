namespace Validot.Tests.Functional.Documentation
{
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class PresenceCommandsFuncTests
    {
        [Fact]
        public void PresenceCommands()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Optional()
                .Member(m => m.Name, m => m
                    .Optional()
                    .NotWhiteSpace()
                    .MaxLength(100)
                )
                .Member(m => m.Email, m => m
                    .Required().WithMessage("Email is obligatory.")
                    .Email()
                )
                .Rule(m => m.Email != m.Name);

            _ = Validator.Factory.Create(authorSpecification);
        }
    }
}
