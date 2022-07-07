using System;
using System.Linq;
using ARGBControl.Serial.Commands;

namespace ARGBControl
{
	public interface IProfileController
	{
		string ActiveProfileName { get; }
		void SetActiveProfile(string name, Profile profile);
	}

	public class ProfileController : IProfileController
	{
		private readonly IQueue<LightCommand> commands;
		public ProfileController(IQueue<LightCommand> commands) => this.commands = commands;

		public string ActiveProfileName { get; private set; }

		public void SetActiveProfile(string name, Profile profile)
		{
			if (profile == null)
			{
				throw new ArgumentNullException(nameof(profile));
			}

			var commands = profile.Devices.SelectMany((device, deviceIndex) => 
				device.Pixels.Select((pixelColor, pixelIndex) =>
					new LightCommand
					{
						DeviceIndex = deviceIndex,
						LightIndex = pixelIndex,
						Color = pixelColor
					}));

			foreach (var command in commands)
			{
				this.commands.Enqueue(command);
			}

			this.ActiveProfileName = name;
		}
	}
}
