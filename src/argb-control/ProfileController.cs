using System;
using System.Drawing;
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
		private readonly IQueue<ISerialCommand> commands;
		public ProfileController(IQueue<ISerialCommand> commands) => this.commands = commands;

		public string ActiveProfileName { get; private set; }

		public void SetActiveProfile(string name, Profile profile)
		{
			if (profile == null)
			{
				throw new ArgumentNullException(nameof(profile));
			}

			var commands = profile.Devices.Select((device, deviceIndex) =>
				new LightCommand
				{
					DeviceIndex = (byte)deviceIndex,
					Pixels = device.Pixels
				}
			);

			foreach (var command in commands)
			{
				this.commands.Enqueue(command);
			}

			this.ActiveProfileName = name;
		}
	}
}
