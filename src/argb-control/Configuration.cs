using System.Collections.Generic;
using System.Drawing;

namespace ARGBControl
{
	public record Configuration
	{
		public string SerialPort { get; set; }
		public string DefaultProfile { get; set; }
		public IDictionary<string, Profile> Profiles { get; set; }
		public ICollection<Trigger> Triggers { get; set; }
		public IList<int> DevicePositions { get; set; }

		public string UserDataPath { get; set; }
		public string UserSettingsFilePath { get; set; }
	}

	public record Profile
	{
		public IList<Device> Devices { get; set; }
	}

	public record Device
	{
		public IList<Color> Pixels { get; set; }
	}

	public record Trigger
	{
		public string ProcessName { get; set; }
		public string ProfileName { get; set; }
	}
}
