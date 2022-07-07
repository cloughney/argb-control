namespace ARGBControl.Serial.Commands
{
	public interface ISerialCommand
	{

	}

	//JSON configuration example?
	//{
	//	"triggers": [
	//		{
	//			"type": "ProcessStarted",
	//			"processName": "SoTGame.exe",
	//			"profile": "Sea of Thieves"
	//		}
	//	],
	//	"profiles": {
	//		"Sea of Thieves": {
	//			"lights": [
	//				["Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise"],
	//				["LimeGreen", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen"],
	//				["LimeGreen", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "Turquoise", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen"],
	//				["LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen"],
	//				["LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen", "LimeGreen"]
	//			]
	//		}
	//	}
	//}

	//public class ActiveWindowHostedService : BackgroundService
	//{
	//	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	//	{
	//		using var argb = new LightController();
	//		await argb.Connect();

	//		int? lastProcessId = null;

	//		while (!stoppingToken.IsCancellationRequested)
	//		{
	//			var foreground = GetActiveProcess();
	//			if (foreground.Id == lastProcessId)
	//			{
	//				continue;
	//			}

	//			lastProcessId = foreground.Id;

	//			if (foreground.MainWindowTitle == "Xbox")
	//			{
	//				await argb.FillColor(1, 12, Color.Green);
	//				await argb.FillColor(2, 12, Color.Green);
	//				await argb.FillColor(3, 12, Color.Green);
	//				await argb.FillColor(4, 12, Color.Green);
	//				await argb.FillColor(5, 12, Color.Green);
	//			}
	//			else if (foreground.ProcessName == "oriandthewillofthewisps-pc")
	//			{
	//				await argb.FillColor(1, 12, Color.Purple);
	//				await argb.FillColor(2, 12, Color.Purple);
	//				await argb.FillColor(3, 12, Color.Purple);
	//				await argb.FillColor(4, 12, Color.Purple);
	//				await argb.FillColor(5, 12, Color.Purple);
	//			}

	//			await Task.Delay(5000, stoppingToken);
	//		}
	//	}

	//	private static Process GetActiveProcess()
	//	{
	//		var handle = GetForegroundWindow();

	//		GetWindowThreadProcessId(handle, out var pid);

	//		return Process.GetProcessById((int)pid);
	//	}

	//	[DllImport("user32.dll")]
	//	static extern IntPtr GetForegroundWindow();

	//	[DllImport("user32.dll")]
	//	public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);
	//}

	//static class LightControllerExtensions
	//{
	//	public static async Task UseDefaultProfile(this LightController controller)
	//	{
	//		await controller.FillColor(1, 12, Color.FromArgb(0, 0, 100, 64));
	//		await controller.FillColor(2, 12, Color.FromArgb(0, 64, 64, 64));
	//		await controller.FillColor(3, 12, Color.FromArgb(0, 64, 64, 64));
	//		await controller.FillColor(4, 12, Color.FromArgb(0, 64, 64, 64));
	//		await controller.FillColor(5, 12, Color.FromArgb(0, 64, 64, 64));
	//	}

	//	public static async Task UseCallOfDutyProfile(this LightController controller)
	//	{
	//		await controller.FillColor(1, 12, Color.Aqua);
	//		await controller.FillColor(2, 12, Color.OrangeRed);
	//		await controller.FillColor(3, 12, Color.OrangeRed);
	//		await controller.FillColor(4, 12, Color.OrangeRed);
	//		await controller.FillColor(5, 12, Color.OrangeRed);
	//	}

	//	public static async Task UseSand(this LightController controller)
	//	{
	//		await controller.FillColor(1, 12, Color.SaddleBrown);
	//		await controller.FillColor(2, 12, Color.SaddleBrown);
	//		await controller.FillColor(3, 12, Color.SaddleBrown);
	//		await controller.FillColor(4, 12, Color.SaddleBrown);
	//		await controller.FillColor(5, 12, Color.SaddleBrown);
	//	}

	//	public static async Task UseSeaOfThievesProfile(this LightController controller)
	//	{
	//		await controller.FillColor(1, 12, Color.Turquoise);

	//		await controller.SetLightColor(2, 1, Color.LimeGreen);
	//		await controller.SetLightColor(2, 2, Color.Turquoise);
	//		await controller.SetLightColor(2, 3, Color.Turquoise);
	//		await controller.SetLightColor(2, 4, Color.Turquoise);
	//		await controller.SetLightColor(2, 5, Color.Turquoise);
	//		await controller.SetLightColor(2, 6, Color.Turquoise);
	//		await controller.SetLightColor(2, 7, Color.Turquoise);
	//		await controller.SetLightColor(2, 8, Color.Turquoise);
	//		await controller.SetLightColor(2, 9, Color.LimeGreen);
	//		await controller.SetLightColor(2, 10, Color.LimeGreen);
	//		await controller.SetLightColor(2, 11, Color.LimeGreen);
	//		await controller.SetLightColor(2, 12, Color.LimeGreen);

	//		await controller.SetLightColor(3, 1, Color.LimeGreen);
	//		await controller.SetLightColor(3, 2, Color.Turquoise);
	//		await controller.SetLightColor(3, 3, Color.Turquoise);
	//		await controller.SetLightColor(3, 4, Color.Turquoise);
	//		await controller.SetLightColor(3, 5, Color.Turquoise);
	//		await controller.SetLightColor(3, 6, Color.Turquoise);
	//		await controller.SetLightColor(3, 7, Color.Turquoise);
	//		await controller.SetLightColor(3, 8, Color.Turquoise);
	//		await controller.SetLightColor(3, 9, Color.LimeGreen);
	//		await controller.SetLightColor(3, 10, Color.LimeGreen);
	//		await controller.SetLightColor(3, 11, Color.LimeGreen);
	//		await controller.SetLightColor(3, 12, Color.LimeGreen);

	//		await controller.FillColor(4, 12, Color.LimeGreen);
	//		await controller.FillColor(5, 12, Color.LimeGreen);
	//	}

	//	public static async Task UseRocketLeagueProfile(this LightController controller)
	//	{
	//		await controller.FillColor(2, 12, Color.Blue);
	//		await controller.FillColor(3, 12, Color.Blue);
	//		await controller.FillColor(4, 12, Color.OrangeRed);
	//		await controller.FillColor(5, 12, Color.OrangeRed);

	//		await controller.SetLightColor(1, 1, Color.OrangeRed);
	//		await controller.SetLightColor(1, 2, Color.OrangeRed);
	//		await controller.SetLightColor(1, 3, Color.OrangeRed);
	//		await controller.SetLightColor(1, 4, Color.OrangeRed);
	//		await controller.SetLightColor(1, 5, Color.OrangeRed);
	//		await controller.SetLightColor(1, 6, Color.OrangeRed);
	//		await controller.SetLightColor(1, 7, Color.Blue);
	//		await controller.SetLightColor(1, 8, Color.Blue);
	//		await controller.SetLightColor(1, 9, Color.Blue);
	//		await controller.SetLightColor(1, 10, Color.Blue);
	//		await controller.SetLightColor(1, 11, Color.Blue);
	//		await controller.SetLightColor(1, 12, Color.Blue);
	//	}
	//}
}
