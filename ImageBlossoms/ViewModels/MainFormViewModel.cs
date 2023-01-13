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

		public enum AspectRatio
		{
			None,
			Square,
			FourThree,
			ThreeTwo,
			SixteenNine,
			ThreeOne
		}
		public Dictionary<string, AspectRatio> AspectRatios = new();

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
		private string _selectedAspectRatio = null;
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
			AspectRatios.Add("None", AspectRatio.None);
			AspectRatios.Add("1:1 (Square)", AspectRatio.Square);
			AspectRatios.Add("4:3", AspectRatio.FourThree);
			AspectRatios.Add("16:9", AspectRatio.SixteenNine);
			AspectRatios.Add("3:2", AspectRatio.ThreeTwo);
			AspectRatios.Add("3:1", AspectRatio.ThreeOne);
		}

		private static string GetTime() => DateTime.Now.ToString("s");
		private void WriteConsole(string text)
		{
			ConsoleText += GetTime() + " " + text + "\n";
		}

		private void ProcessImages()
		{
			ProgressValue = 0;
			if (String.IsNullOrEmpty(InputFolder))
			{
				WriteConsole("Input folder must be selected");
				return;
			}
			if (!UseSameFolder && String.IsNullOrEmpty(OutputFolder))
			{
				WriteConsole("Output folder must be selected");
				return;
			}

			// Get images from input folder
			var result = new List<string>();
			string[] extensions = { ".png", ".jpg", ".jpeg" };
			foreach (string file in Directory.EnumerateFiles(InputFolder, "*.*", SearchOption.AllDirectories)
				.Where(s => extensions.Any(ext => ext == Path.GetExtension(s))))
			{
				string processedMagicId = ".blossoms";
				if (file.Contains(processedMagicId + "."))
				{
					WriteConsole("Skipping processed image " + file);
					continue;
				}
				var image = Image.FromFile(file);
				if (Scaled)
				{
					var scaledImage = ResizeImage(image, (int)Width, (int)Height);
					using MemoryStream memoryStream = new();
					scaledImage.Save(memoryStream, ImageFormat.Png);
					var imageBytes = memoryStream.ToArray();
					var saveFolder = UseSameFolder ? InputFolder : OutputFolder;
					var path = Path.Combine(saveFolder, $"{Path.GetFileNameWithoutExtension(file)}_{Width}x{Height}{processedMagicId}{Path.GetExtension(file)}");
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

		partial void OnSelectedAspectRatioChanged(string value)
		{
			if (SelectedAspectRatio == null) return;
			var aspectRatio = AspectRatios.GetValueOrDefault(SelectedAspectRatio);
			switch (aspectRatio)
			{
				case AspectRatio.None:
					break;
				case AspectRatio.Square:
					Height = Width;
					break;
				case AspectRatio.FourThree:
					Height = (3.0 / 4.0) * Width;
					break;
				case AspectRatio.ThreeTwo:
					Height = (2.0 / 3.0) * Width;
					break;
				case AspectRatio.SixteenNine:
					Height = (9.0 / 16.0) * Width;
					break;
				case AspectRatio.ThreeOne:
					Height = (1.0 / 3.0) * Width;
					break;
				default:
					break;
			}
		}
	}
}
