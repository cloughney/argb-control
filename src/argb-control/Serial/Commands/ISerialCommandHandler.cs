using System.IO;
using System.Threading.Tasks;

namespace ARGBControl.Serial.Commands
{
	public interface ISerialCommandHandler<T> where T : ISerialCommand
	{
		Task HandleCommand(T command, Stream serial);
	}
}
