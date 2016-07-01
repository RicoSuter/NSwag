using System.Windows.Forms;

namespace NSwagStudio.Controls
{
    public class FileOpenPicker : FilePickerBase
    {
        protected override void SelectFile()
        {
            var dlg = new OpenFileDialog();
            dlg.DefaultExt = DefaultExtension;
            dlg.Filter = Filter;
            if (dlg.ShowDialog() == DialogResult.OK)
                FilePath = dlg.FileName;
        }
    }
}