using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NSwagStudio.Controls
{
	public class FolderSavePicker : FilePickerBase
	{
		protected override void SelectFile()
		{
			var dlg = new FolderBrowserDialog();
			if (dlg.ShowDialog().ToString() == "OK")
			{
				FilePath = dlg.SelectedPath;
			}
		}
	}
}
