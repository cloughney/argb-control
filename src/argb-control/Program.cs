using System;
using System.IO;
using ARGBControl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var userDataPath = Path.Combine(Environment.GetEnvironmentVariable("PROGRAMDATA"), @"KrikHome\ARGBControl");

var appSettingsFile = Path.Combine(Environment.CurrentDirectory, "settings.json");
var userSettingsFile = Path.Combine(userDataPath, "settings.json");

if (!File.Exists(userSettingsFile))
{
	Directory.CreateDirectory(userDataPath);
	File.Copy(appSettingsFile, userSettingsFile);
}

var host = new HostBuilder()
	.ConfigureAppConfiguration(config =>
	{
		config.AddJsonFile(appSettingsFile, optional: false, reloadOnChange: true);
		config.AddJsonFile(userSettingsFile, optional: true, reloadOnChange: true);
	})
	.ConfigureLogging((context, logging) =>
	{
		logging.AddEventLog();
	})
	.ConfigureServices((context, services) =>
	{
		services.AddHostedService<TrayIconBackgroundService>();
		services.AddHostedService<LightControlBackgroundService>();
		services.AddHostedService<ProcessWatcherBackgroundService>();

		services.Configure<Configuration>(configuration =>
		{
			context.Configuration.Bind(configuration);

			configuration.UserDataPath = userDataPath;
			configuration.UserSettingsFilePath = userSettingsFile;
		});

		services.AddSingleton<IOptionsWriter<Configuration>>(x => new JsonOptionsWriter<Configuration>(userSettingsFile));

		services.AddSingleton<IProfileController, ProfileController>();
		services.AddSingleton<IQueue<LightCommand>, InMemoryQueue<LightCommand>>();
		services.AddSingleton<IQueue<StatusUpdate>, InMemoryQueue<StatusUpdate>>();
	})
	.Build();

var logger = host.Services.GetRequiredService<ILogger<IHost>>();

try
{
	logger.LogDebug("Activating the default profile...");
	ActivateDefaultProfile(host);

	logger.LogDebug("Starting host...");
	await host.RunAsync();
}
catch (Exception ex)
{
	logger.LogError(ex, "Something went really poorly.");
}

static void ActivateDefaultProfile(IHost host)
{
	var configuration = host.Services.GetRequiredService<IOptionsSnapshot<Configuration>>().Value;
	if (configuration.Profiles.TryGetValue(configuration.DefaultProfile, out var defaultProfile))
	{
		var profileController = host.Services.GetRequiredService<IProfileController>();
		profileController.SetActiveProfile(configuration.DefaultProfile, defaultProfile);
	}
}