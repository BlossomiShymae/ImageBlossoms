using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using WrapMode = System.Drawing.Drawing2D.WrapMode;

namespace ImageBlossoms.ViewModels
{
	public partial class MainFormViewModel : ObservableObject
	{
		public readonly Command AboutCommand = new() { MenuText = "About..." };
		public readonly Command ClickMeCommand = new() { MenuText = "Click Me!", ToolBarText = "Click Me!" };
		public readonly Command QuitCommand = new() { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };

		[ObservableProperty]
		private int _progressValue;
		[ObservableProperty]
		private string _inputFolder;
		[ObservableProperty]
		private string _outputFolder;
		[ObservableProperty]
		private bool _useSameFolder = false;
		[ObservableProperty]
		private bool _outputFolderEnabled = true;
		[ObservableProperty]
		private bool _scaled = false;
		[ObservableProperty]
		private double _width;
		[ObservableProperty]
		private bool _widthEnabled = false;
		[ObservableProperty]
		private double _height;
		[ObservableProperty]
		private bool _heightEnabled = false;
		[ObservableProperty]
		private string _consoleText = "";

		public MainFormViewModel()
		{
			QuitCommand.Executed += (sender, e) => Application.Instance.Quit();
		}

		private static string GetTime() => DateTime.Now.ToString("s");
		private void WriteConsole(string text)
		{
			ConsoleText += GetTime() + " " + text + "\n";
		}

		private void ProcessImages()
		{
			ProgressValue = 0;
			if (InputFolder == null || OutputFolder == null)
			{
				WriteConsole("Folder path must be selected before processing");
				return;
			}
			// Get images from input folder
			var result = new List<string>();
			string[] extensions = { ".png", ".jpg", ".jpeg" };
			foreach (string file in Directory.EnumerateFiles(InputFolder, "*.*", SearchOption.AllDirectories)
				.Where(s => extensions.Any(ext => ext == Path.GetExtension(s))))
			{
				var image = Image.FromFile(file);
				if (Scaled)
				{
					var scaledImage = ResizeImage(image, (int)Width, (int)Height);
					using MemoryStream memoryStream = new();
					scaledImage.Save(memoryStream, ImageFormat.Png);
					var imageBytes = memoryStream.ToArray();
					var path = Path.Combine(OutputFolder, Path.GetFileName(file));
					File.WriteAllBytes(path, imageBytes);
					WriteConsole("Image written to " + path);
					ProgressValue = 100;
				}
			}
		}

		private static Bitmap ResizeImage(Image image, int width, int height)
		{
			var destRect = new Rectangle(0, 0, width, height);
			var destImage = new Bitmap(width, height);
			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using var graphics = Graphics.FromImage(destImage);
			graphics.CompositingMode = CompositingMode.SourceCopy;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;

			using var wrapMode = new ImageAttributes();
			wrapMode.SetWrapMode(WrapMode.TileFlipXY);
			graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

			return destImage;
		}

		[RelayCommand]
		private void Process() => ProcessImages();

		partial void OnScaledChanged(bool value)
		{
			WidthEnabled = value;
			HeightEnabled = value;
		}

		partial void OnUseSameFolderChanged(bool value)
		{
			OutputFolderEnabled = !value;
		}
	}
}
