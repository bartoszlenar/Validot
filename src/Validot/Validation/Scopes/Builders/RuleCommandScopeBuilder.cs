namespace Validot.Validation.Scopes.Builders
{
    using System;

    using Validot.Specification.Commands;

    internal class RuleCommandScopeBuilder<T> : ICommandScopeBuilder
    {
        private readonly ErrorBuilder _errorsBuilder = new ErrorBuilder();

        private readonly RuleCommand<T> _ruleCommand;

        private string _name;

        private Predicate<T> _shouldExecute;

        public RuleCommandScopeBuilder(RuleCommand<T> ruleCommand)
        {
            ThrowHelper.NullArgument(ruleCommand, nameof(ruleCommand));

            _ruleCommand = ruleCommand;

            if (_ruleCommand.Message != null)
            {
                _errorsBuilder = new ErrorBuilder(_ruleCommand.Message, _ruleCommand.Args);
            }
        }

        public ICommandScope Build(IScopeBuilderContext context)
        {
            ThrowHelper.NullArgument(context, nameof(context));

            var ruleCommandScope = new RuleCommandScope<T>
            {
                IsValid = _ruleCommand.ValidCondition
            };

            ruleCommandScope.Name = _name;
            ruleCommandScope.ShouldExecute = _shouldExecute;
            ruleCommandScope.ErrorMode = _errorsBuilder.Mode;

            if (_errorsBuilder.IsEmpty)
            {
                ruleCommandScope.ErrorId = context.DefaultErrorId;
            }
            else
            {
                var error = _errorsBuilder.Build();

                ruleCommandScope.ErrorId = context.RegisterError(error);
            }

            return ruleCommandScope;
        }

        public bool TryAdd(ICommand command)
        {
            if (command is WhenCommand<T> whenCommand)
            {
                _shouldExecute = whenCommand.ExecutionCondition;

                return true;
            }

            if (command is WithNameCommand withNameCommand)
            {
                _name = withNameCommand.Name;

                return true;
            }

            return _errorsBuilder.TryAdd(command);
        }
    }
}
