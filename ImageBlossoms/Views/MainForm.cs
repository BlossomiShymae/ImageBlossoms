using Eto.Drawing;
using Eto.Forms;
using ImageBlossoms.ViewModels;

namespace ImageBlossoms.Views
{
	public partial class MainForm : Form
	{
		public MainForm(MainFormViewModel viewModel)
		{
			Title = "ImageBlossoms";
			MinimumSize = new Size(600, 400);

			var inputFolderPicker = new FilePicker { FileAction = Eto.FileAction.SelectFolder };
			var outputFolderPicker = new FilePicker { FileAction = Eto.FileAction.SelectFolder };
			var useOneFolderCheckBox = new CheckBox { Text = "Use same folder for output" };
			inputFolderPicker.FilePathChanged += (s, e) => viewModel.InputFolder = inputFolderPicker.FilePath;
			outputFolderPicker.FilePathChanged += (s, e) => viewModel.OutputFolder = outputFolderPicker.FilePath;

			var scaleCheckBox = new CheckBox { Text = "Scale" };
			var aspectRatioComboBox = new ComboBox { Text = "Aspect Ratio" };
			var widthStepper = new NumericStepper { MinValue = 1 };
			var heightStepper = new NumericStepper { MinValue = 1 };
			scaleCheckBox.CheckedBinding.BindDataContext<MainFormViewModel>(c => c.IsScaled, DualBindingMode.TwoWay);

			var consoleTextArea = new TextArea { Enabled = false };
			var processingProgressBar = new ProgressBar { Value = 0 };

			Content = new TableLayout
			{
				Padding = 10,
				Spacing = new Size(2, 2),
				Rows =
				{
					processingProgressBar,
					new TableRow("Select input folder"),
					new TableRow(inputFolderPicker),
					new TableRow(useOneFolderCheckBox),
					new TableRow("Select output folder"),
					new TableRow(outputFolderPicker),
					new TableRow("Filters"),
					new TableRow(new StackLayout
					{
						Orientation = Orientation.Horizontal,
						HorizontalContentAlignment = HorizontalAlignment.Stretch,
						Spacing = 5,
						Items =
						{
							scaleCheckBox,
							aspectRatioComboBox,
							new Label { Text = "Width" },
							widthStepper,
							new Label { Text = "Pixels" },
							new Label { Text = "Height" },
							heightStepper,
							new Label { Text = "Pixels"}
						}
					}),
					consoleTextArea
				}
			};

			// create a few commands that can be used for the menu and toolbar
			viewModel.ClickMeCommand.Executed += (sender, e) => MessageBox.Show(this, "I was clicked!");
			viewModel.AboutCommand.Executed += (sender, e) => new AboutDialog().ShowDialog(this);

			// create menu
			Menu = new MenuBar
			{
				Items =
				{
					// File submenu
					new SubMenuItem { Text = "&File", Items = { viewModel.ClickMeCommand } },
					// new SubMenuItem { Text = "&Edit", Items = { /* commands/items */ } },
					// new SubMenuItem { Text = "&View", Items = { /* commands/items */ } },
				},
				ApplicationItems =
				{
					// application (OS X) or file menu (others)
					new ButtonMenuItem { Text = "&Preferences..." },
				},
				QuitItem = viewModel.QuitCommand,
				AboutItem = viewModel.AboutCommand
			};

			// create toolbar			
			ToolBar = new ToolBar { Items = { viewModel.ProcessCommand } };

			// Set DataContext
			DataContext = viewModel;
		}
	}
}
