namespace Validot.Testing
{
    using System;

    public sealed class TestFailedException : ValidotException
    {
        public TestFailedException(string message)
            : base(message)
        {
        }
    }
}
