namespace Validot.Specification.Commands
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    using Validot.Validation.Scopes;
    using Validot.Validation.Scopes.Builders;

    internal abstract class MemberCommand : IScopeCommand
    {
        public abstract ICommandScopeBuilder GetScopeBuilder();
    }

    internal class MemberCommand<T, TMember> : MemberCommand
    {
        public MemberCommand(Expression<Func<T, TMember>> memberSelector, Specification<TMember> specification)
        {
            ThrowHelper.NullArgument(memberSelector, nameof(memberSelector));
            ThrowHelper.NullArgument(specification, nameof(specification));

            MemberSelector = memberSelector;
            Specification = specification;
        }

        public Expression<Func<T, TMember>> MemberSelector { get; }

        public Specification<TMember> Specification { get; }

        public override ICommandScopeBuilder GetScopeBuilder()
        {
            return new CommandScopeBuilder<T>(this, (command, context) =>
            {
                var cmd = (MemberCommand<T, TMember>)command;

                var block = new MemberCommandScope<T, TMember>
                {
                    GetMemberValue = cmd.MemberSelector.Compile(),
                    ScopeId = context.GetOrRegisterSpecificationScope(cmd.Specification),
                    Path = GetMemberName(cmd.MemberSelector)
                };

                return block;
            });
        }

        private static string GetMemberName(Expression<Func<T, TMember>> field)
        {
            if (field.ToString().Count(c => c == '.') > 1)
            {
                throw new InvalidOperationException($"Only one level of nesting is allowed, {field} looks like it is going further (member of a member?)");
            }

            MemberExpression memberExpression = null;

            if (field.Body is MemberExpression)
            {
                memberExpression = (MemberExpression)field.Body;
            }
            else
            {
                throw new InvalidOperationException($"Only properties and variables are valid members to validate, {field} looks like it is pointing at something else (a method?).");
            }

            return memberExpression.Member.Name;
        }
    }
}
