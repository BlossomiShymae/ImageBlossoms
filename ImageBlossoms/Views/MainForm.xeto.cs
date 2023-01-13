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
			new AboutDialog().ShowDialog(this);
		}

		protected void HandleQuit(object sender, EventArgs e)
		{
			Application.Instance.Quit();
		}
	}
}
