namespace Validot.Validation.Stacks
{
    public class ReferenceLoopProtectionSettings
    {
        public ReferenceLoopProtectionSettings(object rootModelReference = null)
        {
            RootModelReference = rootModelReference;
        }

        public object RootModelReference { get; }
    }
}
