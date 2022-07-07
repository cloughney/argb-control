using System;
using System.IO.Ports;
using System.Threading.Tasks;
using ARGBControl.Serial.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ARGBControl.Serial
{
	public interface ISerialCommandExecutor
	{
		Task ExecuteCommand(SerialPort serial, ISerialCommand command);
	}

	public class SerialCommandExecutor : ISerialCommandExecutor
	{
		private readonly ILogger<SerialCommandExecutor> logger;
		private readonly IServiceProvider services;

		public SerialCommandExecutor(
			ILogger<SerialCommandExecutor> logger,
			IServiceProvider services)
		{
			this.logger = logger;
			this.services = services;
		}

		public async Task ExecuteCommand(SerialPort serial, ISerialCommand command)
		{
			var commandType = command.GetType();
			var handlerType = typeof(ISerialCommandHandler<>).MakeGenericType(commandType);
			var handleMethodType = handlerType.GetMethod(nameof(ISerialCommandHandler<ISerialCommand>.HandleCommand));

			try
			{
				using var scope = this.services.CreateScope();
				var handlerInstance = scope.ServiceProvider.GetRequiredService(handlerType);

				await (Task)handleMethodType.Invoke(handlerInstance, new object[] { command, serial.BaseStream });
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Failed to execute serial command {CommandType}.", commandType.Name);
			}
		}
	}
}
