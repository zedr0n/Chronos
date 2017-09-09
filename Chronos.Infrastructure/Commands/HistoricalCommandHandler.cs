using Chronos.Infrastructure.Interfaces;

namespace Chronos.Infrastructure.Commands
{
	public interface IHistoricalCommandHandler<TCommand> : ICommandHandler<HistoricalCommand<TCommand>>
		where TCommand : class,ICommand
	{
		
	}
	
	public class HistoricalCommandHandler<TCommand> : IHistoricalCommandHandler<TCommand>
		where TCommand : class,ICommand
	{
		private readonly ITimeNavigator _timeNavigator;
		private readonly ICommandHandler<TCommand> _handler;

		public HistoricalCommandHandler(ITimeNavigator timeNavigator, ICommandHandler<TCommand> handler)
		{
			_timeNavigator = timeNavigator;
			_handler = handler;
		}

		public void Handle(HistoricalCommand<TCommand> command)
		{
			_timeNavigator.GoTo(command.At);
			_handler.Handle(command.Command);
			_timeNavigator.Reset();
		}
	}	
}


