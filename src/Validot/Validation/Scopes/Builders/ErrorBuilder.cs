namespace Validot.Validation.Scopes.Builders
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Validot.Errors;
    using Validot.Errors.Args;
    using Validot.Specification.Commands;

    internal class ErrorBuilder
    {
        private readonly IReadOnlyList<IArg> _args;

        private readonly List<string> _codes = new List<string>();

        private readonly List<string> _messages = new List<string>();

        public ErrorBuilder()
        {
        }

        public ErrorBuilder(string key, IReadOnlyList<IArg> args = null)
        {
            ThrowHelper.NullArgument(key, nameof(key));

            _messages.Add(key);
            _args = args;
        }

        public bool IsEmpty => !_messages.Any() && !_codes.Any();

        public ErrorMode Mode { get; private set; } = ErrorMode.Append;

        public bool TryAdd(ICommand command)
        {
            ThrowHelper.NullArgument(command, nameof(command));

            if (command is WithMessageCommand withMessageCommand)
            {
                ClearError();
                _messages.Add(withMessageCommand.Message);
            }
            else if (command is WithExtraMessageCommand withExtraMessageCommand)
            {
                _messages.Add(withExtraMessageCommand.Message);
            }
            else if (command is WithCodeCommand withCodeCommand)
            {
                ClearError();
                _codes.Add(withCodeCommand.Code);
            }
            else if (command is WithExtraCodeCommand withExtraCodeCommand)
            {
                _codes.Add(withExtraCodeCommand.Code);
            }
            else if (command is WithErrorClearedCommand)
            {
                ClearError();
            }
            else
            {
                return false;
            }

            return true;
        }

        public IError Build()
        {
            return new Error
            {
                Args = _args ?? Array.Empty<IArg>(),
                Codes = _codes,
                Messages = _messages
            };
        }

        private void ClearError()
        {
            Mode = ErrorMode.Override;
            _codes.Clear();
            _messages.Clear();
        }
    }
}
