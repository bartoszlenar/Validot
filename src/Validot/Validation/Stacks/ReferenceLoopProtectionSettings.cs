namespace Validot.Validation.Stacks
{
    internal class ReferenceLoopProtectionSettings
    {
        public ReferenceLoopProtectionSettings(object rootModelReference = null)
        {
            RootModelReference = rootModelReference;
        }

        public object RootModelReference { get; }
    }
}
