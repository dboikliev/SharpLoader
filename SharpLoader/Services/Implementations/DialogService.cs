using Microsoft.Win32;
using SharpLoader.Services.Contracts;

namespace SharpLoader.Services.Implementations
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
