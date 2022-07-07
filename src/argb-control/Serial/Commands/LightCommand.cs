using System.Buffers.Binary;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace ARGBControl.Serial.Commands
{

	public class LightCommand : ISerialCommand
	{
		public byte DeviceIndex { get; set; }
		public List<Color> Pixels { get; set; }
	}

	public class LightCommandHandler : ISerialCommandHandler<LightCommand>
	{
		private const byte MessageByte = 0x01;

		public async Task HandleCommand(LightCommand command, Stream serial)
		{
			var length = (short)(command.Pixels.Count * 3 + 1);
			var buffer = new byte[3];

			buffer[0] = MessageByte;
			BinaryPrimitives.WriteInt16BigEndian(buffer, length);

			await serial.WriteAsync(buffer, 0, 3);

			buffer[0] = command.DeviceIndex;

			await serial.WriteAsync(buffer, 0, 1);

			foreach (var pixel in command.Pixels) //FIXME test this to determine if it's any better than sending complete buffer at once
			{
				buffer[0] = pixel.R;
				buffer[1] = pixel.G;
				buffer[2] = pixel.B;

				await serial.WriteAsync(buffer, 0, 3);
			}
		}
	}
}
