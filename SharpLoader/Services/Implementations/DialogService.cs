using System.Windows.Forms;
using SharpLoader.Services.Contracts;
using Application = System.Windows.Application;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace SharpLoader.Services.Implementations
{
    public class DialogService : IDialogService
    {
        public string ShowSaveFileDialog()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "Video File (*.mp4, *.avi, *.flv)|*.mp4;*.avi;*.flv";
            dialog.AddExtension = true;
            dialog.ShowDialog();
            var result = dialog.ShowDialog();
            return result != null ? dialog.FileName : null;
        }

        public string ShowSaveFileDialog(string fileName, string defaultExtension, out bool? result)
        {
            bool? isSuccessful = false;
            var res = Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "Video File (*.mp4, *.avi, *.flv)|*.mp4;*.avi;*.flv";
                dialog.AddExtension = true;
                dialog.DefaultExt = defaultExtension;
                dialog.FileName = fileName;
                isSuccessful = dialog.ShowDialog();
                return dialog.FileName;
            });
            result = isSuccessful;
            return res;
        }

        public string ShowChooseDirectoryDialog()
        {
            var res = Application.Current.Dispatcher.Invoke(() =>
            {
                var dialog = new FolderBrowserDialog();
                dialog.ShowDialog();
                return dialog.SelectedPath;
            });
            return res;
        }
    }
}