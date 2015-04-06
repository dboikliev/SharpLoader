using System.Diagnostics;
using Microsoft.Win32;

namespace SharpLoader.Services
{
    public class DialogService : IDialogService
    {
        public string ShowSaveFileDialog()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Video File |*.flv";
            dialog.AddExtension = true;
            dialog.DefaultExt = ".flv";
            dialog.ShowDialog();
            return dialog.FileName;
        }
    }
}
