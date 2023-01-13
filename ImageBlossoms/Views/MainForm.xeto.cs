using Eto.Forms;
using Eto.Serialization.Xaml;
using ImageBlossoms.ViewModels;
using System;
using System.Linq;

namespace ImageBlossoms.Views
{
	public class MainForm : Form
	{
		public MainForm(MainFormViewModel viewModel)
		{
			DataContext = viewModel;
			XamlReader.Load(this);

			var aspectRatios = FindChild<ComboBox>("comboAspectRatio");
			aspectRatios.DataStore = viewModel.AspectRatios.Keys.ToList();
		}

		protected void HandleAbout(object sender, EventArgs e)
		{
			var aboutDialog = new AboutDialog
			{
				Website = new("https://github.com/BlossomiShymae/ImageBlossoms"),
				WebsiteLabel = "GitHub webpage",
				ProgramDescription = new("A simple batch image processor with blossoming transformations.\nMade with love, bees, and kitties! <3"),
				Developers = new[] { "Blossomi Shymae" },
				License = new("GNU General Public License v3.0"),
			};
			aboutDialog.ShowDialog(this);
		}

		protected void HandleQuit(object sender, EventArgs e)
		{
			Application.Instance.Quit();
		}
	}
}
