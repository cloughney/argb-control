using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ARGBControl.Updates
{
	public interface IUpdateClient
	{
		Task<bool> CheckForUpdate(CancellationToken cancellationToken = default);
		Task<FileInfo> GetLatestInstaller(CancellationToken cancellationToken = default);
	}

	public class KrikCoUpdateClient : IUpdateClient
	{
		private readonly HttpClient http;
		public KrikCoUpdateClient(HttpClient http) => this.http = http;

		private string AppVersion => this.GetType().Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;

		public async Task<bool> CheckForUpdate(CancellationToken cancellationToken = default)
		{
			var version = await this.GetVersionInformation(cancellationToken);

			return this.AppVersion != version.Latest;
		}

		public async Task<FileInfo> GetLatestInstaller(CancellationToken cancellationToken = default)
		{
			var version = await this.GetVersionInformation(cancellationToken);

			using var response = await http.GetAsync(version.FileName, cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				throw new UpdateCheckException($"The server responded with status code {response.StatusCode}.");
			}

			var localFileName = Environment.ExpandEnvironmentVariables(@$"%TEMP%\{Guid.NewGuid()}_{version.FileName}");

			using var stream = await response.Content.ReadAsStreamAsync();
			using var file = File.Create(localFileName);

			file.Seek(0, SeekOrigin.Begin);
			await stream.CopyToAsync(file);

			return new FileInfo(localFileName);
		}

		private async Task<VersionInformation> GetVersionInformation(CancellationToken cancellationToken)
		{
			using var response = await http.GetAsync("version.json", cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				throw new UpdateCheckException($"The server responded with status code {response.StatusCode}.");
			}

			using var stream = await response.Content.ReadAsStreamAsync();

			return await JsonSerializer.DeserializeAsync<VersionInformation>(stream, new()
			{
				PropertyNameCaseInsensitive = true
			});
		}

		private record VersionInformation
		{
			public string Latest { get; set; }
			public string FileName { get; set; }
		}
	}

	public class UpdateCheckException : Exception
	{
		public UpdateCheckException(string message) : base(message)
		{
		}
	}
}
