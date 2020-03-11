namespace Validot.Testing
{
    public class TestResult
    {
        private static readonly TestResult SuccessResult = new TestResult(null);

        private TestResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public bool Success => ErrorMessage == null;

        public string ErrorMessage { get; }

        public static TestResult Passed()
        {
            return SuccessResult;
        }

        public static TestResult Failed(string errorMessage)
        {
            return new TestResult(errorMessage);
        }

        public void ThrowExceptionIfFailed()
        {
            if (!Success)
            {
                throw new TestFailedException(ErrorMessage);
            }
        }
    }
}
