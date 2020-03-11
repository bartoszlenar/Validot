namespace Validot.Validation.Scopes.Builders
{
    using System.Collections.Generic;

    using Validot.Specification;
    using Validot.Specification.Commands;
    using Validot.Translations;

    internal class ScopeBuilder
    {
        public SpecificationScope<T> Build<T>(Specification<T> specification, IScopeBuilderContext context)
        {
            ThrowHelper.NullArgument(specification, nameof(specification));
            ThrowHelper.NullArgument(context, nameof(context));

            var specificationApi = (SpecificationApi<T>)specification(new SpecificationApi<T>());

            var commandScopes = new List<ICommandScope<T>>();

            var presenceInfo = ParsePresenceInfo(specificationApi.Commands, context, out var index);

            for (var i = index; i < specificationApi.Commands.Count; ++i)
            {
                var scopeCommand = (IScopeCommand)specificationApi.Commands[i];

                var scopeBuilder = scopeCommand.GetScopeBuilder();

                while (++i < specificationApi.Commands.Count)
                {
                    if (!scopeBuilder.TryAdd(specificationApi.Commands[i]))
                    {
                        break;
                    }
                }

                --i;

                var commandScope = (ICommandScope<T>)scopeBuilder.Build(context);

                commandScopes.Add(commandScope);
            }

            return new SpecificationScope<T>
            {
                Presence = presenceInfo.Presence,
                ForbiddenErrorId = presenceInfo.ForbiddenErrorId,
                RequiredErrorId = presenceInfo.RequiredErrorId,
                CommandScopes = commandScopes
            };
        }

        private static PresenceInfo ParsePresenceInfo(IReadOnlyList<ICommand> commands, IScopeBuilderContext context, out int index)
        {
            index = 0;

            if (commands.Count == 0 || !IsPresenceCommand(commands[0]))
            {
                return new PresenceInfo
                {
                    Presence = Presence.Required,
                    RequiredErrorId = context.RequiredErrorId,
                    ForbiddenErrorId = context.ForbiddenErrorId
                };
            }

            if (commands[0] is OptionalCommand)
            {
                index = 1;

                return new PresenceInfo
                {
                    Presence = Presence.Optional,
                    RequiredErrorId = context.RequiredErrorId,
                    ForbiddenErrorId = context.ForbiddenErrorId
                };
            }

            if (commands[0] is RequiredCommand)
            {
                var requiredErrorId = GetErrorId(context.RequiredErrorId, MessageKey.Global.Required, out index);

                return new PresenceInfo
                {
                    Presence = Presence.Required,
                    RequiredErrorId = requiredErrorId,
                    ForbiddenErrorId = context.ForbiddenErrorId
                };
            }

            var forbiddenErrorId = GetErrorId(context.ForbiddenErrorId, MessageKey.Global.Forbidden, out index);

            return new PresenceInfo
            {
                Presence = Presence.Forbidden,
                RequiredErrorId = context.RequiredErrorId,
                ForbiddenErrorId = forbiddenErrorId
            };

            int GetErrorId(int defaultErrorId, string baseMessageKey, out int i)
            {
                var errorsBuilder = baseMessageKey != null
                    ? new ErrorBuilder(baseMessageKey)
                    : new ErrorBuilder();

                var somethingAdded = false;

                for (i = 1; i < commands.Count; ++i)
                {
                    if (!errorsBuilder.TryAdd(commands[i]))
                    {
                        break;
                    }

                    somethingAdded = true;
                }

                if (!somethingAdded || errorsBuilder.IsEmpty)
                {
                    return defaultErrorId;
                }

                var error = errorsBuilder.Build();

                return context.RegisterError(error);
            }
        }

        private static bool IsPresenceCommand(ICommand c)
        {
            return c is OptionalCommand || c is RequiredCommand || c is ForbiddenCommand;
        }

        private class PresenceInfo
        {
            public Presence Presence { get; set; }

            public int ForbiddenErrorId { get; set; }

            public int RequiredErrorId { get; set; }
        }
    }
}
