namespace Validot.Specification.Commands
{
    using System;

    internal class WithPathCommand : ICommand
    {
        public WithPathCommand(string path)
        {
            ThrowHelper.NullArgument(path, nameof(path));

            if (!PathHelper.IsValidAsPath(path))
            {
                throw new ArgumentException("Invalid path", nameof(path));
            }

            Path = path;
        }

        public string Path { get; }
    }
}
