using System.Runtime.InteropServices;

namespace ScalarDemo.Model;

[StructLayout(LayoutKind.Sequential)]
public struct WelcomeEmailModel
{
	public string Name { get; set; }
	public DateTime Date { get; set; }
	public string DashboardUrl { get; set; }
}
