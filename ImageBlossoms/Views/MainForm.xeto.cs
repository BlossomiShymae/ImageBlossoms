using Eto.Forms;
using Eto.Serialization.Xaml;
using ImageBlossoms.ViewModels;
using System;

namespace ImageBlossoms.Views
{
	public class MainForm : Form
	{
		public MainForm(MainFormViewModel viewModel)
		{
			DataContext = viewModel;
			XamlReader.Load(this);
		}

		protected void HandleClickMe(object sender, EventArgs e)
		{
			MessageBox.Show("I was clicked!");

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
