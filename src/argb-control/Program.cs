using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ARGBControl;
using ARGBControl.Updates;
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
		services.AddHostedService<ProcessWatcherBackgroundService>();

		services.Configure<Configuration>(configuration =>
		{
			context.Configuration.Bind(configuration);

			configuration.UserDataPath = userDataPath;
			configuration.UserSettingsFilePath = userSettingsFile;
		});

		services.AddHttpClient<KrikCoUpdateClient>(http => http.BaseAddress = new Uri("https://krik.co/argb-control/"));
		services.AddTransient<IUpdateClient, KrikCoUpdateClient>(x => x.GetRequiredService<KrikCoUpdateClient>());

		services.AddSingleton<IProfileController, ProfileController>();
		services.AddSingleton<IQueue<StatusUpdate>, InMemoryQueue<StatusUpdate>>();

		services.AddSerialCommunication();
	})
	.Build();

var logger = host.Services.GetRequiredService<ILogger<IHost>>();

try
{
	logger.LogDebug("Checking for updates...");
	await CheckForUpdate(host);

	logger.LogDebug("Activating the default profile...");
	ActivateDefaultProfile(host);

	logger.LogDebug("Starting host...");
	await host.RunAsync();
}
catch (Exception ex)
{
	logger.LogError(ex, "Something went really poorly.");
}

static async Task CheckForUpdate(IHost host)
{
	var updateChecker = host.Services.GetRequiredService<IUpdateClient>();

	try
	{
		if (await updateChecker.CheckForUpdate())
		{
			var result = MessageBox.Show(
				"A new version of ARGB Control is available. Would you like to update?",
				"Update Available",
				MessageBoxButtons.YesNo,
				MessageBoxIcon.Information);

			if (result.HasFlag(DialogResult.No))
			{
				return;
			}

			MessageBox.Show(
				"The update will begin shortly.",
				"Update",
				MessageBoxButtons.OK,
				MessageBoxIcon.Information);

			var file = await updateChecker.GetLatestInstaller();

			Process.Start(new ProcessStartInfo
			{
				FileName = "cmd",
				Arguments = $"/c {file.FullName} & del {file.FullName}",
				CreateNoWindow = true
			});
		}
	}
	catch (Exception)
	{
		//TODO log and fail gracefully
	}
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