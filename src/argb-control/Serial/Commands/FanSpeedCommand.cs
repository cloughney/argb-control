using System.Buffers.Binary;
using System.IO;
using System.Threading.Tasks;

namespace ARGBControl.Serial.Commands
{
	public class FanSpeedCommand : ISerialCommand
	{
		public byte DeviceIndex { get; set; }
		public byte Speed { get; set; }
	}

	public class FanSpeedCommandHandler : ISerialCommandHandler<FanSpeedCommand>
	{
		private const byte MessageByte = 0x01;

		public async Task HandleCommand(FanSpeedCommand command, Stream serial)
		{
			var buffer = new byte[3];

			buffer[0] = MessageByte;
			BinaryPrimitives.WriteInt16BigEndian(buffer, 2);

			await serial.WriteAsync(buffer, 0, 3);

			buffer[0] = command.DeviceIndex;
			buffer[1] = command.Speed;

			await serial.WriteAsync(buffer, 0, 2);
		}
	}
}
