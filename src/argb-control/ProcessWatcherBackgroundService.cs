using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ARGBControl
{
	public class ProcessWatcherBackgroundService : BackgroundService
	{
		private readonly ILogger<ProcessWatcherBackgroundService> logger;
		private readonly IOptionsMonitor<Configuration> options;
		private readonly IProfileController profileController;

		public ProcessWatcherBackgroundService(
			ILogger<ProcessWatcherBackgroundService> logger,
			IOptionsMonitor<Configuration> options,
			IProfileController profileController)
		{
			this.logger = logger;
			this.options = options;
			this.profileController = profileController;
		}

		protected override async Task ExecuteAsync(CancellationToken stoppingToken)
		{
			using var startWatcher = new ManagementEventWatcher("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'");
			using var stopWatcher = new ManagementEventWatcher("SELECT * FROM __InstanceDeletionEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Process'");

			startWatcher.EventArrived += new EventArrivedEventHandler(this.HandleProcessStart);
			stopWatcher.EventArrived += new EventArrivedEventHandler(this.HandleProcessStop);

			while (!stoppingToken.IsCancellationRequested)
			{
				try
				{
					startWatcher.Start();
					stopWatcher.Start();

					await Task.Delay(Timeout.Infinite, stoppingToken);

					startWatcher.Stop();
					stopWatcher.Stop();
				}
				catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
				{
					// Exiting...
				}
				catch (Exception ex)
				{
					this.logger.LogError(ex, "Unable to listen to system events.");
				}

				await Task.Delay(TimeSpan.FromSeconds(30));
			}
		}

		private void HandleProcessStart(object sender, EventArrivedEventArgs args)
		{
			var processName = this.GetProcessNameFromEvent(args);
			if (processName is null)
			{
				return;
			}

			var configuration = this.options.CurrentValue;

			var trigger = configuration.Triggers.FirstOrDefault(x => x.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (trigger == null)
			{
				return;
			}

			if (!configuration.Profiles.TryGetValue(trigger.ProfileName, out var profile))
			{
				this.logger.LogWarning($"Ignoring start of trigger process {processName} because the profile '{configuration.DefaultProfile}' cannot be found.");
				return;
			}

			this.profileController.SetActiveProfile(trigger.ProfileName, profile);
		}

		private void HandleProcessStop(object sender, EventArrivedEventArgs args)
		{
			var processName = this.GetProcessNameFromEvent(args);
			if (processName is null)
			{
				return;
			}

			var configuration = this.options.CurrentValue;

			var trigger = configuration.Triggers.FirstOrDefault(x => x.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase));
			if (trigger == null)
			{
				return;
			}

			if (trigger.ProfileName == configuration.DefaultProfile)
			{
				return;
			}

			if (trigger.ProfileName != this.profileController.ActiveProfileName)
			{
				this.logger.LogDebug($"Ignoring stop of trigger process {processName} because the triggered profile is not active.");
				return;
			}

			if (!configuration.Profiles.TryGetValue(configuration.DefaultProfile, out var profile))
			{
				this.logger.LogWarning($"Ignoring stop of trigger process {processName} because the profile '{configuration.DefaultProfile}' cannot be found.");
				return;
			}

			this.profileController.SetActiveProfile(configuration.DefaultProfile, profile);
		}

		private string GetProcessNameFromEvent(EventArrivedEventArgs args)
		{
			var targetInstance = (ManagementBaseObject) args.NewEvent["TargetInstance"];
			return targetInstance["Name"]?.ToString();
		}
	}
}
