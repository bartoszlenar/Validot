namespace AssemblyWithHolders
{
    using System;

    using Validot;

    public class HolderOfMultipleSpecifications : ISpecificationHolder<DateTime>, ISpecificationHolder<DateTimeOffset>
    {
        Specification<DateTime> ISpecificationHolder<DateTime>.Specification { get; } = s => s
            .AfterOrEqualTo(new DateTime(2000, 1, 1), TimeComparison.JustDate).WithMessage("Dates after 1st of Jan'00 are allowed")
            .Before(new DateTime(2021, 1, 1), TimeComparison.JustDate).WithMessage("Dates before 1st of Jan'21 are allowed");

        Specification<DateTimeOffset> ISpecificationHolder<DateTimeOffset>.Specification { get; } = s => s
            .AfterOrEqualTo(new DateTimeOffset(2000, 1, 1, 1, 1, 1, TimeSpan.Zero)).WithMessage("Dates after midnight 1st of Jan'00 are allowed")
            .Before(new DateTimeOffset(2021, 1, 1, 1, 1, 1, TimeSpan.Zero)).WithMessage("Dates before midnight 1st of Jan'21 are allowed");
    }
}