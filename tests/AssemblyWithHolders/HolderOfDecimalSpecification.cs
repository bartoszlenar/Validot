namespace AssemblyWithHolders
{
    using Validot;

    public class HolderOfDecimalSpecification : ISpecificationHolder<decimal>
    {
        public Specification<decimal> Specification { get; } = s => s
            .GreaterThanOrEqualTo(1).WithMessage("Min value is 1")
            .LessThanOrEqualTo(10).WithMessage("Max value is 10");
    }
}