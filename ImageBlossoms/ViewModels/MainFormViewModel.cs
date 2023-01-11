using CommunityToolkit.Mvvm.ComponentModel;
using Eto.Forms;

namespace ImageBlossoms.ViewModels
{
	public partial class MainFormViewModel : ObservableObject
	{
		public readonly Command AboutCommand = new() { MenuText = "About..." };
		public readonly Command ClickMeCommand = new() { MenuText = "Click Me!", ToolBarText = "Click Me!" };
		public readonly Command QuitCommand = new() { MenuText = "Quit", Shortcut = Application.Instance.CommonModifier | Keys.Q };
		public readonly Command ProcessCommand = new() { ToolBarText = "Process" };

		public MainFormViewModel()
		{
			QuitCommand.Executed += (sender, e) => Application.Instance.Quit();
		}
	}
}
