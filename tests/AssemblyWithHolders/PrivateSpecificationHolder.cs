namespace AssemblyWithHolders
{
    using Validot;

    internal class PrivateSpecificationHolder : ISpecificationHolder<string>
    {
        public Specification<string> Specification { get; } = s => s.NotEmpty();
    }
}