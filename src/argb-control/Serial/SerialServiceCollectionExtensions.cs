using ARGBControl;
using ARGBControl.Serial;
using ARGBControl.Serial.Commands;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class SerialServiceCollectionExtensions
	{
		public static IServiceCollection AddSerialCommunication(this IServiceCollection services)
		{
			services.AddSingleton<IQueue<ISerialCommand>, InMemoryQueue<ISerialCommand>>();

			services.AddSingleton<ISerialCommandExecutor, SerialCommandExecutor>();
			services.AddSingleton<ISerialCommandReceiver, SerialCommandReceiver>();

			services.AddScoped<ISerialCommandHandler<LightCommand>, LightCommandHandler>();
			services.AddScoped<ISerialCommandHandler<FanSpeedCommand>, FanSpeedCommandHandler>();

			services.AddHostedService<SerialCommandBackgroundService>();

			return services;
		}
	}
}
