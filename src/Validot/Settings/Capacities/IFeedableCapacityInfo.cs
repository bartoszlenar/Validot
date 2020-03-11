namespace Validot.Settings.Capacities
{
    using Validot.Validation;

    public interface IFeedableCapacityInfo : ICapacityInfo
    {
        bool ShouldFeed { get; }

        void Feed(IErrorsHolder errorsHolder);
    }
}
