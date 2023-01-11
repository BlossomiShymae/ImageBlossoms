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
			MinimumSize = new Size(800, 600);

			Content = new TableLayout
			{
				Padding = 10,
				Spacing = new Size(2, 2),
				Rows =
				{
					new TableRow("Select input folder"),
					new TableRow(new FilePicker { FileAction = Eto.FileAction.SelectFolder }),
					new TableRow("Select output folder"),
					new TableRow(new FilePicker { FileAction = Eto.FileAction.SelectFolder }),
					new TableRow("Filters"),
					new TableRow(new StackLayout
					{
						Orientation = Orientation.Horizontal,
						HorizontalContentAlignment = HorizontalAlignment.Stretch,
						Spacing = 5,
						Items =
						{
							new CheckBox
							{
								Text = "Scale"
							},
							new TextBox
							{
								PlaceholderText = "Width"
							},
							new TextBox
							{
								PlaceholderText = "Height"
							}
						}
					}),
					null,
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
		}
	}
}
