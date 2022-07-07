using System.IO.Ports;

namespace ARGBControl.Serial
{
	public interface ISerialCommandReceiver
	{
		void HandleIncomingMessage(SerialPort serial);
	}

	public class SerialCommandReceiver : ISerialCommandReceiver
	{
		private byte bufferIndex = 0;
		private readonly byte[] buffer = new byte[byte.MaxValue];

		public void HandleIncomingMessage(SerialPort serial)
		{
			//if (this.bufferIndex == 0)
			//{
			//	serial.Read(this.buffer, 0, 2);
			//	this.bufferIndex = 2;
			//}

			//if (this.buffer[0] < 0x10 || this.buffer[0] > 0x11)
			//{
			//	//invalid message type
			//}

			//var remaining = this.buffer[1] + 2 - this.bufferIndex;
			//serial.Read(this.buffer, 2, remaining);


		}
	}
}
