using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ARGBControl
{
	public class TrayIconBackgroundService : BackgroundService
	{
		private readonly IHostApplicationLifetime host;
		private readonly IOptionsMonitor<Configuration> options;
		private readonly IProfileController profileController;

		public TrayIconBackgroundService(
			IHostApplicationLifetime host,
			IOptionsMonitor<Configuration> options,
			IProfileController profileController)
		{
			this.host = host;
			this.options = options;
			this.profileController = profileController;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.Run(() =>
		{
			using var icon = new NotifyIcon 
			{
				Icon = new Icon(Path.Combine(Environment.CurrentDirectory, "bulb.ico")),
				Text = "KrikHome ARGB Control",
				Visible = true,
				ContextMenuStrip = this.CreateContextMenu()
			};

			this.host.ApplicationStopping.Register(() => {
				icon.Visible = false;
			});

			Application.Run();
		});

		private ContextMenuStrip CreateContextMenu()
		{
			var menu = new ContextMenuStrip();

			menu.Opening += new CancelEventHandler((_, e) =>
			{
				menu.Items.Clear();

				menu.Items.Add(new ToolStripMenuItem("KrikHome ARGB Control") { Enabled = false });

				menu.Items.Add("-");

				menu.Items.Add(this.CreateProfilesSubMenu());

				menu.Items.Add(this.CreateSettingsMenuItem());

				menu.Items.Add("-");

				menu.Items.Add("Exit", default, (_, _) => this.host.StopApplication());

				e.Cancel = false;
			});

			return menu;
		}

		private ToolStripItem CreateProfilesSubMenu()
		{
			const string OffProfileName = "Off";

			var profiles = this.options.CurrentValue.Profiles;
			var menu = new ToolStripMenuItem("Profiles");

			ToolStripItem CreateMenuItem(string name, Profile profile) => new ToolStripMenuItem(name, default,
				(_, _) => this.profileController.SetActiveProfile(name, profile))
			{
				Checked = name == this.profileController.ActiveProfileName,
				CheckOnClick = true
			};

			foreach (var (name, profile) in profiles)
			{
				if (name == OffProfileName)
				{
					continue;
				}

				menu.DropDownItems.Add(CreateMenuItem(name, profile));
			}

			if (profiles.TryGetValue(OffProfileName, out var offProfile))
			{
				menu.DropDownItems.Add("-");
				menu.DropDownItems.Add(CreateMenuItem(OffProfileName, offProfile));
			}

			return menu;
		}

		private ToolStripItem CreateSettingsMenuItem() => new ToolStripMenuItem("Settings", default, (_, _) =>
		{
			Process.Start(new ProcessStartInfo(this.options.CurrentValue.UserSettingsFilePath)
			{
				UseShellExecute = true
			});
		});
	}
}
