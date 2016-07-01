using System.Windows.Forms;

namespace NSwagStudio.Controls
{
    public class FileSavePicker : FilePickerBase
    {
        protected override void SelectFile()
        {
            var dlg = new SaveFileDialog();
            dlg.DefaultExt = DefaultExtension;
            dlg.Filter = Filter;
            if (dlg.ShowDialog() == DialogResult.OK)
                FilePath = dlg.FileName;
        }
    }
}