namespace Validot.Tests.Functional.Documentation
{
    using Validot.Tests.Functional.Documentation.Models;

    using Xunit;

    public class ScopeCommandsFuncTests
    {
        [Fact]
        public void ScopeCommands()
        {
            Specification<AuthorModel> authorSpecification = s => s
                .Member(m => m.Name, m => m.NotWhiteSpace().MaxLength(100))
                .Member(m => m.Email, m => m.Email())
                .Rule(m => m.Email != m.Name);

            _ = Validator.Factory.Create(authorSpecification);
        }
    }
}
