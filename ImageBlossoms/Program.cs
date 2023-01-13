using Eto.Forms;
using ImageBlossoms.Events;
using ImageBlossoms.ViewModels;
using ImageBlossoms.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ImageBlossoms
{
	internal class Program
	{
		private static readonly IHost _host = Host
			.CreateDefaultBuilder()
			.ConfigureServices((context, services) =>
			{
				services.AddSingleton<EventAggregator>();
				// Views
				services.AddSingleton<MainForm>();
				// ViewModels
				services.AddSingleton<MainFormViewModel>();
			})
			.Build();

		[STAThread]
		static void Main(string[] args)
		{
			IServiceProvider provider = _host.Services;
			new Application(Eto.Platform.Detect).Run(provider.GetRequiredService<MainForm>());
		}
	}
}
