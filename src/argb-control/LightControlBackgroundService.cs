using System;
using System.Drawing;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ARGBControl
{
	public class LightControlBackgroundService : BackgroundService
	{
		private readonly ILogger<LightControlBackgroundService> logger;
		private readonly IOptionsMonitor<Configuration> options;
		private readonly IQueue<LightCommand> queue;
		private SerialPort serial;

		public LightControlBackgroundService(
			ILogger<LightControlBackgroundService> logger,
			IOptionsMonitor<Configuration> options,
			IQueue<LightCommand> queue)
		{
			this.logger = logger;
			this.options = options;
			this.queue = queue;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					var serialPortName = this.options.CurrentValue.SerialPort;

					this.logger.LogInformation($"Attempting to open communications with serial port {serialPortName}...");

					this.serial = new SerialPort(serialPortName, 115200);
					this.serial.Open();

					await Task.Delay(125);

					this.logger.LogInformation("Connected!");
				}
				catch (Exception ex)
				{
					this.logger.LogError(ex, $"Opening serial port {this.serial.PortName} failed.");
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

					try
					{
						var positionOverrides = this.options.CurrentValue.DevicePositions;
						if (positionOverrides?.Count > command.DeviceIndex)
						{
							command.DeviceIndex = positionOverrides[command.DeviceIndex];
						}

						await this.SetLightColor(command.DeviceIndex, command.LightIndex, command.Color);
					}
					catch (Exception ex)
					{
						this.logger.LogError(ex, $"Unable to set light {command.LightIndex} on device {command.DeviceIndex} to color {command.Color}.");
					}
				}
			}
		}

		private Task SetLightColor(int deviceIndex, int lightIndex, Color color)
		{
			var device = deviceIndex + 1;
			var light = lightIndex + 1;

			this.serial.WriteLine($"100 {device} {light} {color.R:X} {color.G:X} {color.B:X}");

			this.serial.ReadLine(); // Recieved
			var response = this.serial.ReadLine().Trim();

			if (response.Trim() != "ok")
			{
				throw new ArgumentException($"Unable to set color #{color.R:X}{color.G:X}{color.B:X} on port {device} light {light}: '{response}'.");
			}

			return Task.CompletedTask;
		}
	}
}
