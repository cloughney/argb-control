using System;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using ARGBControl.Serial.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ARGBControl.Serial
{
	public class SerialCommandBackgroundService : BackgroundService
	{
		private readonly ILogger<SerialCommandBackgroundService> logger;
		private readonly IOptionsMonitor<Configuration> options;
		private readonly IQueue<ISerialCommand> queue;
		private readonly ISerialCommandExecutor commandExecutor;

		private SerialPort serial;

		public SerialCommandBackgroundService(
			ILogger<SerialCommandBackgroundService> logger,
			IOptionsMonitor<Configuration> options,
			IQueue<ISerialCommand> queue,
			ISerialCommandExecutor commandExecutor)
		{
			this.logger = logger;
			this.options = options;
			this.queue = queue;
			this.commandExecutor = commandExecutor;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				if (!this.OpenSerialConnection())
				{
					await Task.Delay(5000);
					continue;
				}

				while (!stoppingToken.IsCancellationRequested)
				{
					var command = await this.queue.DequeueAsync(stoppingToken);

					if (!this.serial.IsOpen)
					{
						break;
					}

					await this.commandExecutor.ExecuteCommand(this.serial, command);
				}
			}
		}

		private bool OpenSerialConnection()
		{
			var serialPortName = this.options.CurrentValue.SerialPort;

			try
			{
				this.logger.LogInformation("Attempting to open communications with serial port {SerialPort}...", serialPortName);

				this.serial = new SerialPort(serialPortName, 115200);
				this.serial.Open();

				//await Task.Delay(125);

				this.logger.LogInformation("Connected to serial port {SerialPort}!", serialPortName);
				return true;
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Opening serial port {SerialPort} failed.", serialPortName);
				return false;
			}
		}
	}
}
