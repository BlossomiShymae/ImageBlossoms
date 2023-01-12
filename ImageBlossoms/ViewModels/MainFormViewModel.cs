using CommunityToolkit.Mvvm.ComponentModel;
using Eto.Forms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace ImageBlossoms.ViewModels
{
	public partial class MainFormViewModel : ObservableObject
	{
		public readonly Command AboutCommand = new() { MenuText = "About..." };
		public readonly Command ClickMeCommand = new() { MenuText = "Click Me!", ToolBarText = "Click Me!" };
		public readonly Command QuitCommand = new() { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
		public readonly Command ProcessCommand = new() { ToolBarText = "Process" };

		[ObservableProperty]
		private string _inputFolder;
		[ObservableProperty]
		private string _outputFolder;
		[ObservableProperty]
		private bool _isScaled = false;
		[ObservableProperty]
		private string _width = "0";
		[ObservableProperty]
		private string _height = "0";

		public MainFormViewModel()
		{
			QuitCommand.Executed += (sender, e) => Application.Instance.Quit();
			ProcessCommand.Executed += (sender, e) => ProcessImages();
		}

		private void ProcessImages()
		{
			if (InputFolder == null || OutputFolder == null) throw new Exception("Folder path is null");
			// Get images from input folder
			var result = new List<string>();
			string[] extensions = { ".png", ".jpg", ".jpeg" };
			foreach (string file in Directory.EnumerateFiles(InputFolder, "*.*", SearchOption.AllDirectories)
				.Where(s => extensions.Any(ext => ext == Path.GetExtension(s))))
			{
				var image = System.Drawing.Image.FromFile(file);
				if (IsScaled)
				{
					var scaledImage = ResizeImage(image, Int32.Parse(Width), Int32.Parse(Height));
					using MemoryStream memoryStream = new();
					scaledImage.Save(memoryStream, ImageFormat.Png);
					var imageBytes = memoryStream.ToArray();
					var path = Path.Combine(OutputFolder, Path.GetFileName(file));
					File.WriteAllBytes(path, imageBytes);
					Trace.WriteLine("wrote to path " + path);
				}
			}
		}

		private static System.Drawing.Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
		{
			var destRect = new System.Drawing.Rectangle(0, 0, width, height);
			var destImage = new System.Drawing.Bitmap(width, height);
			destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

			using var graphics = System.Drawing.Graphics.FromImage(destImage);
			graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
			graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
			graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			using var wrapMode = new ImageAttributes();
			wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
			graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);

			return destImage;
		}
	}
}
