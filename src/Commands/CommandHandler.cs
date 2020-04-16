using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace SqlStreamStore.Demo.Commands
{
    public class CommandHandler : ICommandHandler
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MethodInfo _genericHandle;
        public CommandHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _genericHandle = typeof(CommandHandler)
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .First(x => x.GetGenericArguments().Length == 1 && x.Name == "Handle");
        }

        public async Task Handle(ICommand command)
        {   
            var inputType = command.GetType();
            var genericMethod = _genericHandle.MakeGenericMethod(inputType);
            var genericTask = (Task)genericMethod.Invoke(this, new[] { command });
            await genericTask.ConfigureAwait(false);
        }

        public async Task Handle<TCommand>(TCommand command) 
            where TCommand: ICommand
        {
            var handler = _serviceProvider.GetService<ICommandHandler<TCommand>>();
            if (handler == null)
            {
                throw new InvalidOperationException($"No handler found for command {command.GetType().FullName }");
            }
            await handler.Handle(command);
        }
    }
}