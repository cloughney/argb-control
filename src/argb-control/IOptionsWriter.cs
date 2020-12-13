using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ARGBControl
{
	public interface IOptionsWriter<T>
	{
		Task Update(T options, CancellationToken cancellationToken = default);
	}

	public class JsonOptionsWriter<T> : IOptionsWriter<T>
	{
		private readonly string filePath;
		private readonly JsonSerializerOptions serializerOptions;

		public JsonOptionsWriter(string filePath)
		{
			this.filePath = filePath;
			this.serializerOptions = new JsonSerializerOptions
			{
				PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
				WriteIndented = true
			};
		}

		public async Task Update(T options, CancellationToken cancellationToken = default)
		{
			using var stream = File.OpenWrite(this.filePath);
			await JsonSerializer.SerializeAsync(stream, options, this.serializerOptions);
		}
	}
}
