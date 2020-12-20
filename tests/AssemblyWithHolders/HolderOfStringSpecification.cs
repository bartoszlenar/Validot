namespace AssemblyWithHolders
{
    using Validot;

    public class HolderOfStringSpecification : ISpecificationHolder<string>
    {
        public Specification<string> Specification { get; } = s => s
            .NotEmpty().WithMessage("Empty text not allowed")
            .MinLength(3).WithMessage("Text shorter than 3 characters not allowed")
            .MaxLength(10).WithMessage("Text longer than 10 characters not allowed")
            .NotContains("!").WithMessage("Text containing exclamation mark not allowed");
    }
}