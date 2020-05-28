namespace Validot.Testing
{
    public sealed class TestResult
    {
        private static readonly TestResult SuccessResult = new TestResult(null);

        private TestResult(string message)
        {
            Message = message;
        }

        public bool Success => Message == null;

        public string Message { get; }

        public static TestResult Passed()
        {
            return SuccessResult;
        }

        public static TestResult Failed(string message)
        {
            return new TestResult(message);
        }

        public void ThrowExceptionIfFailed()
        {
            if (!Success)
            {
                throw new TestFailedException(Message);
            }
        }
    }
}
