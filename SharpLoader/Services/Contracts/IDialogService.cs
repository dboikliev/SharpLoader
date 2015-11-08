namespace SharpLoader.Services.Contracts
{
    public interface IDialogService
    {
        string ShowSaveFileDialog();
        string ShowSaveFileDialog(string fileName, string defaultExtension, out bool? result);
        string ShowChooseDirectoryDialog();
    }
}
