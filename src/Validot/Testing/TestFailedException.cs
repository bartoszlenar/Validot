namespace Validot.Testing
{
    using System;

    public class TestFailedException : Exception
    {
        public TestFailedException(string message)
            : base(message)
        {
        }
    }
}
